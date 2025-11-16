# 🚀 GUÍA RÁPIDA DE EJECUCIÓN DEL SISTEMA

## Guía paso a paso para ejecutar todo el sistema completo

---

## 📋 REQUISITOS PREVIOS

Antes de comenzar, asegúrate de tener instalado:

- ✅ **Java 17** (OpenJDK)
- ✅ **Node.js 18+** y npm
- ✅ **.NET 8.0 SDK**
- ✅ **Oracle Database 21c XE**
- ✅ **PostgreSQL 16+** (para microservicio Conductores)

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

## 🐘 PASO 1.5: VERIFICAR BASE DE DATOS POSTGRESQL

### 1.5.1 Verificar que PostgreSQL esté corriendo

Abrir **Command Prompt** (CMD):

```bash
# Verificar servicio PostgreSQL
sc query postgresql-x64-16
```

✅ **Debe mostrar:** `STATE: 4 RUNNING`

Si no está corriendo:
```bash
# Iniciar servicio PostgreSQL
net start postgresql-x64-16
```

💡 **Nota:** El nombre del servicio puede variar según tu instalación (postgresql-x64-16, postgresql-16, etc.)

---

### 1.5.2 Conectarse a PostgreSQL y verificar tabla CONDUCTOR

Puedes usar **DBeaver** (recomendado) o **psql** desde CMD:

#### Opción A: Usando DBeaver

1. Abrir **DBeaver**
2. Conectarse a la conexión PostgreSQL existente:
   - **Host:** localhost
   - **Port:** 5432
   - **Database:** postgres
   - **Username:** postgres
   - **Password:** postgres
3. Ejecutar consulta SQL:

```sql
-- Ver las tablas
SELECT table_name FROM information_schema.tables
WHERE table_schema = 'public';

-- Debe mostrar la tabla: conductor

-- Ver estructura de la tabla
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'conductor';

-- Ver datos de prueba
SELECT * FROM conductor;
```

#### Opción B: Usando psql (línea de comandos)

```bash
# Conectarse a PostgreSQL
psql -U postgres -d postgres

# Una vez dentro de psql:
\dt                    # Ver tablas
SELECT * FROM conductor;  # Ver datos

# Salir
\q
```

✅ **Si la tabla CONDUCTOR existe, puedes continuar.**

❌ **Si NO existe la tabla CONDUCTOR, créala usando DBeaver:**

```sql
CREATE TABLE conductor (
    cedula VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    licencia_numero VARCHAR(50),
    fecha_nacimiento TIMESTAMP,
    salario NUMERIC(10,2),
    activo NUMERIC(1,0) DEFAULT 1,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insertar datos de prueba
INSERT INTO conductor (cedula, nombre, apellido, telefono, licencia_numero, salario, activo)
VALUES
    ('1001234567', 'Juan', 'Pérez', '3101234567', 'LIC-001', 2500000, 1),
    ('1007654321', 'María', 'González', '3157654321', 'LIC-002', 2800000, 1);
```

---

## 🖥️ PASO 2: INICIAR MICROSERVICIO CONDUCTORES (Spring Boot)

⚠️ **IMPORTANTE:** Este microservicio debe iniciarse ANTES del Backend Principal.

### 2.1 Abrir una nueva ventana de CMD

**Windows:** Presiona `Win + R` → escribe `cmd` → Enter

### 2.2 Navegar al directorio del microservicio

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesConductores
```

### 2.3 Configurar JAVA_HOME (si es necesario)

```bash
set JAVA_HOME=C:\Users\Sebastian\.jdks\ms-17.0.16
```

💡 **Nota:** Reemplaza con tu ruta de Java si es diferente.

### 2.4 Ejecutar el microservicio

```bash
mvnw.cmd spring-boot:run
```

⏳ **Espera a que aparezca:**

```
Started EmpresarialesConductores in X.XXX seconds
Tomcat started on port 8081 (http)
```

✅ **El microservicio Conductores está corriendo en:** `http://localhost:8081`

### 2.5 Verificar que funciona (opcional)

Abrir otra ventana CMD y ejecutar:

```bash
curl -u admin:admin http://localhost:8081/api/conductor/healthCheck
```

Debe responder: `"Conductor API Status Ok!"`

💡 **IMPORTANTE:** Deja esta ventana CMD abierta. El microservicio debe seguir corriendo.

---

## 🖥️ PASO 3: INICIAR BACKEND PRINCIPAL (Spring Boot)

⚠️ **IMPORTANTE:** El Microservicio Conductores (puerto 8081) debe estar corriendo antes de iniciar este backend.

### 3.1 Abrir una nueva ventana de CMD

**Windows:** Presiona `Win + R` → escribe `cmd` → Enter

### 3.2 Navegar al directorio del backend

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesBackend
```

### 3.3 Configurar JAVA_HOME (si es necesario)

```bash
set JAVA_HOME=C:\Users\Sebastian\.jdks\ms-17.0.16
```

💡 **Nota:** Reemplaza `TU_USUARIO` con tu nombre de usuario de Windows.

### 3.4 Ejecutar el backend

```bash
mvnw.cmd spring-boot:run
```

⏳ **Espera a que aparezca:**

```
Started EmpresarialesProyecto in X.XXX seconds
Tomcat started on port 8080 (http)
```

✅ **El backend está corriendo en:** `http://localhost:8080`

💡 **Este backend actúa como proxy/gateway:** Los clientes (React y C#) se conectan solo al puerto 8080, y este backend internamente llama al microservicio de Conductores (puerto 8081) cuando es necesario.

### 3.5 Verificar que funciona (opcional)

Abrir otra ventana CMD y ejecutar:

```bash
curl -u admin:admin http://localhost:8080/api/carro/healthCheck
```

Debe responder: `"Carros API Status Ok!"`

```bash
# Probar endpoint de conductores (vía proxy)
curl -u admin:admin http://localhost:8080/api/conductor/healthCheck
```

Debe responder: `"Conductor API Status Ok!"` (internamente llama al puerto 8081)

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

## 🌐 PASO 4: INICIAR FRONTEND REACT

### 4.1 Abrir OTRA ventana de CMD (nueva)

**Importante:** No cierres las ventanas de los microservicios y backend.

### 4.2 Navegar al directorio de React

```bash
cd C:\src\EmpresarialesProyecto\EmpresarialesCliente
```

### 4.3 Instalar dependencias (solo primera vez)

```bash
npm install
```

⏳ **Espera 1-2 minutos** mientras se descargan las dependencias.

### 4.4 Ejecutar React en modo desarrollo

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

💡 **Nota:** El frontend React solo se comunica con el puerto 8080 (backend principal). El backend principal se encarga de comunicarse con el microservicio de Conductores cuando sea necesario.

### 4.5 Abrir en el navegador

1. Abre tu navegador web (Chrome, Edge, Firefox)
2. Ve a: **http://localhost:5173**
3. Deberías ver la página principal del sistema

---

## 🖥️ PASO 5: EJECUTAR CLIENTE C# (Windows Forms)

Tienes **2 opciones**: Visual Studio o línea de comandos.

💡 **Nota:** El cliente C# solo se comunica con el puerto 8080 (backend principal). No necesita conocer el microservicio de Conductores.

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

Ahora deberías tener **6 componentes activos** en la arquitectura de microservicios:

| Componente | Estado | Puerto/Ubicación | Base de Datos |
|------------|--------|------------------|---------------|
| 🗄️ **Oracle Database** | ✅ Corriendo | Puerto 1521 | DAE2025 (CARRO, MANTENIMIENTO) |
| 🐘 **PostgreSQL Database** | ✅ Corriendo | Puerto 5432 | postgres (CONDUCTOR) |
| 🖥️ **Microservicio Conductores** | ✅ Corriendo | http://localhost:8081 | PostgreSQL |
| 🖥️ **Backend Principal (Gateway)** | ✅ Corriendo | http://localhost:8080 | Oracle + Proxy a 8081 |
| 🌐 **Frontend React** | ✅ Corriendo | http://localhost:5173 | Cliente → 8080 |
| 🖼️ **Cliente C# WinForms** | ✅ Abierto | Aplicación de escritorio | Cliente → 8080 |

### 🏗️ Arquitectura de Comunicación:

```
React (5173) ───┐
                ├──→ Backend Principal (8080) ──┬──→ Oracle (DAE2025): CARRO, MANTENIMIENTO
C# WinForms ────┘                               │
                                                └──→ Microservicio Conductores (8081) ──→ PostgreSQL: CONDUCTOR
```

💡 **Los clientes (React y C#) solo conocen el puerto 8080.** El Backend Principal actúa como gateway/proxy y distribuye las peticiones a los microservicios correspondientes.

---

## 🧪 PASO 6: PROBAR EL SISTEMA

### 6.1 Probar Frontend React

1. Abre el navegador en: **http://localhost:5173**
2. Deberías ver el menú principal
3. Prueba hacer clic en **"Gestión de Carros"**
4. Haz clic en **"Listar Carros"**
5. Deberías ver los carros de prueba: ABC-123, DEF-456, GHI-789
6. Prueba hacer clic en **"Gestión de Conductores"**
7. Deberías ver los conductores de prueba (Juan Pérez, María González)

### 6.2 Probar Cliente C#

1. En la aplicación de Windows Forms que se abrió
2. Haz clic en **"Listar Carros"**
3. Deberías ver la lista de carros en una tabla
4. Prueba hacer clic en **"Crear Carro"** para agregar uno nuevo
5. Haz clic en **"Listar Conductores"**
6. Deberías ver los conductores de prueba

### 6.3 Probar Backend directamente (opcional)

Abre CMD y ejecuta:

```bash
# Listar carros (desde Oracle vía Backend Principal)
curl -u admin:admin http://localhost:8080/api/carro

# Listar mantenimientos (desde Oracle vía Backend Principal)
curl -u admin:admin http://localhost:8080/api/mantenimiento

# Mantenimientos de un carro específico
curl -u admin:admin http://localhost:8080/api/mantenimiento/carro/ABC-123

# Listar conductores (desde PostgreSQL vía Proxy/Gateway)
curl -u admin:admin http://localhost:8080/api/conductor

# Buscar conductor por cédula
curl -u admin:admin http://localhost:8080/api/conductor/1001234567

# Listar conductores activos
curl -u admin:admin "http://localhost:8080/api/conductor?activo=true"
```

💡 **Nota:** Todos los comandos apuntan al puerto 8080. El backend principal se encarga de:
- Servir datos de CARRO y MANTENIMIENTO desde Oracle (DAE2025)
- Redirigir peticiones de CONDUCTOR al microservicio en puerto 8081 (PostgreSQL)

---

## 🛑 DETENER TODO EL SISTEMA

⚠️ **Orden recomendado de apagado** (inverso al inicio):

### 1. Cerrar Cliente C#
Simplemente cierra la ventana de la aplicación.

### 2. Detener Frontend React
En la ventana CMD de React, presiona: **`Ctrl + C`**

### 3. Detener Backend Principal (puerto 8080)
En la ventana CMD del backend principal, presiona: **`Ctrl + C`**

### 4. Detener Microservicio Conductores (puerto 8081)
En la ventana CMD del microservicio, presiona: **`Ctrl + C`**

### 5. Detener PostgreSQL (opcional, no recomendado)
```bash
net stop postgresql-x64-16
```

### 6. Detener Oracle (opcional, no recomendado)
```bash
lsnrctl stop
```

💡 **Recomendación:** Deja las bases de datos (Oracle y PostgreSQL) corriendo. Solo detén los servicios Java y React.

---

## 📝 COMANDOS RÁPIDOS DE REFERENCIA

### Iniciar todo desde cero:

```bash
# Terminal 1 - Microservicio Conductores (PRIMERO)
cd C:\src\EmpresarialesProyecto\EmpresarialesConductores
set JAVA_HOME=C:\Users\Sebastian\.jdks\ms-17.0.16
mvnw.cmd spring-boot:run

# Terminal 2 - Backend Principal (SEGUNDO, después del microservicio)
cd C:\src\EmpresarialesProyecto\EmpresarialesBackend
set JAVA_HOME=C:\Users\Sebastian\.jdks\ms-17.0.16
mvnw.cmd spring-boot:run

# Terminal 3 - React (nueva ventana CMD)
cd C:\src\EmpresarialesProyecto\EmpresarialesCliente
npm run dev

# Terminal 4 - C# (nueva ventana CMD)
cd C:\src\EmpresarialesProyecto\EmpresarialesClienteCSharp
dotnet run
```

⚠️ **IMPORTANTE:** Debes iniciar el Microservicio Conductores (8081) ANTES del Backend Principal (8080).

---

## 🔧 SOLUCIÓN DE PROBLEMAS RÁPIDA

### ❌ Puerto 8080 ya está en uso
```bash
for /f "tokens=5" %a in ('netstat -aon ^| find ":8080" ^| find "LISTENING"') do taskkill /F /PID %a
```

### ❌ Puerto 8081 ya está en uso (Microservicio Conductores)
```bash
for /f "tokens=5" %a in ('netstat -aon ^| find ":8081" ^| find "LISTENING"') do taskkill /F /PID %a
```

### ❌ Backend Principal no puede conectar con Microservicio Conductores
**Síntoma:** Errores como "Connection refused" o "Cannot invoke..." al intentar listar conductores.

**Solución:**
1. Verifica que el microservicio esté corriendo: `curl -u admin:admin http://localhost:8081/api/conductor/healthCheck`
2. Si no responde, inicia el microservicio primero (puerto 8081)
3. Luego reinicia el backend principal (puerto 8080)

### ❌ Oracle no responde
```bash
lsnrctl status
# Si no está corriendo:
lsnrctl start
```

### ❌ PostgreSQL no responde
```bash
sc query postgresql-x64-16
# Si no está corriendo:
net start postgresql-x64-16
```

### ❌ React muestra error de conexión
Verifica que el backend principal esté corriendo en `http://localhost:8080`

### ❌ C# no puede conectar al backend
Asegúrate de que:
1. El backend principal esté corriendo (puerto 8080)
2. El microservicio conductores esté corriendo (puerto 8081)
3. No haya firewall bloqueando localhost

### ❌ "Table or view does not exist" en Oracle
Ejecuta el script de creación:
```bash
sqlplus DAE2025/DAE2025@localhost:1521/xepdb1
@C:\src\EmpresarialesProyecto\EmpresarialesBackend\CREAR_BD_ORACLE.sql
```

### ❌ "Relation 'conductor' does not exist" en PostgreSQL
Conéctate a PostgreSQL con DBeaver y ejecuta el script CREATE TABLE del PASO 1.5.2

---

## ✅ CHECKLIST DE INICIO RÁPIDO

Marca cada paso al completarlo:

### 🗄️ Bases de Datos
- [ ] Oracle corriendo (`lsnrctl status`)
- [ ] Tablas CARRO y MANTENIMIENTO creadas en Oracle (DAE2025)
- [ ] PostgreSQL corriendo (`sc query postgresql-x64-16`)
- [ ] Tabla CONDUCTOR creada en PostgreSQL (postgres database)

### 🖥️ Microservicios y Backend
- [ ] Microservicio Conductores iniciado (puerto 8081)
- [ ] Microservicio responde: `curl -u admin:admin http://localhost:8081/api/conductor/healthCheck`
- [ ] Backend Principal iniciado (puerto 8080)
- [ ] Backend responde: `curl -u admin:admin http://localhost:8080/api/carro/healthCheck`
- [ ] Proxy funciona: `curl -u admin:admin http://localhost:8080/api/conductor/healthCheck`

### 🌐 Clientes
- [ ] React corriendo (puerto 5173)
- [ ] React se abre en navegador
- [ ] React muestra lista de carros
- [ ] React muestra lista de conductores
- [ ] Cliente C# abierto
- [ ] Cliente C# muestra lista de carros
- [ ] Cliente C# muestra lista de conductores

---

## 📞 ¿NECESITAS AYUDA?

Si algo no funciona:

1. Revisa que Oracle esté corriendo: `lsnrctl status`
2. Revisa que PostgreSQL esté corriendo: `sc query postgresql-x64-16`
3. Verifica que el Microservicio Conductores (8081) esté iniciado ANTES del Backend Principal
4. Verifica que ningún servicio tenga errores en la consola
5. Asegúrate de que no haya puertos ocupados (8080, 8081, 5173)
6. Prueba los health checks:
   - `curl -u admin:admin http://localhost:8081/api/conductor/healthCheck`
   - `curl -u admin:admin http://localhost:8080/api/carro/healthCheck`
   - `curl -u admin:admin http://localhost:8080/api/conductor/healthCheck` (vía proxy)
7. Consulta el README.md principal para más detalles

---

## 🎯 ORDEN DE INICIO RECOMENDADO

```
1️⃣ Bases de Datos (deben estar siempre corriendo)
   ├── Oracle Database (puerto 1521) - DAE2025
   └── PostgreSQL (puerto 5432) - postgres
   ↓
2️⃣ Microservicio Conductores (puerto 8081) ⚠️ PRIMERO
   ↓
3️⃣ Backend Principal / Gateway (puerto 8080) ⚠️ SEGUNDO
   ↓
4️⃣ Frontend React (puerto 5173)
   ↓
5️⃣ Cliente C# WinForms

✅ TODO LISTO PARA USAR
```

### 📐 Arquitectura de Microservicios:

```
┌─────────────────────────────────────────────────────────────────┐
│                         CAPA DE CLIENTES                         │
├─────────────────────────────────────────────────────────────────┤
│  React (5173)                            C# WinForms             │
│       │                                        │                 │
│       └────────────────┬───────────────────────┘                 │
│                        ▼                                         │
│              Backend Principal / Gateway (8080)                  │
│       [Authentication + Proxy Pattern + API Gateway]             │
│                        │                                         │
│       ┌────────────────┴────────────────┐                        │
│       ▼                                 ▼                        │
│  DATOS LOCALES                  MICROSERVICIO EXTERNO            │
│  (Oracle DAE2025)               (Conductores 8081)               │
│       │                                 │                        │
│       ├─ Carros                         └─ Conductores           │
│       └─ Mantenimientos                    (PostgreSQL)          │
│          (Oracle)                                                │
└─────────────────────────────────────────────────────────────────┘

💡 Patrón de Diseño: Backend Principal actúa como API Gateway/Proxy
   - Los clientes solo conocen el puerto 8080
   - Transparencia: Los clientes no saben que Conductores es un microservicio externo
   - Centralización: Autenticación única (admin/admin) en el gateway
   - Escalabilidad: Fácil agregar más microservicios sin cambiar clientes
```

---

**Universidad de Ibagué - 2025-A**

**¡Sistema listo para demostración! 🚀**
