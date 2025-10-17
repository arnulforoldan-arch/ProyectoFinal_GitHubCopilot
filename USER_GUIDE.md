# Guía de Usuario - Módulo Recursos Humanos (Empleados)

Esta guía explica cómo usar las pantallas de gestión de empleados en la aplicación Blazor.
Incluye: Lista de Empleados y Detalle de Empleado.

---
## 1. Acceso al Módulo
- Menú / botón: Haga clic en "Empleados" o navegue a la ruta: `/Employees`.
- Vista inicial: Se carga automáticamente la primera página de empleados activos.

---
## 2. Pantalla: Lista de Empleados (`/Employees`)
### 2.1 Estructura Principal
- Encabezado: Título y descripción del módulo.
- Barra de herramientas: Búsqueda, tamaño de página, botones de acciones (Nuevo, Actualizar).
- Resumen de métricas: Total empleados, filtrados, activos y los mostrados en la página actual.
- Tarjetas de empleado: Cada empleado muestra información básica y acciones rápidas.
- Paginación: Controles para moverse entre páginas.

### 2.2 Búsqueda
- Campo "Buscar empleados...": Escriba parte del Login, Cargo, ID Nacional o ID numérico.
- La búsqueda se aplica medio segundo después de dejar de escribir.
- Para limpiar resultados: Botón "Limpiar Filtros" (aparece si hay búsqueda activa).

### 2.3 Ordenamiento
- Menú "Ordenar por": Seleccione criterio (ID, Login, Cargo, Fecha de Contratación, Número Nacional).
- Botón de orden asc/desc: Ícono cambia según dirección.

### 2.4 Tamaño de Página
- Selector de cantidad: 5, 10, 20, 50.
- Cambia cuántos empleados se muestran por página.

### 2.5 Paginación
Controles disponibles:
- Primera / Última.
- Anterior / Siguiente.
- Números de página visibles (se adaptan a su posición).
Reglas:
- Botones deshabilitados si no se puede avanzar o retroceder.

### 2.6 Acciones sobre cada empleado
En cada tarjeta (parte superior derecha):
- Ver (ícono ojo): Abre la pantalla de detalles.
- Editar (ícono lápiz): Abre ventana modal con formulario de edición.
- Eliminar (ícono basura): Abre confirmación para marcarlo inactivo.

Estados:
- Activo: Etiqueta verde "Activo".
- Inactivo: Etiqueta roja "Inactivo" (no aparece en la lista excepto si estaba antes de eliminarse de su vista).

### 2.7 Crear Nuevo Empleado
1. Clic en "Nuevo Empleado".
2. Completar campos obligatorios (marcados con *):
   - ID Nacional
   - Login ID
   - Cargo
   - Fecha de Nacimiento
   - Estado Civil (S, M, D, W)
   - Género (M / F)
   - Fecha de Contratación
   - Tipo de Empleado (Asalariado / Por Horas)
3. Opcional: Horas de Vacaciones, Horas por Enfermedad.
4. Validaciones clave:
   - Mayor de 18 años.
   - Fecha de contratación no futura.
   - Campos obligatorios completos.
   - No duplicar ID Nacional ni Login.
5. Clic "Crear Empleado".
6. Si correcto: Mensaje de éxito y se recarga la lista (primer página).
7. Si error: Se muestra texto en recuadro rojo.

### 2.8 Editar Empleado
1. Clic ícono lápiz.
2. Modificar campos permitidos.
3. Clic "Guardar Cambios".
4. Mensaje de éxito si se actualiza. Se cierra modal y se refresca la lista.
5. Errores se muestran en recuadro rojo.

### 2.9 Eliminar Empleado (Marcar Inactivo)
1. Clic ícono basura.
2. Confirmar en la ventana emergente.
3. El empleado se marca inactivo y deja de mostrarse.
4. Si la página queda vacía y no es la primera, retrocede una página.

### 2.10 Botón "Actualizar"
- Limpia búsqueda y refresca métricas y página actual (pasa a página 1).

### 2.11 Indicadores y Mensajes
- Cargando: Spinner + texto "Cargando empleados...".
- Lista vacía (sin resultados o sin datos): Mensaje explicativo y opción de limpiar filtros.
- Errores en modales: Recuadro rojo con ícono de advertencia.

---
## 3. Pantalla: Detalle de Empleado (`/Employees/{id}`)
### 3.1 Acceso
- Se abre al pulsar "Ver" en la lista o navegando directamente con la URL que incluye el ID.

### 3.2 Contenido
Secciones agrupadas:
- Información Personal: ID Nacional, Login, Fecha Nacimiento, Edad, Estado Civil, Género.
- Información Laboral: Cargo, Fecha de Contratación, Años en la Empresa, Tipo (Asalariado / Por Horas), Estado.
- Vacaciones y Permisos: Horas y días equivalentes (vacaciones, enfermedad y total).

### 3.3 Acciones
- Editar: Abre modal similar al de la lista para cambiar campos.
- Volver: Retorna a `/Employees`.

### 3.4 Modal de Edición
- Misma lógica de validación que en la lista.
- Botón Guardar actualiza y refresca la vista.

### 3.5 Mensajes
- Cargando: Spinner + texto.
- No encontrado: Mensaje y botón para volver.

---
## 4. Reglas y Validaciones Clave
- Fechas futuras prohibidas (contratación, nacimiento/edad mínima).
- Formatos: Códigos de Estado Civil (S, M, D, W); Género (M, F).
- Duplicados controlados en creación (ID Nacional / Login).

---
## 5. Estados y Colores
- Activo: Badge / etiqueta verde.
- Inactivo: Badge / etiqueta roja.

---
## 6. Consejos de Uso
- Para búsquedas precisas use parte del Login o Cargo.
- Ajuste el tamaño de página si necesita ver más resultados a la vez.
- Use Actualizar si sospecha que hay cambios recientes.
- Verifique errores en creación/edición: suelen indicar reglas de negocio incumplidas.

---
## 7. Preguntas Frecuentes (FAQ)
1. ¿Por qué no veo empleados que eliminé? -> Se marcan inactivos y se ocultan automáticamente.
2. ¿Por qué no puedo crear un empleado menor de edad? -> La política exige ? 18 años.
3. ¿No encuentro un empleado recién creado? -> Presione "Actualizar" para recargar métricas y datos.
4. ¿Qué significa conflicto de duplicado? -> Existe otro empleado con el mismo ID Nacional o Login.

---
## 8. Accesibilidad Básica
- Botones tienen íconos y texto para mejor comprensión.
- Estados diferenciados por color y etiqueta textual.
- Modales se cierran con botón X o acción Cancelar.

---
## 9. Errores Comunes
- Campos vacíos obligatorios: Complete todos los marcados con *.
- Fechas inválidas: Ajuste la fecha a una no futura y válida según la edad requerida.
- Duplicados: Cambie ID Nacional o Login si ya existen.

---
## 10. Flujo Resumido de Tareas
Crear: Lista -> Nuevo Empleado -> Completar -> Crear -> Ver en primera página.
Editar: Lista/Detalle -> Editar -> Guardar -> Refresco.
Eliminar: Lista -> Eliminar -> Confirmar -> Oculto.
Ver Detalle: Lista -> Ver -> Revisar información -> Volver.

---
Fin de la guía.
