# Programming Standards - ICASA

## 1. Naming Conventions
- **C#:** [Official Guide](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
  - PascalCase for classes, methods, and properties.
  - camelCase for variables and parameters.
- **Visual Basic:** [Official Guide](https://learn.microsoft.com/en-us/dotnet/visual-basic/programming-guide/program-structure/program-structure-and-code-conventions)
  - PascalCase for classes, methods, and properties.
  - camelCase for variables and parameters.
- **JavaScript:**
  - camelCase for variables and functions.
  - PascalCase for classes.
- **HTML:**
  - Descriptive and lowercase names for classes and attributes.
- **CSS:**
  - Use lowercase and hyphen-separated names for classes and IDs (e.g., .main-header).
  - Use descriptive names for styles.
- **Python:**
  - snake_case for variables and functions.
  - PascalCase for classes.
- **SQL:**
  - English, descriptive, and uppercase names for tables and columns.
  - Clear prefixes for stored procedures (e.g., usp_ for procedures, fn_ for functions).

## 2. File Structure
- One file per class, page, or component.
- Separate code into folders by functionality (Models, Services, Pages, Scripts, Views, Styles).
- Use regions or comments to separate public and private methods/functions.
- Keep files under 300 lines when possible.

## 3. Comments and Documentation
- Use XML comments for public methods and classes in C# and VB.
- Use docstrings for Python functions and classes.
- Use JSDoc for JavaScript functions and classes.
- Explain complex logic with inline comments in all languages.
- Document endpoints, models, services, and scripts.
- Keep technical documentation up to date.

## 4. Error Handling
- Use try-catch/try-except in critical operations and data access (C#, VB, JavaScript, Python).
- Log errors in the corporate log or appropriate logging system.
- Do not show technical details to the end user.
- Implement custom error pages or messages in web applications.

## 5. Security
- Validate and sanitize all input data (prevent XSS, CSRF, SQL Injection).
- Use DataAnnotations for model validation (C#, VB).
- Use built-in validation libraries for Python and JavaScript.
- Implement CSRF protection in web forms.
- Do not expose sensitive information in error messages or the UI.
- Manage secrets and credentials with secure tools (Azure Key Vault, Secret Manager, environment variables).
- Limit permissions and roles in the application.
- Audit and log sensitive accesses and actions.

## 6. Development Best Practices
- Follow SOLID principles and design patterns (where applicable).
- Use dependency injection for services (C#, Python, JavaScript frameworks).
- Write unit and integration tests.
- Review code via Pull Requests and static analysis.
- Keep dependencies updated and secure.
- Avoid duplicate code and refactor when necessary.
- Use security analysis tools (SonarQube, GitHub Advanced Security, Bandit for Python).
- Separate business logic, presentation, and data access.

## 7. Windows Forms Applications
- Separate business logic from the UI.
- Use well-defined events and handlers.
- Validate data on both client and server sides.
- Handle exceptions and show friendly messages.
- Do not store sensitive information on the client.

## 8. Web Applications (Razor Pages, MVC, Web API, HTML, CSS, JS, Python frameworks)
- Follow web security best practices.
- Use strongly typed models and validation where possible.
- Implement robust authentication and authorization.
- Protect against common attacks (XSS, CSRF, SQL Injection).
- Use layouts, partial views, and reusable components.
- Use semantic HTML and accessible CSS.

## 9. Example of Class or Component (see language-specific documentation)

## 10. References
- [ASP.NET Core Security Guide](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Best Practices in Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [SQL Server Naming Conventions](https://learn.microsoft.com/en-us/sql/relational-databases/sql-server-object-naming-rules)
- [PEP8 Python Style Guide](https://peps.python.org/pep-0008/)
- [MDN HTML/CSS/JS Guidelines](https://developer.mozilla.org/en-US/docs/Web)

---
This document should be reviewed and updated periodically to adapt to new threats, technologies, and languages.
