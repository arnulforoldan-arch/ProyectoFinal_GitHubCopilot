# Estándares de Programación - ICASA

## 1. Convenciones de Nombres
- **C#:** [Guía oficial](https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/coding-style/identifier-names)
  - PascalCase para clases, métodos y propiedades.
  - camelCase para variables y parámetros.
- **Visual Basic:** [Guía oficial](https://learn.microsoft.com/es-es/dotnet/visual-basic/programming-guide/program-structure/program-structure-and-code-conventions)
  - PascalCase para clases, métodos y propiedades.
  - camelCase para variables y parámetros.
- **JavaScript:**
  - camelCase para variables y funciones.
  - PascalCase para clases.
- **HTML:**
  - Nombres descriptivos y en minúsculas para clases y atributos.
- **SQL:**
  - Nombres en inglés, descriptivos y en mayúsculas para tablas y columnas.
  - Prefijos claros para procedimientos almacenados (ej: `usp_` para procedimientos, `fn_` para funciones).

## 2. Estructura de Archivos
- Un archivo por clase, página o componente.
- Separar código por carpetas según funcionalidad (`Models`, `Services`, `Pages`, `Scripts`, `Views`).
- Usar regiones para separar métodos públicos y privados en C# y VB.
- Mantener archivos menores a 300 líneas cuando sea posible.

## 3. Comentarios y Documentación
- Usar comentarios XML en métodos públicos y clases en C# y VB.
- Explicar lógica compleja con comentarios en línea.
- Documentar endpoints, modelos, servicios y scripts.
- Mantener documentación técnica actualizada.

## 4. Manejo de Errores
- Usar try-catch en operaciones críticas y acceso a datos (C#, VB, JavaScript).
- Registrar errores en el log corporativo.
- No mostrar detalles técnicos al usuario final.
- Implementar páginas de error personalizadas en aplicaciones web.

## 5. Seguridad
- Validar y sanear todos los datos de entrada (evitar XSS, CSRF, SQL Injection).
- Usar DataAnnotations para validación en modelos (C#, VB).
- Implementar protección CSRF en formularios web.
- No exponer información sensible en mensajes de error ni en la interfaz.
- Usar HTTPS en todos los entornos.
- Gestionar secretos y credenciales con herramientas seguras (Azure Key Vault, Secret Manager).
- Limitar permisos y roles en la aplicación.
- Auditar y registrar accesos y acciones sensibles.

## 6. Mejores Prácticas de Desarrollo
- Seguir principios SOLID y patrones de diseño.
- Usar inyección de dependencias para servicios.
- Escribir pruebas unitarias y de integración.
- Revisar código mediante Pull Requests y análisis estático.
- Mantener dependencias actualizadas y seguras.
- Evitar código duplicado y refactorizar cuando sea necesario.
- Usar herramientas de análisis de seguridad (SonarQube, GitHub Advanced Security).
- Separar lógica de negocio, presentación y acceso a datos.

## 7. Aplicaciones Windows Forms
- Separar lógica de negocio de la interfaz gráfica.
- Usar eventos y controladores bien definidos.
- Validar datos en el lado del cliente y servidor.
- Manejar excepciones y mostrar mensajes amigables.
- No almacenar información sensible en el cliente.

## 8. Aplicaciones Web Microsoft (Razor Pages, MVC, Web API)
- Seguir las mejores prácticas de seguridad web.
- Usar modelos fuertemente tipados y validación.
- Implementar autenticación y autorización robusta.
- Proteger contra ataques comunes (XSS, CSRF, SQL Injection).
- Usar layouts y vistas parciales para reutilización.

## 9. Ejemplo de Clase C# (Razor Page Segura)

## 10. Referencias
- [Guía de seguridad en ASP.NET Core](https://learn.microsoft.com/es-es/aspnet/core/security/)
- [Mejores prácticas en Razor Pages](https://learn.microsoft.com/es-es/aspnet/core/razor-pages/?view=aspnetcore-8.0)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Convenciones de nombres en SQL Server](https://learn.microsoft.com/es-es/sql/relational-databases/sql-server-object-naming-rules)

---

Este documento debe revisarse y actualizarse periódicamente para adaptarse a nuevas amenazas y tecnologías.