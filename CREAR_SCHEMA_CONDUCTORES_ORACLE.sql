-- =============================================
-- Script de Creación de Esquema Oracle
-- Microservicio: Conductores
-- Usuario/Schema: CONDUCTORES_DB
-- Password: conductor123
-- =============================================

-- PASO 1: Conectarse como SYSTEM
-- sqlplus system/oracle@localhost:1521/XE

-- PASO 2: Crear el usuario CONDUCTORES_DB
DROP USER CONDUCTORES_DB CASCADE;

CREATE USER CONDUCTORES_DB IDENTIFIED BY conductor123
DEFAULT TABLESPACE USERS
TEMPORARY TABLESPACE TEMP
QUOTA UNLIMITED ON USERS;

-- PASO 3: Otorgar privilegios necesarios
GRANT CONNECT TO CONDUCTORES_DB;
GRANT RESOURCE TO CONDUCTORES_DB;
GRANT CREATE SESSION TO CONDUCTORES_DB;
GRANT CREATE TABLE TO CONDUCTORES_DB;
GRANT CREATE VIEW TO CONDUCTORES_DB;
GRANT CREATE SEQUENCE TO CONDUCTORES_DB;
GRANT CREATE TRIGGER TO CONDUCTORES_DB;

-- PASO 4: Conectarse como CONDUCTORES_DB
-- CONN CONDUCTORES_DB/conductor123@localhost:1521/XE

-- PASO 5: Crear la tabla CONDUCTOR
-- NOTA: Con spring.jpa.hibernate.ddl-auto=update, Hibernate creará automáticamente la tabla
-- Este script SQL es opcional, solo para referencia

CREATE TABLE CONDUCTOR (
    CEDULA VARCHAR2(20) PRIMARY KEY,
    NOMBRE VARCHAR2(100) NOT NULL,
    APELLIDO VARCHAR2(100) NOT NULL,
    TELEFONO VARCHAR2(20),
    LICENCIA_NUMERO VARCHAR2(50) NOT NULL UNIQUE,
    FECHA_NACIMIENTO TIMESTAMP NOT NULL,
    SALARIO NUMBER(10,2) NOT NULL CHECK (SALARIO > 0),
    ACTIVO NUMBER(1) DEFAULT 1 NOT NULL CHECK (ACTIVO IN (0,1)),
    FECHA_REGISTRO TIMESTAMP DEFAULT SYSTIMESTAMP NOT NULL
);

-- PASO 6: Crear comentarios en la tabla
COMMENT ON TABLE CONDUCTOR IS 'Tabla que almacena información de conductores';
COMMENT ON COLUMN CONDUCTOR.CEDULA IS 'Cédula del conductor (Llave Primaria)';
COMMENT ON COLUMN CONDUCTOR.NOMBRE IS 'Nombre del conductor';
COMMENT ON COLUMN CONDUCTOR.APELLIDO IS 'Apellido del conductor';
COMMENT ON COLUMN CONDUCTOR.TELEFONO IS 'Teléfono de contacto';
COMMENT ON COLUMN CONDUCTOR.LICENCIA_NUMERO IS 'Número de licencia de conducción (Único)';
COMMENT ON COLUMN CONDUCTOR.FECHA_NACIMIENTO IS 'Fecha de nacimiento del conductor';
COMMENT ON COLUMN CONDUCTOR.SALARIO IS 'Salario mensual del conductor';
COMMENT ON COLUMN CONDUCTOR.ACTIVO IS 'Indica si el conductor está activo (1=Activo, 0=Inactivo)';
COMMENT ON COLUMN CONDUCTOR.FECHA_REGISTRO IS 'Fecha de registro en el sistema';

-- PASO 7: Crear índices para optimizar búsquedas
CREATE INDEX IDX_CONDUCTOR_NOMBRE ON CONDUCTOR(NOMBRE);
CREATE INDEX IDX_CONDUCTOR_APELLIDO ON CONDUCTOR(APELLIDO);
CREATE INDEX IDX_CONDUCTOR_LICENCIA ON CONDUCTOR(LICENCIA_NUMERO);
CREATE INDEX IDX_CONDUCTOR_ACTIVO ON CONDUCTOR(ACTIVO);
CREATE INDEX IDX_CONDUCTOR_SALARIO ON CONDUCTOR(SALARIO);
CREATE INDEX IDX_CONDUCTOR_FECHA_REG ON CONDUCTOR(FECHA_REGISTRO);

-- PASO 8: Insertar datos de prueba (OPCIONAL - DataInitializer.java lo hace automáticamente)
INSERT INTO CONDUCTOR (CEDULA, NOMBRE, APELLIDO, TELEFONO, LICENCIA_NUMERO, FECHA_NACIMIENTO, SALARIO, ACTIVO, FECHA_REGISTRO)
VALUES ('1098765432', 'Carlos', 'Rodríguez', '3201234567', 'LIC-2020-001', TIMESTAMP '1985-05-15 00:00:00', 2500000.00, 1, SYSTIMESTAMP);

INSERT INTO CONDUCTOR (CEDULA, NOMBRE, APELLIDO, TELEFONO, LICENCIA_NUMERO, FECHA_NACIMIENTO, SALARIO, ACTIVO, FECHA_REGISTRO)
VALUES ('1087654321', 'María', 'González', '3109876543', 'LIC-2019-002', TIMESTAMP '1990-08-22 00:00:00', 3000000.00, 1, SYSTIMESTAMP);

INSERT INTO CONDUCTOR (CEDULA, NOMBRE, APELLIDO, TELEFONO, LICENCIA_NUMERO, FECHA_NACIMIENTO, SALARIO, ACTIVO, FECHA_REGISTRO)
VALUES ('1076543210', 'Juan', 'Martínez', '3157654321', 'LIC-2021-003', TIMESTAMP '1988-03-10 00:00:00', 2800000.00, 0, SYSTIMESTAMP);

COMMIT;

-- PASO 9: Verificar la creación
SELECT * FROM CONDUCTOR;
SELECT COUNT(*) AS TOTAL_CONDUCTORES FROM CONDUCTOR;

-- =============================================
-- CONSULTAS ÚTILES
-- =============================================

-- Ver todos los conductores activos
SELECT * FROM CONDUCTOR WHERE ACTIVO = 1 ORDER BY NOMBRE;

-- Ver estadísticas de salarios
SELECT
    COUNT(*) AS TOTAL,
    SUM(CASE WHEN ACTIVO = 1 THEN 1 ELSE 0 END) AS ACTIVOS,
    SUM(CASE WHEN ACTIVO = 0 THEN 1 ELSE 0 END) AS INACTIVOS,
    AVG(SALARIO) AS SALARIO_PROMEDIO,
    MAX(SALARIO) AS SALARIO_MAXIMO,
    MIN(SALARIO) AS SALARIO_MINIMO
FROM CONDUCTOR;

-- Buscar conductor por cédula
SELECT * FROM CONDUCTOR WHERE CEDULA = '1098765432';

-- Buscar conductor por licencia
SELECT * FROM CONDUCTOR WHERE LICENCIA_NUMERO = 'LIC-2020-001';

-- Conductores con salario mayor a 2.5 millones
SELECT * FROM CONDUCTOR WHERE SALARIO > 2500000 ORDER BY SALARIO DESC;

-- Ver estructura de la tabla
DESC CONDUCTOR;

-- =============================================
-- COMANDOS ÚTILES DE ORACLE
-- =============================================

-- Conectarse a SQL*Plus
-- sqlplus CONDUCTORES_DB/conductor123@localhost:1521/XE

-- Ver todas las tablas del usuario actual
-- SELECT TABLE_NAME FROM USER_TABLES;

-- Ver todos los índices
-- SELECT INDEX_NAME, TABLE_NAME FROM USER_INDEXES;

-- Ver todas las constraints
-- SELECT CONSTRAINT_NAME, CONSTRAINT_TYPE, TABLE_NAME FROM USER_CONSTRAINTS;

-- Ver el tamaño de las tablas
-- SELECT TABLE_NAME, NUM_ROWS FROM USER_TABLES;

-- Salir de SQL*Plus
-- EXIT;

-- =============================================
-- NOTAS IMPORTANTES
-- =============================================
-- 1. Oracle XE usa el puerto 1521 por defecto
-- 2. Usuario administrador: system / oracle
-- 3. Usuario para microservicio: CONDUCTORES_DB / conductor123
-- 4. SID: XE
-- 5. La aplicación Spring Boot creará automáticamente la tabla
--    si usas spring.jpa.hibernate.ddl-auto=update
-- 6. Este script es opcional para crear manualmente la estructura
-- 7. Oracle usa NUMBER(1) para booleanos: 1=true, 0=false
-- 8. Oracle usa SYSTIMESTAMP en lugar de CURRENT_TIMESTAMP

-- =============================================
-- ARQUITECTURA DE MICROSERVICIOS
-- =============================================
-- Microservicio Principal (Puerto 8080):
--   - Usuario Oracle: SYSTEM (o tu usuario actual)
--   - Tablas: CARRO, MANTENIMIENTO
--
-- Microservicio Conductores (Puerto 8081):
--   - Usuario Oracle: CONDUCTORES_DB
--   - Tablas: CONDUCTOR
--
-- Ambos microservicios usan la misma instancia de Oracle (XE)
-- pero con usuarios/esquemas separados para mantener la separación lógica
