# Gu�a de Usuario - M�dulo Recursos Humanos (Empleados)

Esta gu�a explica c�mo usar las pantallas de gesti�n de empleados en la aplicaci�n Blazor.
Incluye: Lista de Empleados y Detalle de Empleado.

---
## 1. Acceso al M�dulo
- Men� / bot�n: Haga clic en "Empleados" o navegue a la ruta: `/Employees`.
- Vista inicial: Se carga autom�ticamente la primera p�gina de empleados activos.

---
## 2. Pantalla: Lista de Empleados (`/Employees`)
### 2.1 Estructura Principal
- Encabezado: T�tulo y descripci�n del m�dulo.
- Barra de herramientas: B�squeda, tama�o de p�gina, botones de acciones (Nuevo, Actualizar).
- Resumen de m�tricas: Total empleados, filtrados, activos y los mostrados en la p�gina actual.
- Tarjetas de empleado: Cada empleado muestra informaci�n b�sica y acciones r�pidas.
- Paginaci�n: Controles para moverse entre p�ginas.

### 2.2 B�squeda
- Campo "Buscar empleados...": Escriba parte del Login, Cargo, ID Nacional o ID num�rico.
- La b�squeda se aplica medio segundo despu�s de dejar de escribir.
- Para limpiar resultados: Bot�n "Limpiar Filtros" (aparece si hay b�squeda activa).

### 2.3 Ordenamiento
- Men� "Ordenar por": Seleccione criterio (ID, Login, Cargo, Fecha de Contrataci�n, N�mero Nacional).
- Bot�n de orden asc/desc: �cono cambia seg�n direcci�n.

### 2.4 Tama�o de P�gina
- Selector de cantidad: 5, 10, 20, 50.
- Cambia cu�ntos empleados se muestran por p�gina.

### 2.5 Paginaci�n
Controles disponibles:
- Primera / �ltima.
- Anterior / Siguiente.
- N�meros de p�gina visibles (se adaptan a su posici�n).
Reglas:
- Botones deshabilitados si no se puede avanzar o retroceder.

### 2.6 Acciones sobre cada empleado
En cada tarjeta (parte superior derecha):
- Ver (�cono ojo): Abre la pantalla de detalles.
- Editar (�cono l�piz): Abre ventana modal con formulario de edici�n.
- Eliminar (�cono basura): Abre confirmaci�n para marcarlo inactivo.

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
   - G�nero (M / F)
   - Fecha de Contrataci�n
   - Tipo de Empleado (Asalariado / Por Horas)
3. Opcional: Horas de Vacaciones, Horas por Enfermedad.
4. Validaciones clave:
   - Mayor de 18 a�os.
   - Fecha de contrataci�n no futura.
   - Campos obligatorios completos.
   - No duplicar ID Nacional ni Login.
5. Clic "Crear Empleado".
6. Si correcto: Mensaje de �xito y se recarga la lista (primer p�gina).
7. Si error: Se muestra texto en recuadro rojo.

### 2.8 Editar Empleado
1. Clic �cono l�piz.
2. Modificar campos permitidos.
3. Clic "Guardar Cambios".
4. Mensaje de �xito si se actualiza. Se cierra modal y se refresca la lista.
5. Errores se muestran en recuadro rojo.

### 2.9 Eliminar Empleado (Marcar Inactivo)
1. Clic �cono basura.
2. Confirmar en la ventana emergente.
3. El empleado se marca inactivo y deja de mostrarse.
4. Si la p�gina queda vac�a y no es la primera, retrocede una p�gina.

### 2.10 Bot�n "Actualizar"
- Limpia b�squeda y refresca m�tricas y p�gina actual (pasa a p�gina 1).

### 2.11 Indicadores y Mensajes
- Cargando: Spinner + texto "Cargando empleados...".
- Lista vac�a (sin resultados o sin datos): Mensaje explicativo y opci�n de limpiar filtros.
- Errores en modales: Recuadro rojo con �cono de advertencia.

---
## 3. Pantalla: Detalle de Empleado (`/Employees/{id}`)
### 3.1 Acceso
- Se abre al pulsar "Ver" en la lista o navegando directamente con la URL que incluye el ID.

### 3.2 Contenido
Secciones agrupadas:
- Informaci�n Personal: ID Nacional, Login, Fecha Nacimiento, Edad, Estado Civil, G�nero.
- Informaci�n Laboral: Cargo, Fecha de Contrataci�n, A�os en la Empresa, Tipo (Asalariado / Por Horas), Estado.
- Vacaciones y Permisos: Horas y d�as equivalentes (vacaciones, enfermedad y total).

### 3.3 Acciones
- Editar: Abre modal similar al de la lista para cambiar campos.
- Volver: Retorna a `/Employees`.

### 3.4 Modal de Edici�n
- Misma l�gica de validaci�n que en la lista.
- Bot�n Guardar actualiza y refresca la vista.

### 3.5 Mensajes
- Cargando: Spinner + texto.
- No encontrado: Mensaje y bot�n para volver.

---
## 4. Reglas y Validaciones Clave
- Fechas futuras prohibidas (contrataci�n, nacimiento/edad m�nima).
- Formatos: C�digos de Estado Civil (S, M, D, W); G�nero (M, F).
- Duplicados controlados en creaci�n (ID Nacional / Login).

---
## 5. Estados y Colores
- Activo: Badge / etiqueta verde.
- Inactivo: Badge / etiqueta roja.

---
## 6. Consejos de Uso
- Para b�squedas precisas use parte del Login o Cargo.
- Ajuste el tama�o de p�gina si necesita ver m�s resultados a la vez.
- Use Actualizar si sospecha que hay cambios recientes.
- Verifique errores en creaci�n/edici�n: suelen indicar reglas de negocio incumplidas.

---
## 7. Preguntas Frecuentes (FAQ)
1. �Por qu� no veo empleados que elimin�? -> Se marcan inactivos y se ocultan autom�ticamente.
2. �Por qu� no puedo crear un empleado menor de edad? -> La pol�tica exige ? 18 a�os.
3. �No encuentro un empleado reci�n creado? -> Presione "Actualizar" para recargar m�tricas y datos.
4. �Qu� significa conflicto de duplicado? -> Existe otro empleado con el mismo ID Nacional o Login.

---
## 8. Accesibilidad B�sica
- Botones tienen �conos y texto para mejor comprensi�n.
- Estados diferenciados por color y etiqueta textual.
- Modales se cierran con bot�n X o acci�n Cancelar.

---
## 9. Errores Comunes
- Campos vac�os obligatorios: Complete todos los marcados con *.
- Fechas inv�lidas: Ajuste la fecha a una no futura y v�lida seg�n la edad requerida.
- Duplicados: Cambie ID Nacional o Login si ya existen.

---
## 10. Flujo Resumido de Tareas
Crear: Lista -> Nuevo Empleado -> Completar -> Crear -> Ver en primera p�gina.
Editar: Lista/Detalle -> Editar -> Guardar -> Refresco.
Eliminar: Lista -> Eliminar -> Confirmar -> Oculto.
Ver Detalle: Lista -> Ver -> Revisar informaci�n -> Volver.

---
Fin de la gu�a.
