# Microservicio Conductores

Microservicio independiente para la gestión de conductores del sistema de gestión de vehículos.

## Tecnologías

- **Java 17**
- **Spring Boot 3.5.5**
- **PostgreSQL** (Base de datos independiente)
- **JPA/Hibernate**
- **Maven**
- **Lombok**

## Configuración

### Base de Datos

- **Puerto:** 5432 (PostgreSQL)
- **Base de datos:** `conductores_db`
- **Usuario:** `postgres`
- **Contraseña:** `postgres`

### Servidor

- **Puerto:** 8081
- **Autenticación:** HTTP Basic (admin/admin)

## Instalación de PostgreSQL

### Windows

1. Descargar PostgreSQL desde: https://www.postgresql.org/download/windows/
2. Ejecutar el instalador
3. Configurar contraseña para el usuario `postgres`
4. Asegurar que el puerto sea 5432

### Crear Base de Datos

```bash
# Opción 1: Ejecutar el script SQL
psql -U postgres -f CREAR_BD_POSTGRESQL.sql

# Opción 2: Crear manualmente
psql -U postgres
CREATE DATABASE conductores_db;
\q
```

## Ejecutar el Microservicio

```bash
# Windows
mvnw.cmd clean install
mvnw.cmd spring-boot:run

# El microservicio se iniciará en http://localhost:8081
```

## Endpoints API

### Health Check
```
GET /api/conductor/healthCheck
```

### CRUD Conductores

```
POST   /api/conductor              - Crear conductor
GET    /api/conductor/{cedula}     - Obtener por cédula
GET    /api/conductor              - Listar todos
PUT    /api/conductor/{cedula}     - Actualizar
DELETE /api/conductor/{cedula}     - Eliminar
```

### Búsquedas y Filtros

```
GET /api/conductor?nombre={nombre}        - Buscar por nombre
GET /api/conductor?apellido={apellido}    - Buscar por apellido
GET /api/conductor?activo=true            - Listar activos
GET /api/conductor?licencia={licencia}    - Buscar por licencia
GET /api/conductor?action=estadisticas    - Obtener estadísticas
```

### Filtros Avanzados

```
GET /api/conductor/filtro/salario?min={min}&max={max}
GET /api/conductor/filtro/fecha?inicio={inicio}&fin={fin}
```

## Modelo de Datos

**Conductor:**
- cedula (String) - PK
- nombre (String)
- apellido (String)
- telefono (String)
- licenciaNumero (String) - UNIQUE
- fechaNacimiento (LocalDateTime)
- salario (Double)
- activo (Boolean)
- fechaRegistro (LocalDateTime)

## Datos de Prueba

El sistema carga automáticamente 3 conductores de prueba:
1. Carlos Rodríguez (1098765432)
2. María González (1087654321)
3. Juan Martínez (1076543210)

## Autenticación

Todos los endpoints (excepto healthCheck) requieren HTTP Basic Auth:
- **Usuario:** admin
- **Contraseña:** admin

## Ejemplo de Uso

### Crear Conductor (POST)

```bash
curl -X POST http://localhost:8081/api/conductor \
  -u admin:admin \
  -H "Content-Type: application/json" \
  -d '{
    "cedula": "1234567890",
    "nombre": "Pedro",
    "apellido": "Pérez",
    "telefono": "3001234567",
    "licenciaNumero": "LIC-2024-004",
    "fechaNacimiento": "1992-06-15T00:00:00",
    "salario": 3200000.0,
    "activo": true
  }'
```

### Obtener Conductor (GET)

```bash
curl -X GET http://localhost:8081/api/conductor/1234567890 \
  -u admin:admin
```

### Listar Todos (GET)

```bash
curl -X GET http://localhost:8081/api/conductor \
  -u admin:admin
```

## Logs

Los logs se muestran en consola con nivel DEBUG para la aplicación.

## Arquitectura

Este microservicio es parte de una arquitectura de microservicios:

```
Cliente GUI 1 (React) ──┐
                        ├──> Microservicio A-B (Carro-Mantenimiento:8080) ──> Oracle DB
Cliente GUI 2 (C#) ────┘           │
                                   │
                                   └──> Microservicio C (Conductores:8081) ──> PostgreSQL
```

Los clientes **NO** se comunican directamente con este microservicio. Todas las peticiones pasan a través del microservicio principal (puerto 8080).
