# Documentación Técnica Módulo Recursos Humanos (RRHH)

## Alcance
Este documento describe la arquitectura técnica del módulo de Recursos Humanos enfocado al manejo de empleados. Cubre componentes de la API (endpoints, modelos, DTOs y lógica principal) y su consumo desde los componentes Blazor `EmployeesList.razor` y `EmployeeDetails.razor`.

## Visión General Arquitectónica
El módulo RRHH implementa un patrón de arquitectura por capas:
- Capa de Datos (API): `AdventureWorksDbContext` accede a tablas AdventureWorks (Person.BusinessEntity, Person.Person, HumanResources.Employee) vía Entity Framework Core y ADO.NET para operaciones de creación compuestas.
- Capa de Servicios (API): `EmpleadoService` (no mostrado aquí) encapsula consultas avanzadas (ej. empleados con más tiempo en departamento) y procedimientos almacenados.
- Capa de Presentación (API): `EmployeesController` expone endpoints RESTful CRUD y consultas específicas.
- Capa Cliente (Blazor): Servicios `ApiService` y componentes de UI (`EmployeesList.razor`, `EmployeeDetails.razor`) consumen la API con manejo de paginación, búsqueda, orden y acciones CRUD.

## Endpoints Principales (EmployeesController)
Base route: `api/employees`
1. `GET api/employees` -> Lista de empleados activos (filtra `CurrentFlag = true`).
2. `GET api/employees/paged?page=1&pageSize=10&search=&sortBy=EmployeeId&sortDirection=asc` -> Devuelve `PagedResult<Employee>` con:
   - Filtros: `search` aplicado a `LoginId`, `JobTitle`, `NationalIdNumber`, `EmployeeId`.
   - Orden dinámico: por campos (`EmployeeId`, `LoginId`, `JobTitle`, `HireDate`, `BirthDate`, `NationalIdNumber`) y dirección asc/desc.
   - Headers de paginación agregados (`X-Pagination-*`).
3. `GET api/employees/count` -> Cantidad total de empleados activos.
4. `GET api/employees/{id}` -> Recupera empleado por ID (valida activo).
5. `POST api/employees` -> Creación compuesta del empleado siguiendo pasos AdventureWorks:
   - Inserta en `Person.BusinessEntity`.
   - Inserta registro mínimo en `Person.Person`.
   - Inserta en `HumanResources.Employee`.
   Usa transacción ADO.NET manual para atomicidad.
6. `PUT api/employees/{id}` -> Actualiza campos editables del empleado (validando id y existencia activa). Maneja concurrencia y errores.
7. `DELETE api/employees/{id}` -> Soft-delete: marca `CurrentFlag = false`.
8. `GET api/employees/mas-tiempo` -> Lista `EmpleadoMasTiempoDto` (consulta de antigüedad por departamento).

## Modelos y DTOs (API)
- `Employee` (Entidad): Campos clave: `EmployeeId (BusinessEntityID)`, `NationalIdNumber`, `LoginId`, `JobTitle`, `BirthDate`, `MaritalStatus`, `Gender`, `HireDate`, `SalariedFlag`, `VacationHours`, `SickLeaveHours`, `CurrentFlag`.
  - Reglas de dominio en creación: longitudes máximas, códigos válidos (`MaritalStatus ? {S,M,D,W}`, `Gender ? {M,F}`), mayor de 18 años, `HireDate` no futura.
  - Duplicidad: no repetir `NationalIdNumber` ni `LoginId`.
- `PagedResult<T>`: Estructura de respuesta paginada (propiedades `Items`, `TotalCount`, `CurrentPage`, `PageSize`, `TotalPages`, helpers `HasNextPage`, `HasPreviousPage`).
- `PaginationParams`: Define parámetros de consulta de paginación y genera query string en cliente.
- `EmpleadoMasTiempoDto`: Proporciona vista de empleados con mayor antigüedad por departamento (`YearsInDepartment`).

## Lógica de Negocio Principal
1. Listado Activo: Sólo empleados con `CurrentFlag = true` disponibles para operaciones.
2. Paginación Servidor: Query LINQ con filtros y orden; cálculo de `TotalCount` y subselección con `Skip/Take`.
3. Creación Empleado: Orquestación manual con transacción para cumplir integridad multi-tabla AdventureWorks (no se usa EF para Person.* aquí, se emplea ADO.NET debido a secuencia y mínima inserción requerida).
4. Actualización Controlada: Solo campos directos del registro `Employee`. No se alteran tablas relacionadas (BusinessEntity, Person) en este flujo.
5. Soft Delete: Preserva histórico y evita borrado físico, alineado con AdventureWorks.
6. Búsqueda Flexible: Coincidencia parcial en varios campos; se normaliza a lower-case.
7. Ordenamiento Dinámico: Switch por nombre de campo, fallback a `EmployeeId`.
8. Consulta Avanzada: `EmpleadoService.ObtenerEmpleadosMasTiempoAsync()` encapsula lógica (posible uso de procedimientos almacenados / T-SQL avanzado) para cálculo de antigüedad por departamento.

## Gestión de Errores y Validaciones
- Creación: Acumula errores en lista y retorna `400 BadRequest` si existen reglas incumplidas; conflicto (`409`) si duplicados.
- Paginación: Encapsulada en try-catch; retorna `500` con mensaje si hay excepción.
- Update: Maneja `DbUpdateConcurrencyException` retornando `404` o `409` según estado.
- Operaciones de baja: Retorna `404` si no encuentra empleado.
- Transacciones: rollback ante excepción en la creación compuesta.

## Seguridad y Consistencia
- Validaciones estrictas de códigos controlados (`Gender`, `MaritalStatus`).
- Evita insertar fechas futuras y menores de edad.
- Soft delete mantiene referencialidad.
- Encabezados de paginación facilitan a cliente la navegación eficiente.

## Consumo en Blazor (EmployeesList.razor)
Responsabilidades:
- Presentar listado paginado, buscador y ordenamiento interactivo.
- Crear, editar y eliminar empleados vía `ApiService`.
- Métricas: total empleados (`GetEmployeesCountAsync`), activos, filtrados por página.
- Optimizaciones UX: debounce de búsqueda con `Timer`, simulación de paginación cliente si servidor falla (`SimulateClientPagination`).
Flujo Simplificado:
1. `OnInitializedAsync` -> Carga conteo y primera página (llamadas `GetEmployeesCountAsync` y `GetEmployeesPagedAsync`).
2. Usuario aplica búsqueda / orden / tamaño página -> Regenera parámetros y solicita de nuevo datos.
3. CRUD:
   - Crear: Validación local de campos, llamada `CreateEmployeeAsync`, refresco de métricas y página.
   - Editar: Clonado de registro, actualización con `UpdateEmployeeAsync`.
   - Eliminar: Soft delete con `DeleteEmployeeAsync`, ajuste de página si quedó vacía.
4. Fallback: Si endpoint paginado falla, se obtiene lista completa y se simula paginación en cliente.

## Consumo en Blazor (EmployeeDetails.razor)
Responsabilidades:
- Mostrar ficha completa del empleado con agrupación semántica (personal, laboral, vacaciones/permisos).
- Calcular métricas derivadas (edad, años servicio, días vacaciones/enfermedad).
- Permitir edición modal reutilizando reglas del controlador (validaciones en servidor al guardar). 
Flujo:
1. `OnParametersSetAsync` -> `GetEmployeeAsync(id)`.
2. Si existe -> set `editEmployee` (clon) para edición modal.
3. Edición -> `UpdateEmployeeAsync`; refresca entidad local.

## ApiService (Cliente)
Métodos utilizados en módulo RRHH:
- `GetEmployeesAsync()`
- `GetEmployeesPagedAsync(PaginationParams)`
- `GetEmployeesCountAsync()`
- `GetEmployeeAsync(int id)`
- `CreateEmployeeAsync(Employee model)`
- `UpdateEmployeeAsync(int id, Employee model)`
- `DeleteEmployeeAsync(int id)`
- `CheckEmployeeDuplicatesAsync(nationalId, loginId)` (previo a creación para feedback rápido).
Fallback interno: Si falla paginación servidor, hace paginación cliente.

## Paginación: Contrato de Datos
`PagedResult<Employee>` expuesto por API incluye: `Items`, `TotalCount`, `CurrentPage`, `PageSize`. Cliente calcula `StartIndex`, `EndIndex`, `TotalPages`, y genera controles de navegación (primera, anterior, números dinámicos, siguiente, última) basándose en estas propiedades y helpers.

## Diagramas Textuales
Secuencia Creación Empleado:
UI (EmployeesList Modal) -> ApiService.CreateEmployeeAsync -> POST api/employees -> Transacción (BusinessEntity + Person + Employee) -> 201 Created -> UI refresca métricas/paginación.

Secuencia Listado Paginado:
UI Evento (cambio búsqueda/orden/page) -> Construye `PaginationParams` -> ApiService.GetEmployeesPagedAsync -> GET api/employees/paged -> Response + Headers -> UI renderiza tarjetas empleados.

## Extensibilidad
- Nuevos filtros: ampliar switch de orden y lógica de búsqueda y agregar parámetros en `PaginationParams` y controlador.
- Nuevas métricas: agregar endpoints específicos (ej. activos por género) y tarjetas en `EmployeesList`.
- Auditoría: integrar `ILogger` y/o middleware para registrar acciones CRUD.

## Riesgos y Limitaciones
- Transacciones ADO.NET manuales requieren sincronía con cambios en esquema AdventureWorks (fragilidad ante cambios). Puede migrarse a EF con entidades Person para mayor consistencia.
- Fallback de paginación cliente puede impactar rendimiento si el número de empleados crece mucho.
- Validaciones se replican parcialmente entre cliente y servidor (riesgo de divergencia si se modifican en un solo lado).

## Posibles Mejoras Futuras
- Agregar DataAnnotations y FluentValidation para centralizar reglas.
- Implementar caching en conteo y páginas frecuentes.
- Incluir autorización y roles para limitar edición/borrado.
- Exportación de listado a CSV/Excel.
- Integrar SignalR para actualizar métricas en tiempo real.

## Historial de Cambios
- 2025-10-17 Creación inicial del documento.

---
Fin del documento.
