/*
    Nombre del procedimiento: [HumanResources].[sp_EmpleadosMasTiempo]
    Descripción:
        Este procedimiento almacenado obtiene la lista de empleados que han permanecido más tiempo en su departamento actual.
        Utiliza la tabla EmployeeDepartmentHistory para identificar el historial de departamentos de cada empleado y selecciona
        el registro más reciente (donde EndDate es NULL) para calcular los años que lleva en el departamento.
        El resultado incluye el ID del empleado, LoginID, título del puesto, nombre del departamento, fecha de inicio en el departamento
        y los años en el departamento, ordenados de mayor a menor tiempo.

    Parámetros:
        No recibe parámetros.

    Retorno:
        - EmployeeID: Identificador del empleado.
        - LoginID: Usuario de inicio de sesión.
        - JobTitle: Título del puesto.
        - DepartmentName: Nombre del departamento.
        - DepartmentStartDate: Fecha de inicio en el departamento.
        - YearsInDepartment: Años en el departamento actual.

    Ejemplo de uso:
        EXEC [HumanResources].[sp_EmpleadosMasTiempo];
*/

CREATE PROCEDURE [HumanResources].[sp_EmpleadosMasTiempo]
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        WITH LatestDepartmentHistory AS (
            SELECT 
                e.BusinessEntityID,
                e.LoginID,
                e.JobTitle,
                d.Name AS DepartmentName,
                edh.StartDate,
                DATEDIFF(YEAR, edh.StartDate, GETDATE()) AS YearsInDepartment,
                ROW_NUMBER() OVER (PARTITION BY e.BusinessEntityID ORDER BY edh.StartDate DESC) as rn
            FROM 
                HumanResources.Employee e
                INNER JOIN HumanResources.EmployeeDepartmentHistory edh 
                    ON e.BusinessEntityID = edh.BusinessEntityID
                INNER JOIN HumanResources.Department d 
                    ON edh.DepartmentID = d.DepartmentID
            WHERE 
                edh.EndDate IS NULL
        )
        SELECT 
            BusinessEntityID AS EmployeeID,
            LoginID,
            JobTitle,
            DepartmentName,
            StartDate AS DepartmentStartDate,
            YearsInDepartment
        FROM 
            LatestDepartmentHistory
        WHERE 
            rn = 1
        ORDER BY 
            YearsInDepartment DESC;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR('Error in [HumanResources].[sp_EmpleadosMasTiempo]: %s', @ErrorSeverity, @ErrorState, @ErrorMessage);
    END CATCH
END
GO

--exec [HumanResources].[sp_EmpleadosMasTiempo];
