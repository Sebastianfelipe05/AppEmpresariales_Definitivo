# 🚀 GUÍA RÁPIDA DE EJECUCIÓN DEL SISTEMA

## Guía paso a paso para ejecutar todo el sistema completo

---

## 📋 REQUISITOS PREVIOS

Antes de comenzar, asegúrate de tener instalado:

- ✅ **Java 17** (OpenJDK)
- ✅ **Node.js 18+** y npm
- ✅ **.NET 8.0 SDK**
- ✅ **Oracle Database 21c XE**

---

## 🗄️ PASO 1: INICIAR BASE DE DATOS ORACLE

### 1.1 Verificar que Oracle esté corriendo

Abrir **Command Prompt** (CMD) como Administrador:

```bash
# Verificar servicio Oracle
lsnrctl status
```

✅ **Debe mostrar:** `The listener supports no services` o servicios `XE` y `XEPDB1`

Si no está corriendo:
```bash
lsnrctl start
```

---

### 1.2 Conectarse a Oracle y verificar tablas

```bash
# Conectarse como DAE2025
sqlplus DAE2025/DAE2025@localhost:1521/xepdb1
```

Una vez dentro de SQL*Plus:

```sql
-- Ver las tablas
SELECT table_name FROM user_tables;

-- Debe mostrar:
-- CARRO
-- MANTENIMIENTO

-- Ver datos de prueba
SELECT * FROM CARRO;
SELECT * FROM MANTENIMIENTO;

-- Salir
EXIT;
```

✅ **Si las tablas existen y tienen datos, puedes continuar.**

❌ **Si NO existen las tablas, ejecutar:**

```bash
# Ejecutar script de creación
sqlplus DAE2025/DAE2025@localhost:1521/xepdb1
@C:\src\EmpresarialesProyecto\EmpresarialesBackend\CREAR_BD_ORACLE.sql
EXIT;
```

---

## 🖥️ PASO 2: INICIAR BACKEND (Spring Boot)

### 2.1 Abrir una nueva ventana de CMD

**Windows:** Presiona `Win + R` → escribe `cmd` → Enter

### 2.2 Navegar al directorio del backend

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesBackend
```

### 2.3 Configurar JAVA_HOME (si es necesario)

```bash
set JAVA_HOME=C:\Users\TU_USUARIO\.jdks\ms-17.0.16
```

💡 **Nota:** Reemplaza `TU_USUARIO` con tu nombre de usuario de Windows.

### 2.4 Ejecutar el backend

```bash
mvnw.cmd spring-boot:run
```

⏳ **Espera a que aparezca:**

```
Started EmpresarialesProyecto in X.XXX seconds
Tomcat started on port 8080 (http)
```

✅ **El backend está corriendo en:** `http://localhost:8080`

### 2.5 Verificar que funciona (opcional)

Abrir otra ventana CMD y ejecutar:

```bash
curl -u admin:admin http://localhost:8080/api/carro/healthCheck
```

Debe responder: `"Carros API Status Ok!"`

---

### ⚠️ SOLUCIÓN DE PROBLEMAS COMUNES

#### Error: Puerto 8080 ya está en uso

```bash
# Matar el proceso que usa el puerto 8080
netstat -ano | findstr :8080
taskkill /F /PID <numero_que_aparece>
```

O directamente:
```bash
for /f "tokens=5" %a in ('netstat -aon ^| find ":8080" ^| find "LISTENING"') do taskkill /F /PID %a
```

#### Error: Memoria insuficiente

Ya está corregido en `.mvn\jvm.config`, pero si persiste, reinicia el CMD.

---

## 🌐 PASO 3: INICIAR FRONTEND REACT

### 3.1 Abrir OTRA ventana de CMD (nueva)

**Importante:** No cierres la ventana del backend.

### 3.2 Navegar al directorio de React

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesCliente
```

### 3.3 Instalar dependencias (solo primera vez)

```bash
npm install
```

⏳ **Espera 1-2 minutos** mientras se descargan las dependencias.

### 3.4 Ejecutar React en modo desarrollo

```bash
npm run dev
```

⏳ **Espera a que aparezca:**

```
VITE v5.x.x  ready in XXX ms

➜  Local:   http://localhost:5173/
➜  Network: use --host to expose
```

✅ **El frontend React está corriendo en:** `http://localhost:5173`

### 3.5 Abrir en el navegador

1. Abre tu navegador web (Chrome, Edge, Firefox)
2. Ve a: **http://localhost:5173**
3. Deberías ver la página principal del sistema

---

## 🖥️ PASO 4: EJECUTAR CLIENTE C# (Windows Forms)

Tienes **2 opciones**: Visual Studio o línea de comandos.

---

### OPCIÓN A: Usando Visual Studio 2022 (Recomendado)

#### 4.1 Abrir Visual Studio 2022

1. Inicia **Visual Studio 2022**
2. Clic en **"Abrir un proyecto o solución"**
3. Navegar a: `C:\src\EmpresarialesProyecto\EmpresarialesClienteCSharp`
4. Seleccionar: **`EmpresarialesClienteCSharp.csproj`**
5. Hacer doble clic o dar **Abrir**

#### 4.2 Esperar a que se restauren los paquetes NuGet

En la parte inferior, espera a ver:
```
Restauración completada
Listo
```

#### 4.3 Ejecutar la aplicación

**Método 1:** Presionar la tecla **`F5`**

**Método 2:** Clic en el botón verde ▶️ **"EmpresarialesClienteCSharp"** en la barra superior

✅ **Se abrirá una ventana de Windows Forms** con el menú principal.

---

### OPCIÓN B: Usando línea de comandos (CMD)

#### 4.1 Abrir OTRA ventana de CMD (nueva)

### 4.2 Navegar al directorio de C#

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesClienteCSharp
```

#### 4.3 Restaurar paquetes NuGet

```bash
dotnet restore
```

#### 4.4 Compilar y ejecutar

```bash
dotnet run
```

✅ **Se abrirá la aplicación de Windows Forms.**

---

## 📊 RESUMEN: TODO CORRIENDO

Ahora deberías tener **4 cosas activas**:

| Componente | Estado | Puerto/Ubicación |
|------------|--------|------------------|
| 🗄️ **Oracle Database** | ✅ Corriendo | Puerto 1521 |
| 🖥️ **Backend Spring Boot** | ✅ Corriendo | http://localhost:8080 |
| 🌐 **Frontend React** | ✅ Corriendo | http://localhost:5173 |
| 🖼️ **Cliente C# WinForms** | ✅ Abierto | Aplicación de escritorio |

---

## 🧪 PASO 5: PROBAR EL SISTEMA

### 5.1 Probar Frontend React

1. Abre el navegador en: **http://localhost:5173**
2. Deberías ver el menú principal
3. Prueba hacer clic en **"Gestión de Carros"**
4. Haz clic en **"Listar Carros"**
5. Deberías ver los carros de prueba: ABC-123, DEF-456, GHI-789

### 5.2 Probar Cliente C#

1. En la aplicación de Windows Forms que se abrió
2. Haz clic en **"Listar Carros"**
3. Deberías ver la lista de carros en una tabla
4. Prueba hacer clic en **"Crear Carro"** para agregar uno nuevo

### 5.3 Probar Backend directamente (opcional)

Abre CMD y ejecuta:

```bash
# Listar carros
curl -u admin:admin http://localhost:8080/api/carro

# Listar mantenimientos
curl -u admin:admin http://localhost:8080/api/mantenimiento

# Mantenimientos de un carro específico
curl -u admin:admin http://localhost:8080/api/mantenimiento/carro/ABC-123
```

---

## 🛑 DETENER TODO EL SISTEMA

### Detener Backend Spring Boot
En la ventana CMD del backend, presiona: **`Ctrl + C`**

### Detener Frontend React
En la ventana CMD de React, presiona: **`Ctrl + C`**

### Cerrar Cliente C#
Simplemente cierra la ventana de la aplicación.

### Detener Oracle (opcional, no recomendado)
```bash
lsnrctl stop
```

---

## 📝 COMANDOS RÁPIDOS DE REFERENCIA

### Iniciar todo desde cero:

```bash
# Terminal 1 - Backend
cd C:\src\EmpresarialesProyecto\EmpresarialesBackend
set JAVA_HOME=C:\Users\TU_USUARIO\.jdks\ms-17.0.16
mvnw.cmd spring-boot:run

# Terminal 2 - React (nueva ventana CMD)
cd C:\src\EmpresarialesProyecto\EmpresarialesCliente
npm run dev

# Terminal 3 - C# (nueva ventana CMD)
cd C:\src\EmpresarialesProyecto\EmpresarialesClienteCSharp
dotnet run
```

---

## 🔧 SOLUCIÓN DE PROBLEMAS RÁPIDA

### ❌ Backend no inicia - "Puerto 8080 en uso"
```bash
for /f "tokens=5" %a in ('netstat -aon ^| find ":8080" ^| find "LISTENING"') do taskkill /F /PID %a
```

### ❌ Oracle no responde
```bash
lsnrctl status
# Si no está corriendo:
lsnrctl start
```

### ❌ React muestra error de conexión
Verifica que el backend esté corriendo en `http://localhost:8080`

### ❌ C# no puede conectar al backend
Asegúrate de que:
1. El backend esté corriendo (puerto 8080)
2. No haya firewall bloqueando localhost

---

## ✅ CHECKLIST DE INICIO RÁPIDO

Marca cada paso al completarlo:

- [ ] Oracle corriendo (`lsnrctl status`)
- [ ] Tablas CARRO y MANTENIMIENTO creadas
- [ ] Backend iniciado (puerto 8080)
- [ ] Backend responde: `curl -u admin:admin http://localhost:8080/api/carro/healthCheck`
- [ ] React corriendo (puerto 5173)
- [ ] React se abre en navegador
- [ ] Cliente C# abierto
- [ ] Cliente C# muestra lista de carros

---

## 📞 ¿NECESITAS AYUDA?

Si algo no funciona:

1. Revisa que Oracle esté corriendo: `lsnrctl status`
2. Verifica que el backend no tenga errores en la consola
3. Asegúrate de que no haya puertos ocupados
4. Consulta el README.md principal para más detalles

---

## 🎯 ORDEN DE INICIO RECOMENDADO

```
1️⃣ Oracle Database (debe estar siempre corriendo)
   ↓
2️⃣ Backend Spring Boot (puerto 8080)
   ↓
3️⃣ Frontend React (puerto 5173)
   ↓
4️⃣ Cliente C# WinForms

✅ TODO LISTO PARA USAR
```

---

**Universidad de Ibagué - 2025-A**

**¡Sistema listo para demostración! 🚀**
