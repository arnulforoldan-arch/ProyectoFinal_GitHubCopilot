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
    /// Componente de listado de inventario de productos.
    /// Proporciona búsqueda, ordenamiento, paginación y exportación (CSV, Excel, impresión).
    /// </summary>
    public partial class ProductList : ComponentBase, IDisposable
    {
        [Inject] private ApiService ApiService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private bool isLoading = true;
        private string searchTerm = string.Empty;
        private string sortBy = "ProductId";
        private string sortDirection = "asc";
        private int currentPage = 1;
        private int pageSize = 10;

        private int distinctProducts = 0;
        private int totalQuantity = 0;

        private List<ProductInventory> allInventory = new();
        private PagedResult<ProductInventory>? pagedResult;
        private Dictionary<int, Product> productsById = new();
        private Timer? searchTimer;

        protected override async Task OnInitializedAsync() => await LoadInventory();

        private async Task LoadInventory()
        {
            try
            {
                isLoading = true;
                StateHasChanged();

                var invTask = ApiService.GetAllInventoryAsync();
                var prodTask = ApiService.GetProductsAsync();
                await Task.WhenAll(invTask, prodTask);

                allInventory = invTask.Result;
                var productsList = prodTask.Result ?? new List<Product>();
                productsById = productsList
                    .GroupBy(p => p.ProductId)
                    .Select(g => g.First())
                    .ToDictionary(p => p.ProductId, p => p);

                distinctProducts = allInventory.Select(i => i.ProductId).Distinct().Count();
                totalQuantity = allInventory.Sum(i => (int)i.Quantity);

                pagedResult = SimulateClientPagination(allInventory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando inventario: {ex.Message}");
                allInventory = new();
                productsById = new();
                pagedResult = new PagedResult<ProductInventory>();
                distinctProducts = 0;
                totalQuantity = 0;
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private PagedResult<ProductInventory> SimulateClientPagination(List<ProductInventory> source)
        {
            var filtered = source.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLowerInvariant();
                filtered = filtered.Where(i =>
                {
                    var match = i.ProductId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                               || i.LocationId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                               || (!string.IsNullOrEmpty(i.Shelf) && i.Shelf.Contains(search, StringComparison.OrdinalIgnoreCase))
                               || i.Bin.ToString().Contains(search, StringComparison.OrdinalIgnoreCase);
                    if (!match && productsById.TryGetValue(i.ProductId, out var prod))
                    {
                        match = (!string.IsNullOrEmpty(prod.Name) && prod.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                             || (!string.IsNullOrEmpty(prod.ProductNumber) && prod.ProductNumber.Contains(search, StringComparison.OrdinalIgnoreCase))
                             || (!string.IsNullOrEmpty(prod.Color) && prod.Color.Contains(search, StringComparison.OrdinalIgnoreCase))
                             || prod.SafetyStockLevel.ToString().Contains(search, StringComparison.OrdinalIgnoreCase);
                    }
                    return match;
                });
            }

            var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
            filtered = sortBy.ToLowerInvariant() switch
            {
                "productid" => isDescending ? filtered.OrderByDescending(i => i.ProductId) : filtered.OrderBy(i => i.ProductId),
                "locationid" => isDescending ? filtered.OrderByDescending(i => i.LocationId) : filtered.OrderBy(i => i.LocationId),
                "quantity" => isDescending ? filtered.OrderByDescending(i => i.Quantity) : filtered.OrderBy(i => i.Quantity),
                "shelf" => isDescending ? filtered.OrderByDescending(i => i.Shelf ?? string.Empty) : filtered.OrderBy(i => i.Shelf ?? string.Empty),
                "bin" => isDescending ? filtered.OrderByDescending(i => i.Bin) : filtered.OrderBy(i => i.Bin),
                "status" => isDescending ? filtered.OrderByDescending(GetStatusRank) : filtered.OrderBy(GetStatusRank),
                "level" => isDescending ? filtered.OrderByDescending(GetLevelPercent) : filtered.OrderBy(GetLevelPercent),
                _ => isDescending ? filtered.OrderByDescending(i => i.ProductId) : filtered.OrderBy(i => i.ProductId)
            };

            var totalCount = filtered.Count();
            var items = filtered.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<ProductInventory>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }

        private Task NavigateToPageAsync(int targetPage)
        {
            var maxPages = pagedResult?.TotalPages ?? 1;
            if (targetPage < 1 || targetPage > maxPages || targetPage == currentPage) return Task.CompletedTask;
            currentPage = targetPage;
            pagedResult = SimulateClientPagination(allInventory);
            return Task.CompletedTask;
        }

        private bool CanGoToPreviousPage() => currentPage > 1;
        private bool CanGoToNextPage() => currentPage < (pagedResult?.TotalPages ?? 1);

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
                if (end - start < maxVisiblePages - 1) start = Math.Max(1, end - maxVisiblePages + 1);
                for (int i = start; i <= end; i++) pages.Add(i);
            }
            return pages;
        }

        private void OnSearchChanged()
        {
            searchTimer?.Dispose();
            searchTimer = new Timer(_ =>
            {
                InvokeAsync(() =>
                {
                    currentPage = 1;
                    pagedResult = SimulateClientPagination(allInventory);
                    StateHasChanged();
                });
            }, null, 500, Timeout.Infinite);
        }

        private Task OnPageSizeChangedAsync()
        {
            currentPage = 1;
            pagedResult = SimulateClientPagination(allInventory);
            return Task.CompletedTask;
        }

        private Task OnSortChangedAsync()
        {
            currentPage = 1;
            pagedResult = SimulateClientPagination(allInventory);
            return Task.CompletedTask;
        }

        private Task ToggleSortDirection()
        {
            sortDirection = sortDirection == "asc" ? "desc" : "asc";
            pagedResult = SimulateClientPagination(allInventory);
            return Task.CompletedTask;
        }

        private Task ClearFilters()
        {
            searchTerm = string.Empty;
            currentPage = 1;
            pagedResult = SimulateClientPagination(allInventory);
            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            searchTerm = string.Empty;
            currentPage = 1;
            return LoadInventory();
        }

        private int GetStatusRank(ProductInventory inv)
        {
            if (inv.Quantity == 0) return 0;
            return IsBelowSafetyStock(inv) ? 1 : 2;
        }

        private bool IsBelowSafetyStock(ProductInventory inv)
        {
            if (productsById.TryGetValue(inv.ProductId, out var prod))
            {
                return inv.Quantity < prod.SafetyStockLevel;
            }
            return inv.Quantity < 10;
        }

        private string GetQuantityStatusClass(ProductInventory inv)
        {
            if (inv.Quantity == 0) return "status-cancelled";
            if (productsById.TryGetValue(inv.ProductId, out var prod) && inv.Quantity < prod.SafetyStockLevel) return "status-pending";
            if (!productsById.ContainsKey(inv.ProductId) && inv.Quantity < 10) return "status-pending";
            return "status-completed";
        }

        private string GetQuantityStatusText(ProductInventory inv)
        {
            if (inv.Quantity == 0) return "Sin stock";
            if (productsById.TryGetValue(inv.ProductId, out var prod) && inv.Quantity < prod.SafetyStockLevel) return "Bajo";
            if (!productsById.ContainsKey(inv.ProductId) && inv.Quantity < 10) return "Bajo";
            return "Adecuado";
        }

        private int GetLevelPercent(ProductInventory inv)
        {
            if (!productsById.TryGetValue(inv.ProductId, out var prod))
            {
                var denom = 10;
                return Math.Clamp((int)Math.Round(inv.Quantity * 100.0 / Math.Max(1, denom)), 0, 100);
            }
            var safety = (int)prod.SafetyStockLevel;
            var reorder = (int)prod.ReorderPoint;
            var baseline = Math.Max(1, Math.Max(safety, reorder));
            var percent = (int)Math.Round(inv.Quantity * 100.0 / baseline);
            return Math.Clamp(percent, 0, 100);
        }

        private async Task ExportCsv()
        {
            var backupSize = pageSize;
            pageSize = int.MaxValue;
            var data = SimulateClientPagination(allInventory);
            pageSize = backupSize;

            var rows = new List<string>
            {
                "Estado,Producto,Número,Color,Seguridad,Inventario,Nivel,Ubicación,Estante,Bin"
            };
            string CsvEscape(string? s) => string.IsNullOrEmpty(s) ? string.Empty : $"\"{s.Replace("\"", "\"\"")}\"";
            foreach (var item in data.Items)
            {
                var hasProduct = productsById.TryGetValue(item.ProductId, out var prod);
                var status = GetQuantityStatusText(item);
                var name = hasProduct ? prod!.Name : $"Producto ID: {item.ProductId}";
                var number = hasProduct ? prod!.ProductNumber : string.Empty;
                var color = hasProduct ? (prod!.Color ?? string.Empty) : string.Empty;
                var seguridad = hasProduct ? prod!.SafetyStockLevel.ToString() : "0";
                var inv = item.Quantity.ToString();
                var nivel = GetLevelPercent(item) + "%";
                var ubic = item.LocationId.ToString();
                var shelf = item.Shelf;
                var bin = item.Bin.ToString();
                rows.Add(string.Join(",", new[] { CsvEscape(status), CsvEscape(name), CsvEscape(number), CsvEscape(color), CsvEscape(seguridad), CsvEscape(inv), CsvEscape(nivel), CsvEscape(ubic), CsvEscape(shelf), CsvEscape(bin) }));
            }
            var csv = string.Join("\n", rows);
            await JS.InvokeVoidAsync("awExport.downloadCsv", $"inventario_{DateTime.Now:yyyyMMdd_HHmm}.csv", csv);
        }

        private async Task ExportExcel()
        {
            var backupSize = pageSize;
            pageSize = int.MaxValue;
            var data = SimulateClientPagination(allInventory);
            pageSize = backupSize;

            var sb = new StringBuilder();
            sb.Append("<html><head><meta charset='UTF-8'></head><body>");
            sb.Append("<table border='1'><thead><tr>");
            var headers = new[] { "Estado", "Producto", "Número", "Color", "Seguridad", "Inventario", "Nivel", "Ubicación", "Estante", "Bin" };
            foreach (var h in headers) sb.Append($"<th>{System.Net.WebUtility.HtmlEncode(h)}</th>");
            sb.Append("</tr></thead><tbody>");
            foreach (var item in data.Items)
            {
                var hasProduct = productsById.TryGetValue(item.ProductId, out var prod);
                var status = GetQuantityStatusText(item);
                var name = hasProduct ? prod!.Name : $"Producto ID: {item.ProductId}";
                var number = hasProduct ? prod!.ProductNumber : string.Empty;
                var color = hasProduct ? (prod!.Color ?? string.Empty) : string.Empty;
                var seguridad = hasProduct ? prod!.SafetyStockLevel.ToString() : "0";
                var inv = item.Quantity.ToString();
                var nivel = GetLevelPercent(item) + "%";
                var ubic = item.LocationId.ToString();
                var shelf = item.Shelf;
                var bin = item.Bin.ToString();
                string H(string? s) => System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
                sb.Append("<tr>");
                foreach (var v in new[] { status, name, number, color, seguridad, inv, nivel, ubic, shelf, bin }) sb.Append($"<td>{H(v)}</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></body></html>");
            await JS.InvokeVoidAsync("awExport.downloadHtml", $"inventario_{DateTime.Now:yyyyMMdd_HHmm}.xls", sb.ToString(), "application/vnd.ms-excel");
        }

        private async Task PrintReport()
        {
            var sb = new StringBuilder();
            sb.Append("<html><head><meta charset='UTF-8'><title>Inventario</title>");
            sb.Append("<style>body{font-family:Arial,Helvetica,sans-serif;padding:16px;} table{width:100%;border-collapse:collapse;} th,td{border:1px solid #ddd;padding:8px;} th{background:#f5f5f5;} .right{text-align:right;}</style>");
            sb.Append("</head><body>");
            sb.Append($"<h2>Inventario de Productos - {DateTime.Now:dd/MM/yyyy HH:mm}</h2>");
            sb.Append("<table><thead><tr>");
            var headers = new[] { "Estado", "Producto", "Número", "Color", "Seguridad", "Inventario", "Nivel", "Ubicación", "Estante", "Bin" };
            foreach (var h in headers) sb.Append($"<th>{System.Net.WebUtility.HtmlEncode(h)}</th>");
            sb.Append("</tr></thead><tbody>");
            var items = pagedResult?.Items;
            if (items != null)
            {
                foreach (var item in items)
                {
                    var hasProduct = productsById.TryGetValue(item.ProductId, out var prod);
                    var status = GetQuantityStatusText(item);
                    var name = hasProduct ? prod!.Name : $"Producto ID: {item.ProductId}";
                    var number = hasProduct ? prod!.ProductNumber : string.Empty;
                    var color = hasProduct ? (prod!.Color ?? string.Empty) : string.Empty;
                    var seguridad = hasProduct ? prod!.SafetyStockLevel.ToString() : "0";
                    var inv = item.Quantity.ToString();
                    var nivel = GetLevelPercent(item) + "%";
                    var ubic = item.LocationId.ToString();
                    var shelf = item.Shelf;
                    var bin = item.Bin.ToString();
                    string H(string? s) => System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
                    sb.Append("<tr>");
                    foreach (var v in new[] { status, name, number, color, seguridad, inv, nivel, ubic, shelf, bin }) sb.Append($"<td>{H(v)}</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</tbody></table></body></html>");
            await JS.InvokeVoidAsync("awExport.printHtml", sb.ToString());
        }

        public void Dispose()
        {
            searchTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
