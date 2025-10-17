/**********************************************************************************************
* Nombre del procedimiento: [Production].[sp_Top10ProductosMasVendidos]
* Descripción:
*   Devuelve los 10 productos más vendidos, mostrando el ID del producto, nombre, número,
*   cantidad total vendida, ingresos totales y número de órdenes completadas.
*
* Parámetros:
*   No recibe parámetros.
*
* Retorno:
*   - ProductID: Identificador único del producto.
*   - ProductName: Nombre del producto.
*   - ProductNumber: Número del producto.
*   - TotalQuantitySold: Cantidad total vendida del producto.
*   - TotalRevenue: Ingresos totales generados por el producto.
*   - NumberOfOrders: Número de órdenes completadas en las que aparece el producto.
*
* Notas:
*   - Solo se consideran órdenes con estado completado (Status = 5).
*   - Los resultados se ordenan por cantidad total vendida en orden descendente.
**********************************************************************************************/

CREATE PROCEDURE [Production].[sp_Top10ProductosMasVendidos]
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT TOP 10
            p.ProductID,
            p.Name AS ProductName,
            p.ProductNumber,
            SUM(sod.OrderQty) AS TotalQuantitySold,
            SUM(sod.LineTotal) AS TotalRevenue,
            COUNT(DISTINCT soh.SalesOrderID) AS NumberOfOrders
        FROM 
            Production.Product p
            INNER JOIN Sales.SalesOrderDetail sod 
                ON p.ProductID = sod.ProductID
            INNER JOIN Sales.SalesOrderHeader soh 
                ON sod.SalesOrderID = soh.SalesOrderID
        WHERE 
            soh.Status = 5 -- Órdenes completadas
        GROUP BY 
            p.ProductID,
            p.Name,
            p.ProductNumber
        ORDER BY 
            TotalQuantitySold DESC;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

--exec [Production].[sp_Top10ProductosMasVendidos]
