using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Enterprise.Api.Models
{
    /// <summary>
    /// Par�metros para realizar consultas paginadas.
    /// </summary>
    public class PaginationParams
    {
        private int _pageSize = 10;
        private const int MaxPageSize = 100;

        /// <summary>
        /// N�mero de p�gina (basado en 1).
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "El n�mero de p�gina debe ser mayor a 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// N�mero de elementos por p�gina (m�ximo 100).
        /// </summary>
        [Range(1, MaxPageSize, ErrorMessage = "El tama�o de p�gina debe estar entre 1 y 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// T�rmino de b�squeda opcional.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Campo por el cual ordenar.
        /// </summary>
        public string? SortBy { get; set; } = "EmployeeId";

        /// <summary>
        /// Direcci�n del ordenamiento (asc/desc).
        /// </summary>
        public string SortDirection { get; set; } = "asc";
    }

    /// <summary>
    /// Resultado paginado gen�rico.
    /// </summary>
    /// <typeparam name="T">Tipo de datos que se est�n paginando.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Lista de elementos de la p�gina actual.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// N�mero total de elementos en todas las p�ginas.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// P�gina actual (basada en 1).
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// N�mero de elementos por p�gina.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// N�mero total de p�ginas.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// Indica si hay una p�gina anterior.
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Indica si hay una p�gina siguiente.
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// N�mero del primer elemento de la p�gina actual.
        /// </summary>
        public int StartIndex => (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// N�mero del �ltimo elemento de la p�gina actual.
        /// </summary>
        public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalCount);
    }
}