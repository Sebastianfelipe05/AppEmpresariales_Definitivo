-- =============================================
-- Script de Creación de Base de Datos PostgreSQL
-- Microservicio: Conductores
-- Base de Datos: conductores_db
-- Usuario: postgres / postgres
-- =============================================

-- PASO 1: Conectarse a PostgreSQL como superusuario
-- psql -U postgres

-- PASO 2: Crear la base de datos
CREATE DATABASE conductores_db
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'Spanish_Colombia.1252'
    LC_CTYPE = 'Spanish_Colombia.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

COMMENT ON DATABASE conductores_db
    IS 'Base de datos para el microservicio de Conductores - Proyecto Empresariales';

-- PASO 3: Conectarse a la base de datos conductores_db
-- \c conductores_db

-- PASO 4: Crear la tabla CONDUCTOR
-- NOTA: Con spring.jpa.hibernate.ddl-auto=update, Hibernate creará automáticamente la tabla
-- Este script SQL es opcional, solo para referencia

CREATE TABLE IF NOT EXISTS CONDUCTOR (
    cedula VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    licencia_numero VARCHAR(50) NOT NULL UNIQUE,
    fecha_nacimiento TIMESTAMP NOT NULL,
    salario NUMERIC(10,2) NOT NULL CHECK (salario > 0),
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- PASO 5: Crear índices para optimizar búsquedas
CREATE INDEX IF NOT EXISTS idx_conductor_nombre ON CONDUCTOR(nombre);
CREATE INDEX IF NOT EXISTS idx_conductor_apellido ON CONDUCTOR(apellido);
CREATE INDEX IF NOT EXISTS idx_conductor_licencia ON CONDUCTOR(licencia_numero);
CREATE INDEX IF NOT EXISTS idx_conductor_activo ON CONDUCTOR(activo);
CREATE INDEX IF NOT EXISTS idx_conductor_salario ON CONDUCTOR(salario);
CREATE INDEX IF NOT EXISTS idx_conductor_fecha_registro ON CONDUCTOR(fecha_registro);

-- PASO 6: Comentarios en la tabla y columnas
COMMENT ON TABLE CONDUCTOR IS 'Tabla que almacena información de conductores';
COMMENT ON COLUMN CONDUCTOR.cedula IS 'Cédula del conductor (Llave Primaria)';
COMMENT ON COLUMN CONDUCTOR.nombre IS 'Nombre del conductor';
COMMENT ON COLUMN CONDUCTOR.apellido IS 'Apellido del conductor';
COMMENT ON COLUMN CONDUCTOR.telefono IS 'Teléfono de contacto';
COMMENT ON COLUMN CONDUCTOR.licencia_numero IS 'Número de licencia de conducción (Único)';
COMMENT ON COLUMN CONDUCTOR.fecha_nacimiento IS 'Fecha de nacimiento del conductor';
COMMENT ON COLUMN CONDUCTOR.salario IS 'Salario mensual del conductor';
COMMENT ON COLUMN CONDUCTOR.activo IS 'Indica si el conductor está activo';
COMMENT ON COLUMN CONDUCTOR.fecha_registro IS 'Fecha de registro en el sistema';

-- PASO 7: Insertar datos de prueba (OPCIONAL - DataInitializer.java lo hace automáticamente)
INSERT INTO CONDUCTOR (cedula, nombre, apellido, telefono, licencia_numero, fecha_nacimiento, salario, activo, fecha_registro)
VALUES
    ('1098765432', 'Carlos', 'Rodríguez', '3201234567', 'LIC-2020-001', '1985-05-15 00:00:00', 2500000.00, true, CURRENT_TIMESTAMP),
    ('1087654321', 'María', 'González', '3109876543', 'LIC-2019-002', '1990-08-22 00:00:00', 3000000.00, true, CURRENT_TIMESTAMP),
    ('1076543210', 'Juan', 'Martínez', '3157654321', 'LIC-2021-003', '1988-03-10 00:00:00', 2800000.00, false, CURRENT_TIMESTAMP)
ON CONFLICT (cedula) DO NOTHING;

-- PASO 8: Verificar la creación
SELECT * FROM CONDUCTOR;
SELECT COUNT(*) as total_conductores FROM CONDUCTOR;

-- =============================================
-- CONSULTAS ÚTILES
-- =============================================

-- Ver todos los conductores activos
SELECT * FROM CONDUCTOR WHERE activo = true ORDER BY nombre;

-- Ver estadísticas de salarios
SELECT
    COUNT(*) as total,
    COUNT(CASE WHEN activo = true THEN 1 END) as activos,
    COUNT(CASE WHEN activo = false THEN 1 END) as inactivos,
    AVG(salario) as salario_promedio,
    MAX(salario) as salario_maximo,
    MIN(salario) as salario_minimo
FROM CONDUCTOR;

-- Buscar conductor por cédula
SELECT * FROM CONDUCTOR WHERE cedula = '1098765432';

-- Buscar conductor por licencia
SELECT * FROM CONDUCTOR WHERE licencia_numero = 'LIC-2020-001';

-- Conductores con salario mayor a 2.5 millones
SELECT * FROM CONDUCTOR WHERE salario > 2500000 ORDER BY salario DESC;

-- =============================================
-- COMANDOS ÚTILES DE POSTGRESQL
-- =============================================

-- Listar todas las bases de datos
-- \l

-- Conectarse a una base de datos
-- \c conductores_db

-- Listar todas las tablas
-- \dt

-- Describir una tabla
-- \d CONDUCTOR

-- Ver usuarios
-- \du

-- Salir de psql
-- \q

-- =============================================
-- NOTAS IMPORTANTES
-- =============================================
-- 1. PostgreSQL por defecto usa el puerto 5432
-- 2. Usuario por defecto: postgres
-- 3. Contraseña: La que configuraste durante la instalación
-- 4. La aplicación Spring Boot creará automáticamente la tabla
--    si usas spring.jpa.hibernate.ddl-auto=update
-- 5. Este script es opcional para crear manualmente la estructura
