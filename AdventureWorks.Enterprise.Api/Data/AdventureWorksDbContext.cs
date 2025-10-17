using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Contexto de base de datos para AdventureWorks.Enterprise.Api.
    /// Gestiona el acceso a las entidades principales del modelo AdventureWorks.
    /// </summary>
    public class AdventureWorksDbContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia del contexto de base de datos.
        /// </summary>
        /// <param name="options">Opciones de configuración de DbContext.</param>
        public AdventureWorksDbContext(DbContextOptions<AdventureWorksDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Conjunto de empleados.
        /// </summary>
        public DbSet<Employee> Employees { get; set; } = default!;

        /// <summary>
        /// Conjunto de productos.
        /// </summary>
        public DbSet<Product> Products { get; set; } = default!;

        /// <summary>
        /// Conjunto de órdenes de venta.
        /// </summary>
        public DbSet<SalesOrder> SalesOrders { get; set; } = default!;

        /// <summary>
        /// Conjunto de materiales de ensamblaje.
        /// </summary>
        public DbSet<BillOfMaterials> BillOfMaterials { get; set; } = default!;

        /// <summary>
        /// Conjunto de inventarios de productos.
        /// </summary>
        public DbSet<ProductInventory> ProductInventories { get; set; } = default!;

        /// <summary>
        /// Configura el modelo de datos y las relaciones entre entidades.
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos de EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInventory>()
                .HasKey(pi => new { pi.ProductId, pi.LocationId });

            // Configuración específica para SalesOrder para evitar conflictos con triggers
            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.ToTable("SalesOrderHeader", "Sales", tb => tb.HasTrigger("uSalesOrderHeader"));
                entity.Property(e => e.SalesOrderId)
                    .HasColumnName("SalesOrderID")
                    .ValueGeneratedNever();
                entity.Property(e => e.SubTotal)
                    .HasColumnType("money");
                entity.Property(e => e.TaxAmt)
                    .HasColumnType("money");
                entity.Property(e => e.Freight)
                    .HasColumnType("money");
                entity.Property(e => e.TotalDue)
                    .HasColumnType("money");
            });

            // Configuraciones para Product para evitar warnings de decimales
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ListPrice)
                    .HasColumnType("money");
                entity.Property(e => e.StandardCost)
                    .HasColumnType("money");
                entity.Property(e => e.Weight)
                    .HasColumnType("decimal(8,2)");
            });

            // ... otras configuraciones
        }
    }
}