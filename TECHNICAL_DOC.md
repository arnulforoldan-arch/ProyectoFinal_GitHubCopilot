# Documentaci�n T�cnica M�dulo Recursos Humanos (RRHH)

## Alcance
Este documento describe la arquitectura t�cnica del m�dulo de Recursos Humanos enfocado al manejo de empleados. Cubre componentes de la API (endpoints, modelos, DTOs y l�gica principal) y su consumo desde los componentes Blazor `EmployeesList.razor` y `EmployeeDetails.razor`.

## Visi�n General Arquitect�nica
El m�dulo RRHH implementa un patr�n de arquitectura por capas:
- Capa de Datos (API): `AdventureWorksDbContext` accede a tablas AdventureWorks (Person.BusinessEntity, Person.Person, HumanResources.Employee) v�a Entity Framework Core y ADO.NET para operaciones de creaci�n compuestas.
- Capa de Servicios (API): `EmpleadoService` (no mostrado aqu�) encapsula consultas avanzadas (ej. empleados con m�s tiempo en departamento) y procedimientos almacenados.
- Capa de Presentaci�n (API): `EmployeesController` expone endpoints RESTful CRUD y consultas espec�ficas.
- Capa Cliente (Blazor): Servicios `ApiService` y componentes de UI (`EmployeesList.razor`, `EmployeeDetails.razor`) consumen la API con manejo de paginaci�n, b�squeda, orden y acciones CRUD.

## Endpoints Principales (EmployeesController)
Base route: `api/employees`
1. `GET api/employees` -> Lista de empleados activos (filtra `CurrentFlag = true`).
2. `GET api/employees/paged?page=1&pageSize=10&search=&sortBy=EmployeeId&sortDirection=asc` -> Devuelve `PagedResult<Employee>` con:
   - Filtros: `search` aplicado a `LoginId`, `JobTitle`, `NationalIdNumber`, `EmployeeId`.
   - Orden din�mico: por campos (`EmployeeId`, `LoginId`, `JobTitle`, `HireDate`, `BirthDate`, `NationalIdNumber`) y direcci�n asc/desc.
   - Headers de paginaci�n agregados (`X-Pagination-*`).
3. `GET api/employees/count` -> Cantidad total de empleados activos.
4. `GET api/employees/{id}` -> Recupera empleado por ID (valida activo).
5. `POST api/employees` -> Creaci�n compuesta del empleado siguiendo pasos AdventureWorks:
   - Inserta en `Person.BusinessEntity`.
   - Inserta registro m�nimo en `Person.Person`.
   - Inserta en `HumanResources.Employee`.
   Usa transacci�n ADO.NET manual para atomicidad.
6. `PUT api/employees/{id}` -> Actualiza campos editables del empleado (validando id y existencia activa). Maneja concurrencia y errores.
7. `DELETE api/employees/{id}` -> Soft-delete: marca `CurrentFlag = false`.
8. `GET api/employees/mas-tiempo` -> Lista `EmpleadoMasTiempoDto` (consulta de antig�edad por departamento).

## Modelos y DTOs (API)
- `Employee` (Entidad): Campos clave: `EmployeeId (BusinessEntityID)`, `NationalIdNumber`, `LoginId`, `JobTitle`, `BirthDate`, `MaritalStatus`, `Gender`, `HireDate`, `SalariedFlag`, `VacationHours`, `SickLeaveHours`, `CurrentFlag`.
  - Reglas de dominio en creaci�n: longitudes m�ximas, c�digos v�lidos (`MaritalStatus ? {S,M,D,W}`, `Gender ? {M,F}`), mayor de 18 a�os, `HireDate` no futura.
  - Duplicidad: no repetir `NationalIdNumber` ni `LoginId`.
- `PagedResult<T>`: Estructura de respuesta paginada (propiedades `Items`, `TotalCount`, `CurrentPage`, `PageSize`, `TotalPages`, helpers `HasNextPage`, `HasPreviousPage`).
- `PaginationParams`: Define par�metros de consulta de paginaci�n y genera query string en cliente.
- `EmpleadoMasTiempoDto`: Proporciona vista de empleados con mayor antig�edad por departamento (`YearsInDepartment`).

## L�gica de Negocio Principal
1. Listado Activo: S�lo empleados con `CurrentFlag = true` disponibles para operaciones.
2. Paginaci�n Servidor: Query LINQ con filtros y orden; c�lculo de `TotalCount` y subselecci�n con `Skip/Take`.
3. Creaci�n Empleado: Orquestaci�n manual con transacci�n para cumplir integridad multi-tabla AdventureWorks (no se usa EF para Person.* aqu�, se emplea ADO.NET debido a secuencia y m�nima inserci�n requerida).
4. Actualizaci�n Controlada: Solo campos directos del registro `Employee`. No se alteran tablas relacionadas (BusinessEntity, Person) en este flujo.
5. Soft Delete: Preserva hist�rico y evita borrado f�sico, alineado con AdventureWorks.
6. B�squeda Flexible: Coincidencia parcial en varios campos; se normaliza a lower-case.
7. Ordenamiento Din�mico: Switch por nombre de campo, fallback a `EmployeeId`.
8. Consulta Avanzada: `EmpleadoService.ObtenerEmpleadosMasTiempoAsync()` encapsula l�gica (posible uso de procedimientos almacenados / T-SQL avanzado) para c�lculo de antig�edad por departamento.

## Gesti�n de Errores y Validaciones
- Creaci�n: Acumula errores en lista y retorna `400 BadRequest` si existen reglas incumplidas; conflicto (`409`) si duplicados.
- Paginaci�n: Encapsulada en try-catch; retorna `500` con mensaje si hay excepci�n.
- Update: Maneja `DbUpdateConcurrencyException` retornando `404` o `409` seg�n estado.
- Operaciones de baja: Retorna `404` si no encuentra empleado.
- Transacciones: rollback ante excepci�n en la creaci�n compuesta.

## Seguridad y Consistencia
- Validaciones estrictas de c�digos controlados (`Gender`, `MaritalStatus`).
- Evita insertar fechas futuras y menores de edad.
- Soft delete mantiene referencialidad.
- Encabezados de paginaci�n facilitan a cliente la navegaci�n eficiente.

## Consumo en Blazor (EmployeesList.razor)
Responsabilidades:
- Presentar listado paginado, buscador y ordenamiento interactivo.
- Crear, editar y eliminar empleados v�a `ApiService`.
- M�tricas: total empleados (`GetEmployeesCountAsync`), activos, filtrados por p�gina.
- Optimizaciones UX: debounce de b�squeda con `Timer`, simulaci�n de paginaci�n cliente si servidor falla (`SimulateClientPagination`).
Flujo Simplificado:
1. `OnInitializedAsync` -> Carga conteo y primera p�gina (llamadas `GetEmployeesCountAsync` y `GetEmployeesPagedAsync`).
2. Usuario aplica b�squeda / orden / tama�o p�gina -> Regenera par�metros y solicita de nuevo datos.
3. CRUD:
   - Crear: Validaci�n local de campos, llamada `CreateEmployeeAsync`, refresco de m�tricas y p�gina.
   - Editar: Clonado de registro, actualizaci�n con `UpdateEmployeeAsync`.
   - Eliminar: Soft delete con `DeleteEmployeeAsync`, ajuste de p�gina si qued� vac�a.
4. Fallback: Si endpoint paginado falla, se obtiene lista completa y se simula paginaci�n en cliente.

## Consumo en Blazor (EmployeeDetails.razor)
Responsabilidades:
- Mostrar ficha completa del empleado con agrupaci�n sem�ntica (personal, laboral, vacaciones/permisos).
- Calcular m�tricas derivadas (edad, a�os servicio, d�as vacaciones/enfermedad).
- Permitir edici�n modal reutilizando reglas del controlador (validaciones en servidor al guardar). 
Flujo:
1. `OnParametersSetAsync` -> `GetEmployeeAsync(id)`.
2. Si existe -> set `editEmployee` (clon) para edici�n modal.
3. Edici�n -> `UpdateEmployeeAsync`; refresca entidad local.

## ApiService (Cliente)
M�todos utilizados en m�dulo RRHH:
- `GetEmployeesAsync()`
- `GetEmployeesPagedAsync(PaginationParams)`
- `GetEmployeesCountAsync()`
- `GetEmployeeAsync(int id)`
- `CreateEmployeeAsync(Employee model)`
- `UpdateEmployeeAsync(int id, Employee model)`
- `DeleteEmployeeAsync(int id)`
- `CheckEmployeeDuplicatesAsync(nationalId, loginId)` (previo a creaci�n para feedback r�pido).
Fallback interno: Si falla paginaci�n servidor, hace paginaci�n cliente.

## Paginaci�n: Contrato de Datos
`PagedResult<Employee>` expuesto por API incluye: `Items`, `TotalCount`, `CurrentPage`, `PageSize`. Cliente calcula `StartIndex`, `EndIndex`, `TotalPages`, y genera controles de navegaci�n (primera, anterior, n�meros din�micos, siguiente, �ltima) bas�ndose en estas propiedades y helpers.

## Diagramas Textuales
Secuencia Creaci�n Empleado:
UI (EmployeesList Modal) -> ApiService.CreateEmployeeAsync -> POST api/employees -> Transacci�n (BusinessEntity + Person + Employee) -> 201 Created -> UI refresca m�tricas/paginaci�n.

Secuencia Listado Paginado:
UI Evento (cambio b�squeda/orden/page) -> Construye `PaginationParams` -> ApiService.GetEmployeesPagedAsync -> GET api/employees/paged -> Response + Headers -> UI renderiza tarjetas empleados.

## Extensibilidad
- Nuevos filtros: ampliar switch de orden y l�gica de b�squeda y agregar par�metros en `PaginationParams` y controlador.
- Nuevas m�tricas: agregar endpoints espec�ficos (ej. activos por g�nero) y tarjetas en `EmployeesList`.
- Auditor�a: integrar `ILogger` y/o middleware para registrar acciones CRUD.

## Riesgos y Limitaciones
- Transacciones ADO.NET manuales requieren sincron�a con cambios en esquema AdventureWorks (fragilidad ante cambios). Puede migrarse a EF con entidades Person para mayor consistencia.
- Fallback de paginaci�n cliente puede impactar rendimiento si el n�mero de empleados crece mucho.
- Validaciones se replican parcialmente entre cliente y servidor (riesgo de divergencia si se modifican en un solo lado).

## Posibles Mejoras Futuras
- Agregar DataAnnotations y FluentValidation para centralizar reglas.
- Implementar caching en conteo y p�ginas frecuentes.
- Incluir autorizaci�n y roles para limitar edici�n/borrado.
- Exportaci�n de listado a CSV/Excel.
- Integrar SignalR para actualizar m�tricas en tiempo real.

## Historial de Cambios
- 2025-10-17 Creaci�n inicial del documento.

---
Fin del documento.
