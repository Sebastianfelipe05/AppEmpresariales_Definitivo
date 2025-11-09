-- ============================================
-- SCRIPT COMPLETO PARA BASE DE DATOS ORACLE
-- Proyecto: Sistema de Gestión de Carros y Mantenimientos
-- Universidad de Ibagué - DAE 2025-B
-- ============================================

-- =====================================
-- PASO 1: CREAR USUARIO (como SYSTEM)
-- =====================================
-- Ejecuta esto conectado como SYSTEM/admin123

ALTER SESSION SET "_ORACLE_SCRIPT"=true;

-- Crear usuario DAE2025
CREATE USER DAE2025 IDENTIFIED BY DAE2025
DEFAULT TABLESPACE users
TEMPORARY TABLESPACE temp
QUOTA UNLIMITED ON users;

-- Otorgar permisos
GRANT CONNECT, RESOURCE, DBA TO DAE2025;
GRANT CREATE SESSION TO DAE2025;
GRANT CREATE TABLE TO DAE2025;
GRANT CREATE VIEW TO DAE2025;
GRANT CREATE SEQUENCE TO DAE2025;

-- Verificar creación
SELECT username, account_status FROM dba_users WHERE username = 'DAE2025';

COMMIT;

-- =====================================
-- PASO 2: CONECTARSE COMO DAE2025
-- =====================================
-- Cierra la conexión anterior y crea una nueva en DBeaver:
-- Usuario: DAE2025
-- Password: DAE2025
-- Database: XE

-- =====================================
-- PASO 3: CREAR TABLAS
-- =====================================
-- Ejecuta esto conectado como DAE2025/DAE2025

-- TABLA MAESTRA: CARRO
-- Representa vehículos disponibles en el concesionario
CREATE TABLE CARRO (
    -- Clave primaria
    placa VARCHAR2(10) PRIMARY KEY,

    -- Atributos heredados de Vehiculo
    marca VARCHAR2(50) NOT NULL,
    color VARCHAR2(30) NOT NULL,
    modelo VARCHAR2(50) NOT NULL,
    combustible VARCHAR2(20) NOT NULL,
    anio NUMBER(4) NOT NULL CHECK (anio >= 1950 AND anio <= 2025),
    estado VARCHAR2(20) NOT NULL CHECK (estado IN ('NUEVO', 'USADO', 'EXCELENTE', 'BUENO', 'REGULAR')),

    -- Atributos específicos de Carro
    numero_puertas NUMBER(1) CHECK (numero_puertas BETWEEN 2 AND 5),
    tiene_aire_acondicionado NUMBER(1) DEFAULT 0 CHECK (tiene_aire_acondicionado IN (0, 1)),
    precio NUMBER(15,2) CHECK (precio > 0),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tipo_transmision VARCHAR2(20) CHECK (tipo_transmision IN ('MANUAL', 'AUTOMATICA'))
);

-- TABLA DETALLE: MANTENIMIENTO
-- Representa mantenimientos realizados a los carros
CREATE TABLE MANTENIMIENTO (
    -- Clave primaria (auto-incremental)
    id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,

    -- Clave foránea hacia CARRO (relación @ManyToOne)
    placa_carro VARCHAR2(10) NOT NULL,

    -- Atributos del mantenimiento
    fecha_mantenimiento TIMESTAMP NOT NULL,
    kilometraje NUMBER(10) CHECK (kilometraje >= 0 AND kilometraje <= 1000000),
    tipo_mantenimiento VARCHAR2(50) NOT NULL
        CHECK (tipo_mantenimiento IN ('PREVENTIVO', 'CORRECTIVO', 'REVISION', 'CAMBIO_ACEITE', 'CAMBIO_LLANTAS', 'OTROS')),
    costo NUMBER(15,2) NOT NULL CHECK (costo >= 0),
    descripcion VARCHAR2(500) NOT NULL,
    proximo_mantenimiento TIMESTAMP,
    completado NUMBER(1) DEFAULT 0 CHECK (completado IN (0, 1)),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Constraint de clave foránea (ON DELETE CASCADE)
    CONSTRAINT fk_mantenimiento_carro
        FOREIGN KEY (placa_carro)
        REFERENCES CARRO(placa)
        ON DELETE CASCADE
);

-- =====================================
-- PASO 4: CREAR ÍNDICES
-- =====================================
-- Mejoran el rendimiento de las consultas

CREATE INDEX idx_mantenimiento_placa ON MANTENIMIENTO(placa_carro);
CREATE INDEX idx_mantenimiento_fecha ON MANTENIMIENTO(fecha_mantenimiento);
CREATE INDEX idx_mantenimiento_tipo ON MANTENIMIENTO(tipo_mantenimiento);
CREATE INDEX idx_carro_estado ON CARRO(estado);
CREATE INDEX idx_carro_marca ON CARRO(marca);
CREATE INDEX idx_carro_anio ON CARRO(anio);

-- =====================================
-- PASO 5: DATOS INICIALES
-- =====================================
-- Base de datos vacía - Sin datos de prueba
-- Los datos se agregarán desde la aplicación

COMMIT;

-- =====================================
-- PASO 6: CONSULTAS DE VERIFICACIÓN
-- =====================================

-- Ver todos los carros
SELECT * FROM CARRO ORDER BY placa;

-- Ver todos los mantenimientos
SELECT * FROM MANTENIMIENTO ORDER BY fecha_mantenimiento DESC;

-- CONSULTA MAESTRO-DETALLE (Requerida en el PDF)
-- Muestra carros con sus mantenimientos
SELECT
    c.placa,
    c.marca,
    c.modelo,
    c.anio,
    c.precio,
    m.tipo_mantenimiento,
    m.costo,
    m.fecha_mantenimiento,
    m.descripcion
FROM CARRO c
LEFT JOIN MANTENIMIENTO m ON c.placa = m.placa_carro
ORDER BY c.placa, m.fecha_mantenimiento DESC;

-- Contar registros
SELECT 'CARROS' AS TABLA, COUNT(*) AS CANTIDAD FROM CARRO
UNION ALL
SELECT 'MANTENIMIENTOS', COUNT(*) FROM MANTENIMIENTO;

-- Ver estructura de las tablas
DESCRIBE CARRO;
DESCRIBE MANTENIMIENTO;

-- Ver constraints
SELECT constraint_name, constraint_type, table_name
FROM user_constraints
WHERE table_name IN ('CARRO', 'MANTENIMIENTO')
ORDER BY table_name, constraint_type;

-- =====================================
-- FIN DEL SCRIPT
-- =====================================

-- Si todo salió bien, deberías ver:
-- - Usuario DAE2025 creado
-- - 2 tablas creadas (CARRO, MANTENIMIENTO)
-- - 6 índices creados
-- - Base de datos vacía (sin datos de prueba)
-- - Relación FK funcionando

-- Ahora puedes ejecutar tu aplicación Spring Boot
-- y agregar datos desde el frontend React o C#.
