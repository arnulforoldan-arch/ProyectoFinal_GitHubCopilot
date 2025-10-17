##### **PROMPS PRINCIPALES UTILIZADOS POR FASES**



**Fase 2: Construcción del Backend**



* Necesito que analices y memorices el contexto de las siguientes tablas que vamos a trabajar en la solucion SistemasGestionEmpresarial, primero te dare la estructura de tablas SQL que usare dentro del esquema HumanResources de la base de datos AdventureWorks2014, y luego te dare los scripts sql de las tablas que utilizare en el esquema Production y por ultimo los scripts sql de las tablas del esquema Sales. (seguido le comparti a Copilot los scripts de las estructuras de las tablas que se usuaron)



* @workspace Necesito que prepares el proyecto AdventureWorks.Enterprise.Api para usar Entity Framework Core con SQL Server, modifica el archivo .csproj para agregar las últimas versiones estables de los paquetes NuGet 'Microsoft.EntityFrameworkCore.SqlServer' y 'Microsoft.EntityFrameworkCore.Tools'.  



* **Ingeniería de Contexto (BBDD):**

1\. Dentro del proyecto AdventureWorks.Enterprise.Api crea crea una nueva carpeta llamada Data.

2\. Dentro de Data, crea una clase por cada tabla seleccionada de los esquemas HumanResources, Production y Sales. 

3\. Mapea las columnas de cada tabla a propiedades y usa nombres en PascalCase y los tipos de datos de .NET correctos. 

4\. Para la clase Employee.cs asigna la clave primaria BusinessEntityID, para la clase Product.cs la clave primaria ProductID y para la clase SalesOrder.cs la clave primaria SalesOrderID.

5\. Dentro de la carpeta Data crea una clase llamada AdventureWorksDbContext.cs que herede de DbContext, esta debe de incluir un constructor que acepte DbContextOptions y un DbSet<Employee> llamado Employees, otro DbSet<SalesOrder> llamado SalesOrders y otro DbSet<Product> llamado Products.



* Necesito que modifiques mi archivo 'appsettings.json' para agregar una cadena de conexión llamada 'DefaultConnection'. Debe apuntar a la base de datos 'AdventureWorks2014' en mi servidor local SQL tiene que usar Integrated Security, luego tienes que configurar la inyección de dependencias en Program.cs. Registra el AdventureWorksDbContext para que use SQL Server con la cadena de conexión 'DefaultConnection'.



* **Scaffolding Inteligente de Controladores EmployeesController, OrdersController y ProductsController**

1\. Necesito que generes los controladores siguientes: EmployeesController con sus operaciones CRUD para empleados, otro controlador con nombre OrdersController y otro controlador ProductsController, crea los controladores en la carpeta Crontrollers.  

Estos controladores deben de usar nuestro 'AdventureWorksDbContext' a través de inyección de dependencias e implementar métodos CRUD completos y asíncronos para cada entidad, siguiendo las convenciones RESTful y manejando casos de error como NotFound.

2\. Implementa en el contrador OrdersController operaciones para la gestión de órdenes de venta

3\. Implementa en el contrador ProductsController operaciones para la gestión de productos e inventario



* **Consultas SQL Avanzadas para generación de Reportes**



1. De acuerdo a la estructura de tablas del esquema HumanResources de la base de datos AdventureWorks2014, necesito que generes una consulta T-SQL avanzada que obtenga los Empleados con más tiempo en su departamento actual.  Necesito tambien que utilices las tablas del esquema Production y generes una consulta T-SQL avanzada que genere el topo 10 de productos más vendidos y por ultimo utiliza las tablas del esquema Sales y generes una consulta T-SQL avanzada que genere Un listado de productos con bajo inventario (ej. cantidad menor a 10).



2\. Convierte cada consulta generada en un procedimiento almacenado.

3\. De acuerdo al contexto de las tablas utilizadas en cada procedimiento almacenado integralos en sus respectivos controladores del proyecto AdventureWorks.Enterprise.Api, los controladores son #file:'EmployeesController.cs' , #file:'OrdersController.cs' y #file:'ProductsController.cs' , utiliza DTOs y ajusta los datos segun lo que devuelva cada cada procedimiento almacenado.  Crea los DTOs en la carpeta Models.





**Fase 3: Desarrollo del Frontend** 



* @workspace Voy a construir una interfaz de usuario web con Blazor Server en el proyecto 'AdventureWorks.Enterprise.WebApp'. Mi API backend está en el proyecto 'AdventureWorks.Enterpirse.Api' dentro de esta misma solución. Encuentra y analiza los archivos #file:'EmployeesController.cs' , #file:'OrdersController.cs'  y #file:'ProductsController.cs' y úsalos como las fuentes de verdad para entender las rutas, métodos y modelos que debo consumir.

&nbsp;  Necesito que me confirmes cuando lo hayas analizado.



* Basándote en el contexto de los controladores  #file:'EmployeesController.cs' , #file:'OrdersController.cs'  y #file:'ProductsController.cs' , crea una nueva carpeta Models en el proyecto 'AdventureWorks.Enterprise.WebApp' y dentro de ella, crea una clase C# Employee.cs, SalesOrder.cs y Product.cs. Cada clase debe tener exactamente las mismas propiedades públicas que los modelos Employee, SalesOrder y Product utilizados en la API para asegurar una deserialización JSON correcta.



* En el proyecto 'AdventureWorks.Enterprise.WebApp', crea una nueva carpeta Services y dentro, un archivo ApiService.cs. Esta clase debe tener:

1\. Un constructor que inyecte HttpClient.

2\. Contener un método público asíncrono por cada API expuesta en cada uno de los controladores  #file:'EmployeesController.cs' , #file:'OrdersController.cs'  y #file:'ProductsController.cs' por ejemplo si existe una API para obtener productos debes crear el método publico asíncrono GetProductsAsync que devuelva una Task<List<Product>>, debes de seguir el mismo patron para crear cada método por cada API dentro de los controladores.

3\. Dentro de cada método creado, debes realizar una llamada segun su tipo ya sea GET, POST, PUT o DELETE a la ruta correcta para obtener cada tipo de información y deserializar la respuesta JSON.

4\. Agrega un manejo de errores en cada método creado.



* Modifica el archivo #file:'AdventureWorks.Enterprise.WebApp\\Program.cs'   registra nuestro ApiService y configurar su HttpClient. La configuración debe:

1\. Leer la sección "ApiKeySettings" desde appsettings.json.

2\. Establecer la dirección base del cliente (BaseAddress) apuntando a cada API.

3\. Añadir un encabezado de solicitud por defecto (DefaultRequestHeaders) llamado "X-API-Key" con el valor de la clave

leída desde la configuración.



* **Portal de RRHH**

En el proyecto 'AdventureWorks.Enterprise.WebApp', crea un nuevo componente de Blazor en la carpeta Pages llamado EmployeesList.razor. Este componente debe:

1\. Tener la ruta de página @page "/employeeslist".

2\. Inyectar nuestro ApiService.

3\. Mostrar un título <h1>Lista de Empleados</h1>.

4\. Durante la inicialización (OnInitializedAsync), llamar a GetEmployeesAsync.

5\. Mientras carga, mostrar un mensaje <p>Cargando lista de empleados...</p>.

6\. Una vez cargados, renderizar una tabla HTML que muestre: employeeId, loginId, jobTitle.



Crea un componente Blazor en la carpeta Pages llamado EmployeeDetails.razor. Debe:

1\. Aceptar un parámetro de ruta de tipo entero llamado 'id'. La ruta debe ser "/Employees/{id:int}".

2\. Inyectar ApiService.

3\. En OnParametersSetAsync, usar el 'id' del parámetro para llamar a GetEmployeeAsync y obtener los detalles del empleado.

4\. Mostrar "Cargando..." mientras se espera.

5\. Si el empleado se encuentra, mostrar sus propiedades (employeeId,nationalIdNumber, loginId, jobTitle, etc.) en una lista descriptiva.



* Necesito que me ayudes a implementar la funcionalidad de agregar un empleado nuevo  en el boton Nuevo Empleado del archivo #file:'EmployeesList.razor'  , debes de Inyectar ApiService, por lo que analiza los campos necesarios que utiliza el servicio POST para crear un empleado, recuerda mantener el mismo diseño del sitio.



* **Portal de Ventas**

Crea los siguientes componentes Blazor en la carpeta Pages llamados OrdersList.razor y OrderDetails.razor, necesito crear una vista maestro detalle de ordenes de venta, por lo que debes Inyectar ApiService y asignar la vista en una pagina nueva de Ordenes de Venta y debe de ser llamada desde el boton Pedidos del Portal de Ventas en la página principal.



* Necesito que recuerdes todos los pasos que se implementaron para agregar paginacion a la lista de empleados porque ahora necesito que agregues paginación al listado de ordenes de venta, agrega los servicios que sean necesarios y no olvides aplicar la solucion que se implemento para que funcionaran los controles de paginación.



* Necesito que me ayudes a implementar la funcionalidad de agregar una nueva orden de venta el boton Nueva Orden del archivo #file:'OrdersList.razor'  , debes de Inyectar ApiService, por lo que analiza los campos necesarios que utiliza el servicio POST para crear ordenes, recuerda mantener el mismo diseño del sitio.  Realiza los mismos pasos y acciones realizadas que se implementaron para crear un nuevo empleado.



* **Portal de Producción**



En el proyecto 'AdventureWorks.Enterprise.WebApp', crea un nuevo componente de Blazor en la carpeta Pages llamado ProductList.razor. Este componente debe:

1\. Tener la ruta de página @page "/productlist".

2\. Inyectar nuestro ApiService.

3\. Mostrar un título <h1>Inventario de Productos</h1>.

4\. Durante la inicialización (OnInitializedAsync), llamar a GetProductInventoryAsync.

5\. Mientras carga, mostrar un mensaje <p>Cargando inventario de productos...</p>.

6\. Debes de cargar los datos principales que informen el nivel del inventario.

7\. Asigna la pagina nueva al boton Inventario de la tarjeta Portal de Producción en la pagina de inicio de la aplicación.

8\. Tambien asigna la pagina nueva al menu superior desplegable en la pestaña Producción opción Inventario.

9\. Implementa la paginación de la informacion del inventario de productos de acuerdo a como se implemento en la lista de empleados y la lista de ordenes de venta.



En el proyecto 'AdventureWorks.Enterprise.WebApp', crea un nuevo componente de Blazor en la carpeta Pages llamado LowInventoryReport.razor. Este componente debe:

1\. Tener la ruta de página @page "/lowinventoryreport".

2\. Inyectar nuestro ApiService.

3\. Mostrar un título <h1>Reporte de Productos con Bajo Stock</h1>.

4\. Durante la inicialización (OnInitializedAsync), llamar a GetProductLowInventoryAsync.

5\. Mientras carga, mostrar un mensaje <p>Cargando inventario de productos con bajo stock...</p>.

6\. Debes de cargar los datos principales que informen el nivel de bajo inventario.

7\. Asigna la pagina nueva al boton Producción de la tarjeta Portal de Producción en la pagina de inicio de la aplicación, pero cambia el nombre del boton a Reporte Bajo Stock.

8\. Tambien asigna la pagina nueva al menu superior desplegable en la pestaña Producción opción Producción y tambien cambia el nombre de la opcion a Reporte Bajo Stock.

9\. Implementa la paginacion, diseño y opciones de filtro, exportar a formatos csv/excel e impresora de acuerdo al reporte de inventario archivo ProductList.razor







**Fase 4: Garantía de Calidad y Depuración**



* **Pruebas Unitarias Conformes**

@workspace Necesito que me ayudes a generar todos los casos de éxito y de fallo del controlador #file:'EmployeesController.cs'  y crea un archivo de tests xUnit en el proyecto AdventureWorks.Enterprise.Api.Tests con nombre EmployeesControllerTests.cs



@workspace Necesito que me ayudes a generar todos los casos de éxito y de fallo del controlador #file:'OrdersController.cs'  y crea un archivo de tests xUnit en el proyecto AdventureWorks.Enterprise.Api.Tests con nombre OrdersControllerTests.cs



@workspace Necesito que me ayudes a generar todos los casos de éxito y de fallo del controlador #file:'ProductsController.cs'  y crea un archivo de tests xUnit en el proyecto AdventureWorks.Enterprise.Api.Tests con nombre ProductsControllerTests.cs







