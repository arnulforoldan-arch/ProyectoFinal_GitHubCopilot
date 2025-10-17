using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdventureWorks.Enterprise.WebApp.Models;
using AdventureWorks.Enterprise.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AdventureWorks.Enterprise.WebApp.Components.Pages
{
    /// <summary>
    /// Componente de reporte de productos con bajo inventario.
    /// Proporciona filtrado, ordenamiento, paginación y exportación (CSV, Excel, impresión).
    /// </summary>
    public partial class LowInventoryReport : ComponentBase, IDisposable
    {
        /// <summary>Servicio de acceso a la API.</summary>
        [Inject] private ApiService ApiService { get; set; } = default!;
        /// <summary>Runtime de JavaScript para operaciones de exportación.</summary>
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool isLoading = true;
        private string searchTerm = string.Empty;
        private string sortBy = "ProductName";
        private string sortDirection = "asc";
        private int currentPage = 1;
        private int pageSize = 10;

        private List<ProductoBajoInventarioDto> allItems = new();
        private PagedResult<ProductoBajoInventarioDto> pagedResult = new();
        private int totalLowInventory = 0;
        private Timer? searchTimer;

        /// <summary>
        /// Inicializa el componente cargando los datos iniciales.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        #region Carga de Datos
        /// <summary>
        /// Carga la lista completa de productos con bajo inventario y calcula acumulados.
        /// </summary>
        private async Task LoadData()
        {
            try
            {
                isLoading = true;
                StateHasChanged();

                var data = await ApiService.GetProductLowInventoryAsync();
                allItems = data ?? new List<ProductoBajoInventarioDto>();
                totalLowInventory = allItems.Sum(i => Math.Max(0, i.CurrentInventory));
                pagedResult = SimulateClientPagination(allItems);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
        #endregion

        #region Paginación y Ordenamiento
        /// <summary>
        /// Simula la paginación, ordenamiento y búsqueda del lado cliente.
        /// </summary>
        private PagedResult<ProductoBajoInventarioDto> SimulateClientPagination(List<ProductoBajoInventarioDto> source)
        {
            var filtered = source.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var s = searchTerm.ToLower();
                filtered = filtered.Where(i =>
                    (i.ProductName?.ToLower().Contains(s) ?? false) ||
                    (i.ProductNumber?.ToLower().Contains(s) ?? false) ||
                    (i.ProductLocation?.ToLower().Contains(s) ?? false) ||
                    i.SafetyStockLevel.ToString().Contains(s) ||
                    i.ReorderPoint.ToString().Contains(s) ||
                    i.CurrentInventory.ToString().Contains(s));
            }

            var isDesc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
            filtered = sortBy.ToLower() switch
            {
                "productname" => isDesc ? filtered.OrderByDescending(i => i.ProductName) : filtered.OrderBy(i => i.ProductName),
                "productnumber" => isDesc ? filtered.OrderByDescending(i => i.ProductNumber) : filtered.OrderBy(i => i.ProductNumber),
                "currentinventory" => isDesc ? filtered.OrderByDescending(i => i.CurrentInventory) : filtered.OrderBy(i => i.CurrentInventory),
                "safetystocklevel" => isDesc ? filtered.OrderByDescending(i => i.SafetyStockLevel) : filtered.OrderBy(i => i.SafetyStockLevel),
                "level" => isDesc ? filtered.OrderByDescending(i => GetLevelPercent(i)) : filtered.OrderBy(i => GetLevelPercent(i)),
                "inventorystatus" => isDesc ? filtered.OrderByDescending(i => GetStatusRank(i)) : filtered.OrderBy(i => GetStatusRank(i)),
                _ => isDesc ? filtered.OrderByDescending(i => i.ProductName) : filtered.OrderBy(i => i.ProductName)
            };

            var total = filtered.Count();
            var items = filtered.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<ProductoBajoInventarioDto>
            {
                Items = items,
                TotalCount = total,
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Navega a una página específica si está dentro de rango.
        /// </summary>
        private async Task NavigateToPageAsync(int targetPage)
        {
            var maxPages = pagedResult?.TotalPages ?? 1;
            if (targetPage < 1 || targetPage > maxPages || targetPage == currentPage)
                return;
            currentPage = targetPage;
            pagedResult = SimulateClientPagination(allItems);
            await Task.CompletedTask;
        }

        private bool CanGoToPreviousPage() => currentPage > 1;
        private bool CanGoToNextPage() => currentPage < (pagedResult?.TotalPages ?? 1);

        /// <summary>
        /// Obtiene la lista de números de página visibles (ventana dinámica).
        /// </summary>
        private List<int> GetPageNumbers()
        {
            if (pagedResult == null) return new List<int>();
            const int maxVisiblePages = 5;
            var pages = new List<int>();
            var totalPages = pagedResult.TotalPages;
            if (totalPages <= maxVisiblePages)
            {
                for (int i = 1; i <= totalPages; i++) pages.Add(i);
            }
            else
            {
                int start = Math.Max(1, currentPage - 2);
                int end = Math.Min(totalPages, start + maxVisiblePages - 1);
                if (end - start < maxVisiblePages - 1)
                    start = Math.Max(1, end - maxVisiblePages + 1);
                for (int i = start; i <= end; i++) pages.Add(i);
            }
            return pages;
        }
        #endregion

        #region Filtros y Búsqueda
        /// <summary>
        /// Evento disparado cuando el término de búsqueda cambia (debounce 500 ms).
        /// </summary>
        private void OnSearchChanged()
        {
            searchTimer?.Dispose();
            searchTimer = new Timer(async _ =>
            {
                await InvokeAsync(() =>
                {
                    currentPage = 1;
                    pagedResult = SimulateClientPagination(allItems);
                    StateHasChanged();
                });
            }, null, 500, Timeout.Infinite);
        }

        /// <summary>Recalcula la paginación al cambiar el tamaño de página.</summary>
        private async Task OnPageSizeChangedAsync()
        {
            currentPage = 1;
            pagedResult = SimulateClientPagination(allItems);
            await Task.CompletedTask;
        }

        /// <summary>Recalcula la lista al cambiar el criterio de ordenamiento.</summary>
        private async Task OnSortChangedAsync()
        {
            currentPage = 1;
            pagedResult = SimulateClientPagination(allItems);
            await Task.CompletedTask;
        }

        /// <summary>Invierte la dirección de ordenamiento (asc/desc).</summary>
        private async Task ToggleSortDirection()
        {
            sortDirection = sortDirection == "asc" ? "desc" : "asc";
            pagedResult = SimulateClientPagination(allItems);
            await Task.CompletedTask;
        }

        /// <summary>Limpiar filtros y reiniciar a la primera página.</summary>
        private async Task ClearFilters()
        {
            searchTerm = string.Empty;
            currentPage = 1;
            pagedResult = SimulateClientPagination(allItems);
            await Task.CompletedTask;
        }

        /// <summary>Recarga los datos desde la fuente API (reinicia filtros).</summary>
        private async Task RefreshData()
        {
            searchTerm = string.Empty;
            currentPage = 1;
            await LoadData();
        }
        #endregion

        #region Cálculo de Estados
        private int GetStatusRank(ProductoBajoInventarioDto inv)
        {
            if (inv.CurrentInventory <= 0) return 0;
            if (inv.CurrentInventory < inv.SafetyStockLevel) return 1;
            return 2;
        }

        private string GetQuantityStatusClass(ProductoBajoInventarioDto inv)
        {
            if (inv.CurrentInventory <= 0) return "status-cancelled";
            if (inv.CurrentInventory < inv.SafetyStockLevel) return "status-pending";
            return "status-completed";
        }

        private string GetQuantityStatusText(ProductoBajoInventarioDto inv)
        {
            if (inv.CurrentInventory <= 0) return "Sin stock";
            if (inv.CurrentInventory < inv.SafetyStockLevel) return "Bajo";
            return "Adecuado";
        }

        private int GetLevelPercent(ProductoBajoInventarioDto inv)
        {
            var baseline = Math.Max(1, Math.Max(inv.SafetyStockLevel, inv.ReorderPoint));
            var pct = (int)Math.Round(inv.CurrentInventory * 100.0 / baseline);
            return Math.Clamp(pct, 0, 100);
        }
        #endregion

        #region Exportaciones
        /// <summary>Exporta el reporte completo en formato CSV.</summary>
        private async Task ExportCsv()
        {
            var backupSize = pageSize;
            pageSize = int.MaxValue;
            var data = SimulateClientPagination(allItems);
            pageSize = backupSize;

            var rows = new List<string>();
            string CsvEscape(string? s) => string.IsNullOrEmpty(s) ? "" : ($"\"{s.Replace("\"", "\"\"")}\"");
            rows.Add("Estado,Producto,Número,Seguridad,Reorden,Inventario,Nivel,Ubicación");
            foreach (var item in data.Items)
            {
                var status = GetQuantityStatusText(item);
                var nivel = GetLevelPercent(item) + "%";
                rows.Add(string.Join(",", new[]
                {
                    CsvEscape(status), CsvEscape(item.ProductName), CsvEscape(item.ProductNumber),
                    CsvEscape(item.SafetyStockLevel.ToString()), CsvEscape(item.ReorderPoint.ToString()),
                    CsvEscape(item.CurrentInventory.ToString()), CsvEscape(nivel), CsvEscape(item.ProductLocation)
                }));
            }
            var csv = string.Join("\n", rows);
            await JS.InvokeVoidAsync("awExport.downloadCsv", $"reporte_bajo_stock_{DateTime.Now:yyyyMMdd_HHmm}.csv", csv);
        }

        /// <summary>Exporta el reporte completo en formato HTML compatible con Excel.</summary>
        private async Task ExportExcel()
        {
            var backupSize = pageSize;
            pageSize = int.MaxValue;
            var data = SimulateClientPagination(allItems);
            pageSize = backupSize;

            var sb = new StringBuilder();
            sb.Append("<html><head><meta charset='UTF-8'></head><body>");
            sb.Append("<table border='1'><thead><tr>");
            var headers = new[] { "Estado", "Producto", "Número", "Seguridad", "Reorden", "Inventario", "Nivel", "Ubicación" };
            foreach (var h in headers) sb.Append($"<th>{System.Net.WebUtility.HtmlEncode(h)}</th>");
            sb.Append("</tr></thead><tbody>");
            foreach (var item in data.Items)
            {
                var status = GetQuantityStatusText(item);
                var nivel = GetLevelPercent(item) + "%";
                string H(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
                sb.Append("<tr>");
                foreach (var v in new[] { status, item.ProductName, item.ProductNumber, item.SafetyStockLevel.ToString(), item.ReorderPoint.ToString(), item.CurrentInventory.ToString(), nivel, item.ProductLocation })
                    sb.Append($"<td>{H(v)}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></body></html>");
            await JS.InvokeVoidAsync("awExport.downloadHtml", $"reporte_bajo_stock_{DateTime.Now:yyyyMMdd_HHmm}.xls", sb.ToString(), "application/vnd.ms-excel");
        }

        /// <summary>Imprime el reporte de la página actual.</summary>
        private async Task PrintReport()
        {
            var sb = new StringBuilder();
            sb.Append("<html><head><meta charset='UTF-8'><title>Reporte Bajo Stock</title>");
            sb.Append("<style>body{font-family:Arial,Helvetica,sans-serif;padding:16px;} table{width:100%;border-collapse:collapse;} th,td{border:1px solid #ddd;padding:8px;} th{background:#f5f5f5;} .right{text-align:right;}</style>");
            sb.Append("</head><body>");
            sb.Append($"<h2>Reporte de Productos con Bajo Stock - {DateTime.Now:dd/MM/yyyy HH:mm}</h2>");
            sb.Append("<table><thead><tr>");
            var headers = new[] { "Estado", "Producto", "Número", "Seguridad", "Reorden", "Inventario", "Nivel", "Ubicación" };
            foreach (var h in headers) sb.Append($"<th>{System.Net.WebUtility.HtmlEncode(h)}</th>");
            sb.Append("</tr></thead><tbody>");
            var items = pagedResult.Items;
            foreach (var item in items)
            {
                var status = GetQuantityStatusText(item);
                var nivel = GetLevelPercent(item) + "%";
                string H(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
                sb.Append("<tr>");
                foreach (var v in new[] { status, item.ProductName, item.ProductNumber, item.SafetyStockLevel.ToString(), item.ReorderPoint.ToString(), item.CurrentInventory.ToString(), nivel, item.ProductLocation })
                    sb.Append($"<td>{H(v)}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></body></html>");
            await JS.InvokeVoidAsync("awExport.printHtml", sb.ToString());
        }
        #endregion

        /// <summary>
        /// Libera recursos del componente (timer de búsqueda).
        /// </summary>
        public void Dispose()
        {
            searchTimer?.Dispose();
        }
    }
}
