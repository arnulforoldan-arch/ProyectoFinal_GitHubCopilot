using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Enterprise.Api.Models
{
    /// <summary>
    /// Parámetros para realizar consultas paginadas.
    /// </summary>
    public class PaginationParams
    {
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        /// <summary>
        /// Número de página (basado en 1).
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Número de elementos por página (máximo 100).
        /// </summary>
        [Range(1, MaxPageSize, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Término de búsqueda opcional.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Campo por el cual ordenar.
        /// </summary>
        public string? SortBy { get; set; } = "EmployeeId";

        /// <summary>
        /// Dirección del ordenamiento (asc/desc).
        /// </summary>
        public string SortDirection { get; set; } = "asc";
    }

    /// <summary>
    /// Resultado paginado genérico.
    /// </summary>
    /// <typeparam name="T">Tipo de datos que se están paginando.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista de elementos de la página actual.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Número total de elementos en todas las páginas.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Página actual (basada en 1).
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Número de elementos por página.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Número total de páginas.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Indica si hay una página anterior.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Indica si hay una página siguiente.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Número del primer elemento de la página actual.
        /// </summary>
        public int StartIndex => (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// Número del último elemento de la página actual.
        /// </summary>
        public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalCount);
    }
}