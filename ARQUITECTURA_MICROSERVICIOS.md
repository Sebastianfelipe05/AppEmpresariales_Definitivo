# Arquitectura de Microservicios

## Proyecto: Sistema de Gestión de Carros, Mantenimientos y Conductores

### Universidad de Ibagué - Desarrollo de Aplicaciones Empresariales 2025-A

---

## 📐 Diagrama de Arquitectura

```
┌─────────────────────┐         ┌─────────────────────┐
│   Cliente React     │         │   Cliente C#        │
│   (Puerto 5173)     │         │   (.NET 8.0)        │
│   GUI 1             │         │   GUI 2             │
└──────────┬──────────┘         └──────────┬──────────┘
           │                               │
           │    HTTP Basic Auth            │
           │    (admin/admin)              │
           │                               │
           └───────────────┬───────────────┘
                           │
                           ▼
         ┌─────────────────────────────────────────┐
         │  MICROSERVICIO A-B (Principal)          │
         │  Puerto: 8080                           │
         │  ─────────────────────────────────────  │
         │  • Carro (Clase A - Maestro)            │
         │  • Mantenimiento (Clase B - Detalle)    │
         │  • ConductorProxyController             │
         │  ─────────────────────────────────────  │
         │  Base de Datos: Oracle 21c XE           │
         │  Usuario: DAE2025 / DAE2025             │
         │  Tablas: CARRO, MANTENIMIENTO           │
         └────────────────┬────────────────────────┘
                          │
                          │ RestTemplate
                          │ HTTP Proxy
                          │
                          ▼
         ┌─────────────────────────────────────────┐
         │  MICROSERVICIO C (Conductores)          │
         │  Puerto: 8081                           │
         │  ─────────────────────────────────────  │
         │  • Conductor (Clase C)                  │
         │  • CRUD Independiente                   │
         │  ─────────────────────────────────────  │
         │  Base de Datos: PostgreSQL              │
         │  Usuario: postgres / postgres           │
         │  Base de Datos: conductores_db          │
         │  Tabla: CONDUCTOR                       │
         └─────────────────────────────────────────┘
```

---

## 🗂️ Estructura de Directorios

```
EmpresarialesProyecto/
│
├── EmpresarialesBackend/              ← MICROSERVICIO A-B
│   ├── src/main/java/cal/example/POCEmpleado/
│   │   ├── model/
│   │   │   ├── Carro.java            (Clase A - Maestro)
│   │   │   ├── Mantenimiento.java    (Clase B - Detalle)
│   │   │   └── ConductorDTO.java     (DTO para comunicación)
│   │   ├── repository/
│   │   │   ├── CarroRepository.java
│   │   │   └── MantenimientoRepository.java
│   │   ├── service/
│   │   │   ├── CarroService.java
│   │   │   ├── MantenimientoService.java
│   │   │   └── ConductorProxyService.java  ← Comunicación con Microservicio C
│   │   ├── controller/
│   │   │   ├── CarroController.java
│   │   │   ├── MantenimientoController.java
│   │   │   └── ConductorProxyController.java  ← Proxy hacia puerto 8081
│   │   └── config/
│   │       └── SecurityConfig.java
│   ├── src/main/resources/
│   │   └── application.properties    (Oracle DB + URL Microservicio C)
│   └── pom.xml                        (Oracle JDBC)
│
├── EmpresarialesConductores/          ← MICROSERVICIO C (INDEPENDIENTE)
│   ├── src/main/java/cal/example/conductores/
│   │   ├── model/
│   │   │   └── Conductor.java        (Clase C)
│   │   ├── repository/
│   │   │   └── ConductorRepository.java
│   │   ├── service/
│   │   │   ├── IConductorService.java
│   │   │   └── ConductorService.java
│   │   ├── controller/
│   │   │   └── ConductorController.java
│   │   ├── config/
│   │   │   ├── SecurityConfig.java
│   │   │   └── DataInitializer.java
│   │   └── errors/
│   │       └── ErrorMessage.java
│   ├── src/main/resources/
│   │   └── application.properties    (PostgreSQL DB)
│   ├── pom.xml                        (PostgreSQL JDBC)
│   ├── mvnw.cmd
│   └── README.md
│
├── EmpresarialesCliente/              ← CLIENTE GUI 1 (React)
│   ├── src/
│   │   ├── pages/
│   │   │   ├── CreateCarro.tsx
│   │   │   ├── ListarCarros.tsx
│   │   │   ├── CrearMantenimiento.tsx
│   │   │   ├── CrearConductor.tsx       ← Nuevas páginas
│   │   │   └── ListarConductores.tsx    ← Nuevas páginas
│   │   ├── services/
│   │   │   ├── carroApi.ts
│   │   │   ├── mantenimientoApi.ts
│   │   │   └── conductorApi.ts          ← Nueva API
│   │   └── types/
│   │       ├── Carro.ts
│   │       ├── Mantenimiento.ts
│   │       └── Conductor.ts             ← Nuevo tipo
│   └── package.json
│
├── EmpresarialesClienteCSharp/        ← CLIENTE GUI 2 (C#)
│   ├── Forms/
│   │   ├── CrearCarroForm.cs
│   │   ├── ListarCarrosForm.cs
│   │   ├── CrearMantenimientoForm.cs
│   │   ├── CrearConductorForm.cs        ← Nuevos formularios
│   │   └── ListarConductoresForm.cs     ← Nuevos formularios
│   ├── Services/
│   │   ├── CarroService.cs
│   │   ├── MantenimientoService.cs
│   │   └── ConductorService.cs          ← Nuevo servicio
│   └── Models/
│       ├── Carro.cs
│       ├── Mantenimiento.cs
│       └── Conductor.cs                 ← Nuevo modelo
│
├── CREAR_BD_ORACLE.sql                (Script para Microservicio A-B)
├── CREAR_BD_POSTGRESQL.sql            (Script para Microservicio C)
└── ARQUITECTURA_MICROSERVICIOS.md     (Este archivo)
```

---

## 🔑 Características Clave de Separación de Microservicios

### ✅ Separación de Responsabilidades

| Característica | Microservicio A-B | Microservicio C |
|----------------|-------------------|-----------------|
| **Puerto** | 8080 | 8081 |
| **Base de Datos** | Oracle 21c XE | PostgreSQL |
| **Esquema/DB** | DAE2025/XEPDB1 | conductores_db |
| **Responsabilidad** | Gestión de Carros y Mantenimientos | Gestión de Conductores |
| **Tecnología BD** | ojdbc11 | postgresql |
| **Independencia** | ✅ Puede ejecutarse solo | ✅ Puede ejecutarse solo |

### ✅ Comunicación entre Microservicios

```java
// En Microservicio A-B (Puerto 8080)
@Service
public class ConductorProxyService {
    @Value("${conductor.service.url:http://localhost:8081/api/conductor}")
    private String conductorServiceUrl;

    // Usa RestTemplate para llamar al Microservicio C
    public ConductorDTO crearConductor(ConductorDTO conductor) {
        // HTTP POST a http://localhost:8081/api/conductor
    }
}
```

### ✅ Transparencia para los Clientes

Los clientes GUI **SOLO** se comunican con el puerto **8080**:

```
Cliente React → http://localhost:8080/api/carro
Cliente React → http://localhost:8080/api/mantenimiento
Cliente React → http://localhost:8080/api/conductor  ← Proxy al puerto 8081

Cliente C# → http://localhost:8080/api/carro
Cliente C# → http://localhost:8080/api/mantenimiento
Cliente C# → http://localhost:8080/api/conductor  ← Proxy al puerto 8081
```

**NUNCA** los clientes llaman directamente al puerto 8081.

---

## 🚀 Cómo Ejecutar los Microservicios

### Paso 1: Bases de Datos

**Oracle (Microservicio A-B):**
```bash
# Ejecutar en DBeaver o SQL Developer
sqlplus SYSTEM/admin123@localhost:1521/xepdb1
@CREAR_BD_ORACLE.sql
```

**PostgreSQL (Microservicio C):**
```bash
# Opción 1: Crear con psql
psql -U postgres
CREATE DATABASE conductores_db;
\q

# Opción 2: Ejecutar script
psql -U postgres -f CREAR_BD_POSTGRESQL.sql
```

### Paso 2: Iniciar Microservicio A-B (Puerto 8080)

```bash
cd EmpresarialesBackend
mvnw.cmd clean install
mvnw.cmd spring-boot:run

# Debe mostrar:
# Started POCEmpleadoApplication on port 8080
```

### Paso 3: Iniciar Microservicio C (Puerto 8081)

```bash
cd EmpresarialesConductores
mvnw.cmd clean install
mvnw.cmd spring-boot:run

# Debe mostrar:
# Microservicio Conductores iniciado en puerto 8081
```

### Paso 4: Iniciar Cliente React (Puerto 5173)

```bash
cd EmpresarialesCliente
npm install
npm run dev

# Abrir: http://localhost:5173
```

### Paso 5: Iniciar Cliente C#

```bash
cd EmpresarialesClienteCSharp
dotnet run

# O desde Visual Studio: F5
```

---

## 🔗 Endpoints de los Microservicios

### Microservicio A-B (Puerto 8080)

**Carros:**
- `GET /api/carro` - Listar
- `POST /api/carro` - Crear
- `PUT /api/carro/{placa}` - Actualizar
- `DELETE /api/carro/{placa}` - Eliminar

**Mantenimientos:**
- `GET /api/mantenimiento` - Listar
- `POST /api/mantenimiento` - Crear
- `GET /api/mantenimiento/carro/{placa}` - Master-detail

**Conductores (Proxy):**
- `GET /api/conductor` - Listar (→ 8081)
- `POST /api/conductor` - Crear (→ 8081)
- `PUT /api/conductor/{cedula}` - Actualizar (→ 8081)
- `DELETE /api/conductor/{cedula}` - Eliminar (→ 8081)

### Microservicio C (Puerto 8081)

**Conductores:**
- `GET /api/conductor` - Listar
- `GET /api/conductor/{cedula}` - Obtener
- `POST /api/conductor` - Crear
- `PUT /api/conductor/{cedula}` - Actualizar
- `DELETE /api/conductor/{cedula}` - Eliminar
- `GET /api/conductor?activo=true` - Filtrar activos
- `GET /api/conductor?action=estadisticas` - Estadísticas

---

## 📊 Modelo de Datos

### Base de Datos Oracle (Microservicio A-B)

```sql
CARRO (Clase A - Maestro)
├── placa (PK)
├── marca, modelo, color
├── anio, precio
├── tipo_transmision
├── cedula_conductor  ← Referencia lógica al Microservicio C
└── [1:N] → MANTENIMIENTO

MANTENIMIENTO (Clase B - Detalle)
├── id (PK)
├── placa_carro (FK → CARRO)
├── fecha_mantenimiento
├── tipo_mantenimiento
└── costo
```

### Base de Datos PostgreSQL (Microservicio C)

```sql
CONDUCTOR (Clase C)
├── cedula (PK)
├── nombre, apellido
├── telefono
├── licencia_numero
├── fecha_nacimiento
├── salario
└── activo
```

---

## ✅ Cumplimiento de Requerimientos del PDF

| Requerimiento | Estado | Implementación |
|---------------|--------|----------------|
| Clase A (Maestro) | ✅ | Carro |
| Clase B (Detalle) | ✅ | Mantenimiento |
| Clase C (Microservicio independiente) | ✅ | Conductor |
| Relación @OneToMany/@ManyToOne | ✅ | Carro ←→ Mantenimiento |
| Base de datos relacional para A-B | ✅ | Oracle 21c XE |
| Base de datos independiente para C | ✅ | PostgreSQL |
| Servicios Web REST | ✅ | Ambos microservicios |
| Cliente 1 (diferente lenguaje) | ✅ | React + TypeScript |
| Cliente 2 (diferente lenguaje) | ✅ | C# .NET 8.0 |
| Proxy transparente | ✅ | ConductorProxyService |
| 5+ atributos con tipos requeridos | ✅ | Todos los modelos |
| CRUD completo | ✅ | Todas las entidades |

---

## 🎯 Ventajas de Esta Arquitectura

1. **Escalabilidad**: Cada microservicio puede escalar independientemente
2. **Mantenibilidad**: Cambios en Conductores no afectan Carros/Mantenimientos
3. **Tecnología flexible**: Oracle para uno, PostgreSQL para otro
4. **Despliegue independiente**: Puedes actualizar solo un microservicio
5. **Separación de responsabilidades**: Cada servicio tiene su propósito claro
6. **Tolerancia a fallos**: Si Conductores falla, Carros/Mantenimientos siguen funcionando

---

## 👥 Equipo de Desarrollo

- Juan David Reyes
- Julio David Suarez
- Sebastian Felipe Solano

**Institución:** Universidad de Ibagué - Facultad de Ingeniería
**Curso:** Desarrollo de Aplicaciones Empresariales 2025-A
