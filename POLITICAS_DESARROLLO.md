# Est�ndares de Programaci�n - ICASA

## 1. Convenciones de Nombres
- **C#:** [Gu�a oficial](https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/coding-style/identifier-names)
  - PascalCase para clases, m�todos y propiedades.
  - camelCase para variables y par�metros.
- **Visual Basic:** [Gu�a oficial](https://learn.microsoft.com/es-es/dotnet/visual-basic/programming-guide/program-structure/program-structure-and-code-conventions)
  - PascalCase para clases, m�todos y propiedades.
  - camelCase para variables y par�metros.
- **JavaScript:**
  - camelCase para variables y funciones.
  - PascalCase para clases.
- **HTML:**
  - Nombres descriptivos y en min�sculas para clases y atributos.
- **SQL:**
  - Nombres en ingl�s, descriptivos y en may�sculas para tablas y columnas.
  - Prefijos claros para procedimientos almacenados (ej: `usp_` para procedimientos, `fn_` para funciones).

## 2. Estructura de Archivos
- Un archivo por clase, p�gina o componente.
- Separar c�digo por carpetas seg�n funcionalidad (`Models`, `Services`, `Pages`, `Scripts`, `Views`).
- Usar regiones para separar m�todos p�blicos y privados en C# y VB.
- Mantener archivos menores a 300 l�neas cuando sea posible.

## 3. Comentarios y Documentaci�n
- Usar comentarios XML en m�todos p�blicos y clases en C# y VB.
- Explicar l�gica compleja con comentarios en l�nea.
- Documentar endpoints, modelos, servicios y scripts.
- Mantener documentaci�n t�cnica actualizada.

## 4. Manejo de Errores
- Usar try-catch en operaciones cr�ticas y acceso a datos (C#, VB, JavaScript).
- Registrar errores en el log corporativo.
- No mostrar detalles t�cnicos al usuario final.
- Implementar p�ginas de error personalizadas en aplicaciones web.

## 5. Seguridad
- Validar y sanear todos los datos de entrada (evitar XSS, CSRF, SQL Injection).
- Usar DataAnnotations para validaci�n en modelos (C#, VB).
- Implementar protecci�n CSRF en formularios web.
- No exponer informaci�n sensible en mensajes de error ni en la interfaz.
- Usar HTTPS en todos los entornos.
- Gestionar secretos y credenciales con herramientas seguras (Azure Key Vault, Secret Manager).
- Limitar permisos y roles en la aplicaci�n.
- Auditar y registrar accesos y acciones sensibles.

## 6. Mejores Pr�cticas de Desarrollo
- Seguir principios SOLID y patrones de dise�o.
- Usar inyecci�n de dependencias para servicios.
- Escribir pruebas unitarias y de integraci�n.
- Revisar c�digo mediante Pull Requests y an�lisis est�tico.
- Mantener dependencias actualizadas y seguras.
- Evitar c�digo duplicado y refactorizar cuando sea necesario.
- Usar herramientas de an�lisis de seguridad (SonarQube, GitHub Advanced Security).
- Separar l�gica de negocio, presentaci�n y acceso a datos.

## 7. Aplicaciones Windows Forms
- Separar l�gica de negocio de la interfaz gr�fica.
- Usar eventos y controladores bien definidos.
- Validar datos en el lado del cliente y servidor.
- Manejar excepciones y mostrar mensajes amigables.
- No almacenar informaci�n sensible en el cliente.

## 8. Aplicaciones Web Microsoft (Razor Pages, MVC, Web API)
- Seguir las mejores pr�cticas de seguridad web.
- Usar modelos fuertemente tipados y validaci�n.
- Implementar autenticaci�n y autorizaci�n robusta.
- Proteger contra ataques comunes (XSS, CSRF, SQL Injection).
- Usar layouts y vistas parciales para reutilizaci�n.

## 9. Ejemplo de Clase C# (Razor Page Segura)

## 10. Referencias
- [Gu�a de seguridad en ASP.NET Core](https://learn.microsoft.com/es-es/aspnet/core/security/)
- [Mejores pr�cticas en Razor Pages](https://learn.microsoft.com/es-es/aspnet/core/razor-pages/?view=aspnetcore-8.0)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Convenciones de nombres en SQL Server](https://learn.microsoft.com/es-es/sql/relational-databases/sql-server-object-naming-rules)

---

Este documento debe revisarse y actualizarse peri�dicamente para adaptarse a nuevas amenazas y tecnolog�as.