using System;

namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Par�metros para realizar consultas paginadas desde el cliente.
    /// Permiten construir la cadena de consulta para la API.
    /// </summary>
    public class PaginationParams
    {
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        /// <summary>N�mero de p�gina (basado en 1).</summary>
        public int Page { get; set; } = 1;
        /// <summary>N�mero de elementos por p�gina (m�ximo 100).</summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        /// <summary>T�rmino de b�squeda opcional.</summary>
        public string? Search { get; set; }
        /// <summary>Campo por el cual ordenar.</summary>
        public string? SortBy { get; set; } = "OrderDate";
        /// <summary>Direcci�n del ordenamiento (asc/desc).</summary>
        public string SortDirection { get; set; } = "desc";

        /// <summary>
        /// Convierte los par�metros a una cadena de consulta (query string).
        /// </summary>
        public string ToQueryString()
        {
            var queryParams = new List<string>
            {
                $"page={Page}",
                $"pageSize={PageSize}"
            };

            if (!string.IsNullOrWhiteSpace(Search))
                queryParams.Add($"search={Uri.EscapeDataString(Search)}");
            if (!string.IsNullOrWhiteSpace(SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(SortBy)}");
            if (!string.IsNullOrWhiteSpace(SortDirection))
                queryParams.Add($"sortDirection={Uri.EscapeDataString(SortDirection)}");

            return string.Join("&", queryParams);
        }
    }

    /// <summary>
    /// Resultado paginado gen�rico usado en la aplicaci�n cliente.
    /// </summary>
    /// <typeparam name="T">Tipo de elementos contenidos.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>Lista de elementos en la p�gina actual.</summary>
        public List<T> Items { get; set; } = new();
        /// <summary>N�mero total de elementos.</summary>
        public int TotalCount { get; set; }
        /// <summary>P�gina actual (basada en 1).</summary>
        public int CurrentPage { get; set; }
        /// <summary>N�mero de elementos por p�gina.</summary>
        public int PageSize { get; set; }
        /// <summary>N�mero total de p�ginas calculado autom�ticamente.</summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        /// <summary>Indica si existe una p�gina anterior.</summary>
        public bool HasPreviousPage => CurrentPage > 1;
        /// <summary>Indica si existe una p�gina siguiente.</summary>
        public bool HasNextPage => CurrentPage < TotalPages;
        /// <summary>�ndice del primer elemento mostrado.</summary>
        public int StartIndex => (CurrentPage - 1) * PageSize + 1;
        /// <summary>�ndice del �ltimo elemento mostrado.</summary>
        public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalCount);
    }
}