/*
    Nombre del procedimiento: sp_ProductosBajoInventario
    Esquema: Production

    Descripción:
    Este procedimiento almacenado retorna una lista de productos cuyo inventario actual es menor al valor especificado por @StockMinimo.
    Incluye información relevante como el nombre del producto, número de producto, nivel de stock de seguridad, inventario actual,
    punto de reorden, ubicación del producto y el estado del inventario (por debajo del stock de seguridad, por debajo del punto de reorden o bajo inventario).

    Parámetros:
    @StockMinimo INT = 10
        Valor mínimo de inventario para considerar un producto como bajo inventario. Por defecto es 10.

    Salida:
    - ProductID: Identificador único del producto.
    - ProductName: Nombre del producto.
    - ProductNumber: Número de producto.
    - SafetyStockLevel: Nivel de stock de seguridad.
    - CurrentInventory: Inventario actual calculado como la suma de las cantidades en inventario.
    - ReorderPoint: Punto de reorden del producto.
    - ProductLocation: Ubicación del producto.
    - InventoryStatus: Estado del inventario según el nivel actual comparado con los umbrales de seguridad y reorden.

    Uso:
    exec [Production].[sp_ProductosBajoInventario] @StockMinimo = 10
*/

CREATE PROCEDURE [Production].[sp_ProductosBajoInventario]
    @StockMinimo INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        SELECT 
            p.ProductID,
            p.Name AS ProductName,
            p.ProductNumber,
            p.SafetyStockLevel,
            SUM(pi.Quantity) AS CurrentInventory,
            p.ReorderPoint,
            pl.Name AS ProductLocation,
            CASE 
                WHEN SUM(pi.Quantity) < p.SafetyStockLevel THEN 'Below Safety Stock'
                WHEN SUM(pi.Quantity) < p.ReorderPoint THEN 'Below Reorder Point'
                ELSE 'Low Stock'
            END AS InventoryStatus
        FROM 
            Production.Product p
            LEFT JOIN Production.ProductInventory pi 
                ON p.ProductID = pi.ProductID
            LEFT JOIN Production.Location pl 
                ON pi.LocationID = pl.LocationID
        WHERE 
            p.FinishedGoodsFlag = 1
        GROUP BY 
            p.ProductID,
            p.Name,
            p.ProductNumber,
            p.SafetyStockLevel,
            p.ReorderPoint,
            pl.Name
        HAVING 
            SUM(pi.Quantity) < @StockMinimo
        ORDER BY 
            SUM(pi.Quantity) ASC;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

--exec [Production].[sp_ProductosBajoInventario]
