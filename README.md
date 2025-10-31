# 📋 Proyecto Empresariales - Sistema de Gestión de Carros y Mantenimientos

Sistema completo de gestión de vehículos y mantenimientos con arquitectura de microservicios, implementando relaciones maestro-detalle **@OneToMany/@ManyToOne** con Spring Boot, Oracle Database, y clientes en React y C#.

**Equipo de Desarrollo:**
- Juan David Reyes
- Julio David Suarez
- Sebastian Felipe Solano

**Universidad de Ibagué** - Facultad de Ingeniería - Desarrollo de Aplicaciones Empresariales - 2025-A

---

## 📦 1. REQUISITOS E INSTALACIÓN

### 1.1 Software Necesario

#### **Java Development Kit (JDK) 17** ✅
- **Descargar:** [Microsoft OpenJDK 17](https://learn.microsoft.com/en-us/java/openjdk/download#openjdk-17)
- **Versión:** 17.0.16 o superior
- **Verificar:**
  ```bash
  java -version
  ```

#### **Node.js y npm** ✅
- **Descargar:** [Node.js LTS](https://nodejs.org/)
- **Versión:** Node.js 18.x o superior
- **Verificar:**
  ```bash
  node -v
  npm -v
  ```

#### **.NET SDK 8.0** ✅
- **Descargar:** [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Verificar:**
  ```bash
  dotnet --version
  ```

#### **Oracle Database 21c Express Edition (XE)** ✅
- **Descargar:** [Oracle XE 21c](https://www.oracle.com/database/technologies/xe-downloads.html)
- **Puerto:** 1521
- **Bases de datos:** XE (Container) + XEPDB1 (Pluggable)

---

## 🗄️ 2. CONFIGURACIÓN DE BASE DE DATOS ORACLE

### 2.1 Crear Usuario DAE2025

Abrir **Command Prompt** y ejecutar:

```bash
sqlplus / as sysdba
```

Luego ejecutar estos comandos SQL:

```sql
-- Cambiar al Pluggable Database
ALTER SESSION SET CONTAINER = XEPDB1;

-- Habilitar creación de usuarios
ALTER SESSION SET "_ORACLE_SCRIPT"=true;

-- Crear usuario
CREATE USER DAE2025 IDENTIFIED BY DAE2025
DEFAULT TABLESPACE USERS
TEMPORARY TABLESPACE TEMP
QUOTA UNLIMITED ON USERS;

-- Otorgar permisos
GRANT CONNECT, RESOURCE, DBA TO DAE2025;
GRANT CREATE SESSION TO DAE2025;
GRANT CREATE TABLE TO DAE2025;
GRANT CREATE VIEW TO DAE2025;
GRANT CREATE SEQUENCE TO DAE2025;

EXIT;
```

---

### 2.2 Crear Tablas CARRO y MANTENIMIENTO

#### **Script SQL completo:**

**Ubicación:** `EmpresarialesBackend\CREAR_BD_ORACLE.sql`

Conectarse como DAE2025:
```bash
sqlplus DAE2025/DAE2025@localhost:1521/xepdb1
```

Ejecutar el script:
```sql
@C:\src\EmpresarialesProyecto\EmpresarialesBackend\CREAR_BD_ORACLE.sql
```

**O copiar y pegar este código:**

```sql
-- Borrar tablas si existen
BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE MANTENIMIENTO CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE CARRO CASCADE CONSTRAINTS';
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

-- TABLA MAESTRA: CARRO
CREATE TABLE CARRO (
    placa VARCHAR2(10) PRIMARY KEY,
    marca VARCHAR2(50) NOT NULL,
    modelo VARCHAR2(50) NOT NULL,
    anio NUMBER(4) NOT NULL CHECK (anio >= 1900 AND anio <= 2100),
    color VARCHAR2(30) NOT NULL,
    numero_puertas NUMBER(1) NOT NULL CHECK (numero_puertas BETWEEN 2 AND 5),
    tiene_aire_acondicionado NUMBER(1) DEFAULT 0 CHECK (tiene_aire_acondicionado IN (0, 1)),
    precio NUMBER(10,2) NOT NULL CHECK (precio > 0),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tipo_transmision VARCHAR2(20) NOT NULL CHECK (tipo_transmision IN ('MANUAL', 'AUTOMATICA')),
    estado VARCHAR2(20) DEFAULT 'DISPONIBLE'
);

-- TABLA DETALLE: MANTENIMIENTO (Relación 1:N con CARRO)
CREATE TABLE MANTENIMIENTO (
    id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    placa_carro VARCHAR2(10) NOT NULL,
    fecha_mantenimiento TIMESTAMP NOT NULL,
    kilometraje NUMBER(10),
    tipo_mantenimiento VARCHAR2(50) NOT NULL,
    costo NUMBER(10,2) NOT NULL,
    descripcion VARCHAR2(500) NOT NULL,
    proximo_mantenimiento TIMESTAMP,
    completado NUMBER(1) DEFAULT 0,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- FOREIGN KEY: Relación Maestro-Detalle
    CONSTRAINT fk_mantenimiento_carro FOREIGN KEY (placa_carro)
        REFERENCES CARRO(placa) ON DELETE CASCADE
);

-- Índices
CREATE INDEX idx_mantenimiento_placa ON MANTENIMIENTO(placa_carro);

-- Datos de prueba
INSERT INTO CARRO VALUES ('ABC-123', 'Toyota', 'Corolla', 2022, 'Blanco', 4, 1, 25000.00, CURRENT_TIMESTAMP, 'AUTOMATICA', 'DISPONIBLE');
INSERT INTO CARRO VALUES ('DEF-456', 'Honda', 'Civic', 2021, 'Negro', 4, 1, 23000.00, CURRENT_TIMESTAMP, 'MANUAL', 'DISPONIBLE');
INSERT INTO CARRO VALUES ('GHI-789', 'Mazda', 'CX-5', 2023, 'Rojo', 5, 1, 35000.00, CURRENT_TIMESTAMP, 'AUTOMATICA', 'DISPONIBLE');

INSERT INTO MANTENIMIENTO (placa_carro, fecha_mantenimiento, kilometraje, tipo_mantenimiento, costo, descripcion, proximo_mantenimiento, completado)
VALUES ('ABC-123', TIMESTAMP '2024-01-15 09:30:00', 15000, 'PREVENTIVO', 350.00, 'Cambio de aceite y filtros', TIMESTAMP '2024-07-15 09:30:00', 1);

INSERT INTO MANTENIMIENTO (placa_carro, fecha_mantenimiento, kilometraje, tipo_mantenimiento, costo, descripcion, proximo_mantenimiento, completado)
VALUES ('ABC-123', TIMESTAMP '2024-06-20 14:00:00', 22000, 'CAMBIO_ACEITE', 180.00, 'Cambio de aceite sintético', TIMESTAMP '2024-12-20 14:00:00', 1);

COMMIT;
```

---

### 2.3 Verificar Datos

```sql
-- Ver carros
SELECT * FROM CARRO;

-- Ver mantenimientos
SELECT * FROM MANTENIMIENTO;

-- Consulta maestro-detalle
SELECT c.placa, c.marca, c.modelo, m.tipo_mantenimiento, m.costo
FROM CARRO c
LEFT JOIN MANTENIMIENTO m ON c.placa = m.placa_carro
ORDER BY c.placa;
```

---

## 🏗️ 3. ARQUITECTURA DEL PROYECTO

### 3.1 Relación Maestro-Detalle (@OneToMany/@ManyToOne)

```
┌─────────────────────┐
│      CARRO          │  (Maestro)
│  @OneToMany         │
├─────────────────────┤
│ placa (PK)          │───┐
│ marca               │   │
│ modelo              │   │ 1:N
│ precio              │   │
└─────────────────────┘   │
                          │
                          ▼
┌─────────────────────┐
│   MANTENIMIENTO     │  (Detalle)
│   @ManyToOne        │
├─────────────────────┤
│ id (PK)             │
│ placa_carro (FK)    │◄──┘
│ tipo_mantenimiento  │
│ costo               │
└─────────────────────┘
```

**Anotaciones JPA implementadas:**

```java
// Carro.java
@OneToMany(mappedBy = "carro", cascade = CascadeType.ALL,
           orphanRemoval = true, fetch = FetchType.LAZY)
@JsonManagedReference
private List<Mantenimiento> mantenimientos;

// Mantenimiento.java
@ManyToOne(fetch = FetchType.LAZY)
@JoinColumn(name = "placa_carro", nullable = false)
@JsonBackReference
private Carro carro;
```

---

## 🚀 4. EJECUCIÓN DE APLICACIONES

### 4.1 Backend - Spring Boot (Puerto 8080)

```bash
# Navegar al directorio
cd C:\src\EmpresarialesProyecto\EmpresarialesBackend

# Configurar JAVA_HOME (si es necesario)
set JAVA_HOME=C:\Users\TU_USUARIO\.jdks\ms-17.0.16

# Compilar y ejecutar
mvnw.cmd spring-boot:run
```

**Verificar que esté funcionando:**
```bash
curl -u admin:admin http://localhost:8080/api/carro/healthCheck
```

**Credenciales:** `admin / admin`

---

### 4.2 Frontend - React (Puerto 5173)

```bash
# Navegar al directorio
cd C:\src\EmpresarialesProyecto\EmpresarialesCliente

# Instalar dependencias (solo primera vez)
npm install

# Ejecutar en desarrollo
npm run dev
```

**Acceder:** `http://localhost:5173`

---

### 4.3 Cliente - C# WinForms

```bash
# Navegar al directorio
cd C:\src\EmpresarialesProyecto\EmpresarialesClienteCSharp

# Restaurar paquetes
dotnet restore

# Compilar y ejecutar
dotnet run
```

**O abrir en Visual Studio 2022 y presionar F5**

---

## 📡 5. API REST - ENDPOINTS PRINCIPALES

### 5.1 Carros

```bash
# Listar todos
GET http://localhost:8080/api/carro

# Buscar por placa
GET http://localhost:8080/api/carro?placa=ABC-123

# Crear carro
POST http://localhost:8080/api/carro
Content-Type: application/json
{
  "placa": "XYZ-999",
  "marca": "Nissan",
  "modelo": "Sentra",
  "anio": 2023,
  "color": "Gris",
  "numeroPuertas": 4,
  "tieneAireAcondicionado": true,
  "precio": 28000.00,
  "tipoTransmision": "AUTOMATICA",
  "estado": "DISPONIBLE"
}

# Actualizar
PUT http://localhost:8080/api/carro/ABC-123

# Eliminar
DELETE http://localhost:8080/api/carro/ABC-123
```

---

### 5.2 Mantenimientos

```bash
# Listar todos
GET http://localhost:8080/api/mantenimiento

# Mantenimientos de un carro (maestro-detalle)
GET http://localhost:8080/api/mantenimiento/carro/ABC-123

# Mantenimientos urgentes
GET http://localhost:8080/api/mantenimiento?action=urgentes

# Estadísticas
GET http://localhost:8080/api/mantenimiento?action=estadisticas

# Crear mantenimiento
POST http://localhost:8080/api/mantenimiento
{
  "carro": {"placa": "ABC-123"},
  "fechaMantenimiento": "2024-11-01T10:00:00",
  "kilometraje": 30000,
  "tipoMantenimiento": "CAMBIO_ACEITE",
  "costo": 200.00,
  "descripcion": "Cambio de aceite sintético",
  "proximoMantenimiento": "2025-05-01T10:00:00",
  "completado": false
}

# Actualizar
PUT http://localhost:8080/api/mantenimiento/1

# Eliminar
DELETE http://localhost:8080/api/mantenimiento/1
```

---

## 🛠️ 6. SOLUCIÓN DE PROBLEMAS

### Puerto 8080 ocupado
```bash
# Ver proceso en puerto 8080
netstat -ano | findstr :8080

# Matar proceso (reemplazar PID)
taskkill /F /PID <PID>
```

### Error de conexión a Oracle
```bash
# Verificar servicio
lsnrctl status

# Asegurarse de usar /xepdb1 no /xe
jdbc:oracle:thin:@localhost:1521/xepdb1
```

### Error de memoria JVM
Ya corregido en `.mvn\jvm.config`:
```
-Xmx512m
-Xms256m
```

---

## 📊 7. CARACTERÍSTICAS IMPLEMENTADAS

✅ **Arquitectura de microservicios** (Spring Boot REST API)
✅ **Base de datos relacional Oracle** (21c XE)
✅ **Operaciones CRUD completas**
✅ **Relación Maestro-Detalle @OneToMany/@ManyToOne**
✅ **12+ queries JPA personalizadas en MantenimientoRepository**
✅ **10+ queries JPA personalizadas en CarroRepository**
✅ **Consultas maestro-detalle con JOIN FETCH**
✅ **Validaciones con Bean Validation**
✅ **Transacciones con @Transactional**
✅ **Connection pooling con HikariCP**
✅ **Autenticación HTTP Basic Auth**
✅ **CORS habilitado**
✅ **Clientes en React y C# WinForms**

---

## 📝 8. COMANDOS RÁPIDOS

### Oracle
```bash
# Conectar como SYSDBA
sqlplus / as sysdba

# Conectar como DAE2025
sqlplus DAE2025/DAE2025@localhost:1521/xepdb1

# Ver tablas
SELECT table_name FROM user_tables;
```

### Spring Boot
```bash
cd EmpresarialesBackend
mvnw.cmd spring-boot:run
mvnw.cmd clean package    # Compilar JAR
```

### React
```bash
cd EmpresarialesCliente
npm install
npm run dev
npm run build     # Producción
```

### C#
```bash
cd EmpresarialesClienteCSharp
dotnet restore
dotnet run
dotnet build --configuration Release
```

---

## 📚 9. TECNOLOGÍAS

| Componente | Tecnología | Versión |
|------------|------------|---------|
| Backend | Spring Boot | 3.5.5 |
| Java | OpenJDK | 17.0.16 |
| ORM | Hibernate/JPA | 6.6.26 |
| Base de Datos | Oracle XE | 21.3 |
| Frontend | React + Vite | 18.x |
| Cliente Desktop | .NET WinForms | 8.0 |
| Build Tool | Maven | 3.9.x |

---

## 🎯 10. ESTRUCTURA DE DIRECTORIOS

```
EmpresarialesProyecto/
│
├── EmpresarialesBackend/              # Spring Boot API
│   ├── src/main/java/.../
│   │   ├── controller/                # REST Controllers
│   │   ├── service/                   # Business Logic
│   │   ├── repository/                # JPA Repositories
│   │   ├── model/                     # Entities (Carro, Mantenimiento)
│   │   └── config/                    # Security Config
│   ├── src/main/resources/
│   │   └── application.properties     # Oracle connection
│   ├── .mvn/jvm.config               # JVM memory config
│   ├── pom.xml
│   └── CREAR_BD_ORACLE.sql           # Database script
│
├── EmpresarialesCliente/             # React Frontend
│   ├── src/pages/                    # React pages
│   ├── package.json
│   └── vite.config.ts
│
└── EmpresarialesClienteCSharp/       # C# WinForms
    ├── Forms/                        # Windows Forms
    ├── Utils/ApiClient.cs            # HTTP Client
    └── Program.cs
```

---

## ✅ CHECKLIST DE INICIO

- [ ] Instalar Java 17
- [ ] Instalar Node.js 18+
- [ ] Instalar .NET 8.0
- [ ] Instalar Oracle XE 21c
- [ ] Crear usuario DAE2025
- [ ] Ejecutar script de tablas
- [ ] Verificar conexión a BD
- [ ] Iniciar backend (puerto 8080)
- [ ] Iniciar frontend React (puerto 5173)
- [ ] Compilar cliente C#
- [ ] Probar endpoints con curl

---

**¡Proyecto listo para ejecutar! 🚀**

**Universidad de Ibagué** - 2025-A
