INDICES
-- ============================================================
-- ÍNDICES ESENCIALES (Solo identificadores)
-- ============================================================

-- CLIENTE: Búsquedas por identificadores únicos
CREATE INDEX IX_CLIENTE_nit      ON dbo.CLIENTE(nit_ci);
CREATE INDEX IX_CLIENTE_nombre   ON dbo.CLIENTE(nombre_razon_social);
CREATE INDEX IX_CLIENTE_email    ON dbo.CLIENTE(email);
CREATE INDEX IX_CLIENTE_telefono ON dbo.CLIENTE(telefono);
CREATE INDEX IX_CLIENTE_ci       ON dbo.CLIENTE(nit_ci) WHERE tipo_cliente = 'N';  -- CI para naturales

-- CONTRATO: Búsqueda por cliente
CREATE INDEX IX_CONTRATO_cliente ON dbo.CONTRATO(id_cliente);

-- USUARIO: Búsqueda por email
CREATE INDEX IX_USUARIO_email ON dbo.USUARIO(email);

PRINT 'Índices de identificación creados correctamente.';
GO







JOBS
-- ============================================================
-- CONFIGURACIÓN DE ALERTAS (SQL Server Agent Alerts)
-- ============================================================

-- Alerta 1: Error grave en la base de datos (severidad 16+)
IF NOT EXISTS (SELECT 1 FROM msdb.dbo.sysalerts WHERE name = 'FIS_Error_Grave_BD')
BEGIN
    EXEC msdb.dbo.sp_add_alert
        @name = N'FIS_Error_Grave_BD',
        @message_id = 0,
        @severity = 16,
        @notification_message = N'Se ha producido un error grave en la base de datos FullInternetServices. Revisar inmediatamente.',
        @job_name = N'FIS_Control_Mora_Diario';
END
GO

-- Alerta 2: Muchos intentos fallidos de login (más de 10 en 5 min)
-- Se configura mediante un operador de correo
IF NOT EXISTS (SELECT 1 FROM msdb.dbo.sysoperators WHERE name = 'Admin_FIS')
BEGIN
    EXEC msdb.dbo.sp_add_operator
        @name = N'Admin_FIS',
        @enabled = 1,
        @email_address = N'admin@fullinternetservices.bo',
        @pager_days = 127;  -- Todos los días
END
GO

PRINT 'Alertas configuradas correctamente.';
GO

SELECT 
    name AS Nombre_Alerta,
    event_source AS Origen,
    severity AS Severity_Vigilada,
    enabled AS Activada,
    last_occurrence_date AS Ultima_Vez_Que_Ocurrio,
    occurrence_count AS Total_De_Veces_Disparada
FROM msdb.dbo.sysalerts
WHERE name LIKE 'FIS%';


SELECT 
    name AS Nombre_Operador,
    enabled AS Activo,
    email_address AS Correo_Electronico,
    last_email_date AS Ultimo_Correo_Enviado
FROM msdb.dbo.sysoperators
WHERE name = 'Admin_FIS';









PROCESO ALMACENADO CLIENTE

USE FullInternetServices;
GO

-- ============================================================
-- CLIENTE: INSERT
-- ============================================================
CREATE OR ALTER PROCEDURE sp_cliente_insert
    @tipo_cliente        CHAR(1),
    @nombre_razon_social VARCHAR(200),
    @nit_ci              VARCHAR(20),
    @email               VARCHAR(150),
    @telefono            VARCHAR(20),
    @direccion           VARCHAR(500),
    @ciudad              VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @codigo VARCHAR(20);
    SET @codigo = 'CLI-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-' + 
                  RIGHT('000' + CAST(ISNULL((SELECT MAX(id_cliente) FROM dbo.CLIENTE), 0) + 1 AS VARCHAR), 4);
    
    INSERT INTO dbo.CLIENTE (tipo_cliente, codigo_cliente, nombre_razon_social, nit_ci, email, telefono, direccion, ciudad)
    VALUES (@tipo_cliente, @codigo, @nombre_razon_social, @nit_ci, @email, @telefono, @direccion, @ciudad);
    
    SELECT SCOPE_IDENTITY() AS id_cliente, @codigo AS codigo_cliente;
END
GO

-- ============================================================
-- CLIENTE: SELECT (por filtros)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_cliente_select
    @id_cliente INT = NULL,
    @nit_ci     VARCHAR(20) = NULL,
    @email      VARCHAR(150) = NULL,
    @nombre     VARCHAR(200) = NULL,
    @activo     BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT id_cliente, tipo_cliente, codigo_cliente, nombre_razon_social, 
           nit_ci, email, telefono, direccion, ciudad, activo, fecha_registro
    FROM dbo.CLIENTE
    WHERE (@id_cliente IS NULL OR id_cliente = @id_cliente)
      AND (@nit_ci IS NULL OR nit_ci = @nit_ci)
      AND (@email IS NULL OR email = @email)
      AND (@nombre IS NULL OR nombre_razon_social LIKE '%' + @nombre + '%')
      AND activo = @activo
    ORDER BY nombre_razon_social;
END
GO

-- ============================================================
-- CLIENTE: UPDATE
-- ============================================================
CREATE OR ALTER PROCEDURE sp_cliente_update
    @id_cliente          INT,
    @nombre_razon_social VARCHAR(200) = NULL,
    @email               VARCHAR(150) = NULL,
    @telefono            VARCHAR(20) = NULL,
    @direccion           VARCHAR(500) = NULL,
    @ciudad              VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM dbo.CLIENTE WHERE id_cliente = @id_cliente)
    BEGIN
        RAISERROR('Cliente no encontrado.', 16, 1);
        RETURN;
    END
    
    UPDATE dbo.CLIENTE
    SET nombre_razon_social = ISNULL(@nombre_razon_social, nombre_razon_social),
        email               = ISNULL(@email, email),
        telefono            = ISNULL(@telefono, telefono),
        direccion           = ISNULL(@direccion, direccion),
        ciudad              = ISNULL(@ciudad, ciudad)
    WHERE id_cliente = @id_cliente;
    
    SELECT 'Cliente actualizado correctamente.' AS mensaje;
END
GO

-- ============================================================
-- CLIENTE: DELETE (lógico)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_cliente_delete
    @id_cliente INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar contratos activos
    IF EXISTS (SELECT 1 FROM dbo.CONTRATO WHERE id_cliente = @id_cliente AND estado = 'activo')
    BEGIN
        RAISERROR('No se puede desactivar el cliente. Tiene contratos activos.', 16, 1);
        RETURN;
    END
    
    UPDATE dbo.CLIENTE SET activo = 0 WHERE id_cliente = @id_cliente;
    
    SELECT 'Cliente desactivado correctamente.' AS mensaje;
END
GO

SELECT 
    name AS Nombre_Procedimiento,
    create_date AS Fecha_Creacion,
    modify_date AS Ultima_Modificacion
FROM sys.procedures
ORDER BY name;






PROCESO ALMACENADO PAGOS
-- ============================================================
-- PAGO: INSERT (Registrar pago con conceptos JSON)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_pago_insert
    @id_contrato    INT,
    @id_cajero      INT,
    @metodo_pago    VARCHAR(20),
    @periodo_mes    TINYINT,
    @periodo_anio   SMALLINT,
    @conceptos_json NVARCHAR(MAX)     -- '[{"c":"Internet","p":250}]'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @monto_total   DECIMAL(10,2);
    DECLARE @monto_mora    DECIMAL(10,2) = 0;
    DECLARE @numero_recibo VARCHAR(30);
    DECLARE @id_pago       INT;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar contrato activo
        IF NOT EXISTS (SELECT 1 FROM dbo.CONTRATO WHERE id_contrato = @id_contrato AND estado = 'activo')
        BEGIN
            RAISERROR('El contrato no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        -- Validar que no exista pago del mismo período
        IF EXISTS (SELECT 1 FROM dbo.PAGO 
                   WHERE id_contrato = @id_contrato 
                     AND periodo_mes = @periodo_mes 
                     AND periodo_anio = @periodo_anio 
                     AND anulado = 0)
        BEGIN
            RAISERROR('Ya existe un pago registrado para ese período.', 16, 1);
            RETURN;
        END
        
        -- Calcular monto desde JSON
        SELECT @monto_total = SUM(CAST(JSON_VALUE(value, '$.p') AS DECIMAL(10,2)))
        FROM OPENJSON(@conceptos_json);
        
        -- Mora si día > 11
        IF DAY(GETDATE()) > 11
            SET @monto_mora = ROUND(@monto_total * 0.10, 2);
        
        -- Generar recibo
        SET @numero_recibo = 'RC-' + CAST(@periodo_anio AS VARCHAR) + '-' + 
            RIGHT('000000' + CAST(ISNULL((SELECT MAX(id_pago) FROM dbo.PAGO), 0) + 1 AS VARCHAR), 6);
        
        -- Insertar pago
        INSERT INTO dbo.PAGO (id_contrato, id_cajero, metodo_pago, numero_recibo,
            periodo_mes, periodo_anio, monto_total, monto_mora, conceptos)
        VALUES (@id_contrato, @id_cajero, @metodo_pago, @numero_recibo,
            @periodo_mes, @periodo_anio, @monto_total + @monto_mora, @monto_mora, @conceptos_json);
        
        SET @id_pago = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        SELECT @id_pago AS id_pago, @numero_recibo AS numero_recibo,
               @monto_total AS monto, @monto_mora AS mora,
               @monto_total + @monto_mora AS total;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END
GO

-- ============================================================
-- PAGO: SELECT
-- ============================================================
CREATE OR ALTER PROCEDURE sp_pago_select
    @id_contrato INT = NULL,
    @id_pago     INT = NULL,
    @anulado     BIT = 0,
    @desde       DATE = NULL,
    @hasta       DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT p.id_pago, p.id_contrato, c.nombre_razon_social AS cliente,
           p.metodo_pago, p.numero_recibo, p.periodo_mes, p.periodo_anio,
           p.fecha_pago, p.monto_total, p.monto_mora, p.anulado
    FROM dbo.PAGO p
    INNER JOIN dbo.CONTRATO co ON co.id_contrato = p.id_contrato
    INNER JOIN dbo.CLIENTE c ON c.id_cliente = co.id_cliente
    WHERE (@id_contrato IS NULL OR p.id_contrato = @id_contrato)
      AND (@id_pago IS NULL OR p.id_pago = @id_pago)
      AND p.anulado = @anulado
      AND (@desde IS NULL OR p.fecha_pago >= @desde)
      AND (@hasta IS NULL OR p.fecha_pago <= @hasta)
    ORDER BY p.fecha_pago DESC;
END
GO

-- ============================================================
-- PAGO: UPDATE (Anular pago - no se modifica, solo se anula)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_pago_anular
    @id_pago          INT,
    @id_usuario       INT,
    @motivo_anulacion NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT 1 FROM dbo.PAGO WHERE id_pago = @id_pago AND anulado = 0)
    BEGIN
        RAISERROR('El pago no existe o ya fue anulado.', 16, 1);
        RETURN;
    END
    
    UPDATE dbo.PAGO
    SET anulado = 1, motivo_anulacion = @motivo_anulacion, fecha_anulacion = GETDATE()
    WHERE id_pago = @id_pago;
    
    SELECT 'Pago anulado correctamente.' AS mensaje;
END
GO

-- ============================================================
-- PAGO: DELETE (No se eliminan pagos, solo se anulan - política)
-- ============================================================
CREATE OR ALTER PROCEDURE sp_pago_delete
    @id_pago INT
AS
BEGIN
    RAISERROR('No se permite eliminar pagos. Use la anulación.', 16, 1);
END
GO

-- Ver todos los pagos realizados
EXEC sp_pago_select;
GO







TABLAS (8 TABLAS)
-- ============================================================
--  FULL INTERNET SERVICES - TABLAS OPTIMIZADAS v3.0
--  8 Tablas (Reducción del 50% vs original de 16)
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FullInternetServices')
    CREATE DATABASE FullInternetServices;
GO

USE FullInternetServices;
GO

-- ============================================================
-- 1. ROL
-- ============================================================
CREATE TABLE dbo.ROL (
    id_rol       TINYINT IDENTITY(1,1) PRIMARY KEY,
    nombre_rol   VARCHAR(20) NOT NULL UNIQUE,
    activo       BIT NOT NULL DEFAULT 1
);

-- ============================================================
-- 2. USUARIO (absorbe TÉCNICO)
-- ============================================================
CREATE TABLE dbo.USUARIO (
    id_usuario        INT IDENTITY(1,1) PRIMARY KEY,
    id_rol            TINYINT NOT NULL REFERENCES dbo.ROL(id_rol),
    username          VARCHAR(50) NOT NULL UNIQUE,
    password_hash     VARCHAR(255) NOT NULL,
    email             VARCHAR(150) NOT NULL UNIQUE,
    nombres           VARCHAR(100) NOT NULL,
    apellidos         VARCHAR(100) NOT NULL,
    especialidad      VARCHAR(100) NULL,           -- solo para rol Técnico
    activo            BIT NOT NULL DEFAULT 1,
    intentos_fallidos TINYINT NOT NULL DEFAULT 0,
    bloqueado_hasta   DATETIME2 NULL,
    created_at        DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- 3. CLIENTE (tipo_cliente como CHAR)
-- ============================================================
CREATE TABLE dbo.CLIENTE (
    id_cliente          INT IDENTITY(1,1) PRIMARY KEY,
    tipo_cliente        CHAR(1) NOT NULL CHECK (tipo_cliente IN ('N','J')),
    codigo_cliente      VARCHAR(20) NOT NULL UNIQUE,
    nombre_razon_social VARCHAR(200) NOT NULL,
    nit_ci              VARCHAR(20) NOT NULL UNIQUE,
    email               VARCHAR(150) NOT NULL UNIQUE,
    telefono            VARCHAR(20) NOT NULL,
    direccion           VARCHAR(500) NOT NULL,
    ciudad              VARCHAR(100) NOT NULL,
    activo              BIT NOT NULL DEFAULT 1,
    fecha_registro      DATE NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- 4. PLAN_SERVICIO (absorbe TIPO_CONTRATO)
-- ============================================================
CREATE TABLE dbo.PLAN_SERVICIO (
    id_plan               INT IDENTITY(1,1) PRIMARY KEY,
    nombre_plan           VARCHAR(100) NOT NULL,
    tipo_servicio         VARCHAR(30) NOT NULL CHECK (tipo_servicio IN 
                            ('Internet Residencial','Internet Empresarial',
                             'Hosting Web','Dominio .bo','Correo Corporativo')),
    velocidad_bajada_mbps DECIMAL(8,2) NULL,
    velocidad_subida_mbps DECIMAL(8,2) NULL,
    precio_mensual        DECIMAL(10,2) NOT NULL,
    activo                BIT NOT NULL DEFAULT 1
);

-- ============================================================
-- 5. CONTRATO
-- ============================================================
CREATE TABLE dbo.CONTRATO (
    id_contrato         INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente          INT NOT NULL REFERENCES dbo.CLIENTE(id_cliente),
    id_plan             INT NOT NULL REFERENCES dbo.PLAN_SERVICIO(id_plan),
    id_usuario_registro INT NOT NULL REFERENCES dbo.USUARIO(id_usuario),
    numero_contrato     VARCHAR(30) NOT NULL UNIQUE,
    fecha_inicio        DATE NOT NULL,
    fecha_fin           DATE NOT NULL,
    estado              VARCHAR(20) NOT NULL DEFAULT 'activo' 
                         CHECK (estado IN ('activo','suspendido','finalizado','cancelado')),
    created_at          DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_CONTRATO_fechas CHECK (fecha_fin > fecha_inicio)
);

-- ============================================================
-- 6. PAGO (absorbe METODO_PAGO y DETALLE_PAGO)
-- ============================================================
CREATE TABLE dbo.PAGO (
    id_pago          INT IDENTITY(1,1) PRIMARY KEY,
    id_contrato      INT NOT NULL REFERENCES dbo.CONTRATO(id_contrato),
    id_cajero        INT NOT NULL REFERENCES dbo.USUARIO(id_usuario),
    metodo_pago      VARCHAR(20) NOT NULL CHECK (metodo_pago IN 
                      ('Efectivo','QR','Transferencia','Tarjeta Débito','Tarjeta Crédito')),
    numero_recibo    VARCHAR(30) NOT NULL UNIQUE,
    periodo_mes      TINYINT NOT NULL CHECK (periodo_mes BETWEEN 1 AND 12),
    periodo_anio     SMALLINT NOT NULL,
    fecha_pago       DATETIME2 NOT NULL DEFAULT GETDATE(),
    monto_total      DECIMAL(10,2) NOT NULL,
    monto_mora       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    conceptos        NVARCHAR(MAX) NULL,     -- JSON con desglose
    anulado          BIT NOT NULL DEFAULT 0,
    motivo_anulacion NVARCHAR(500) NULL,
    fecha_anulacion  DATETIME2 NULL
);

-- ============================================================
-- 7. MORA (relación 1:1 con CONTRATO)
-- ============================================================
CREATE TABLE dbo.MORA (
    id_mora              INT IDENTITY(1,1) PRIMARY KEY,
    id_contrato          INT NOT NULL UNIQUE REFERENCES dbo.CONTRATO(id_contrato),
    en_mora              BIT NOT NULL DEFAULT 0,
    fecha_inicio_mora    DATE NULL,
    monto_adeudado       DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    servicio_cortado     BIT NOT NULL DEFAULT 0,
    fecha_regularizacion DATE NULL,
    updated_at           DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- 8. RECLAMO (absorbe TIPO_RECLAMO, ESTADO_RECLAMO, ASIGNACION, GRABACION)
-- ============================================================
CREATE TABLE dbo.RECLAMO (
    id_reclamo           INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente           INT NOT NULL REFERENCES dbo.CLIENTE(id_cliente),
    id_contrato          INT NULL REFERENCES dbo.CONTRATO(id_contrato),
    id_tecnico           INT NULL REFERENCES dbo.USUARIO(id_usuario),
    id_usuario_registro  INT NOT NULL REFERENCES dbo.USUARIO(id_usuario),
    numero_reclamo       VARCHAR(30) NOT NULL UNIQUE,
    tipo_reclamo         VARCHAR(10) NOT NULL CHECK (tipo_reclamo IN ('Leve','Medio','Complejo')),
    estado               VARCHAR(15) NOT NULL DEFAULT 'Recepcionado' 
                          CHECK (estado IN ('Recepcionado','En Proceso','Observado','Cerrado')),
    descripcion_problema NVARCHAR(MAX) NOT NULL,
    solucion_aplicada    NVARCHAR(MAX) NULL,
    prioridad            TINYINT NOT NULL DEFAULT 3 CHECK (prioridad BETWEEN 1 AND 5),
    canal_entrada        VARCHAR(10) NOT NULL DEFAULT 'telefono' 
                          CHECK (canal_entrada IN ('telefono','web','presencial','app')),
    fecha_apertura       DATETIME2 NOT NULL DEFAULT GETDATE(),
    fecha_cierre         DATETIME2 NULL,
    calificacion         TINYINT NULL CHECK (calificacion BETWEEN 1 AND 5),
    tiempo_respuesta_min INT NULL,
    ruta_audio           VARCHAR(500) NULL,
    duracion_audio_seg   INT NULL,
    hash_sha256          VARCHAR(64) NULL
);

-- =================================================

PRINT '=============================================';
PRINT '  8 Tablas creadas exitosamente            ';
PRINT '  ROL, USUARIO, CLIENTE, PLAN_SERVICIO     ';
PRINT '  CONTRATO, PAGO, MORA, RECLAMO            ';
PRINT '=============================================';
GO

SELECT 
    t.name AS Tabla,
    i.name AS Nombre_Indice,
    i.type_desc AS Tipo_Indice
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('CLIENTE', 'CONTRATO', 'USUARIO');











TRIGERS

-- ============================================================
-- TRIGGER 1: Reactivar servicio tras pago válido
-- Acción: Después de INSERT en PAGO
-- Propósito: Si el contrato estaba en mora/suspendido, lo reactiva
-- ============================================================
CREATE OR ALTER TRIGGER trg_pago_reactivar_servicio
ON dbo.PAGO
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Solo procesar pagos no anulados
    IF NOT EXISTS (SELECT 1 FROM inserted WHERE anulado = 0)
        RETURN;
    
    -- Levantar mora
    UPDATE m
    SET en_mora = 0, 
        monto_adeudado = 0, 
        servicio_cortado = 0,
        fecha_regularizacion = CAST(GETDATE() AS DATE),
        updated_at = GETDATE()
    FROM dbo.MORA m
    INNER JOIN inserted i ON m.id_contrato = i.id_contrato
    WHERE i.anulado = 0 AND m.en_mora = 1;
    
    -- Reactivar contrato suspendido
    UPDATE c
    SET estado = 'activo'
    FROM dbo.CONTRATO c
    INNER JOIN inserted i ON c.id_contrato = i.id_contrato
    WHERE i.anulado = 0 AND c.estado = 'suspendido';
END
GO

-- ============================================================
-- TRIGGER 2: Prevenir eliminación de registros críticos
-- Acción: Antes de DELETE en CONTRATO, PAGO, CLIENTE
-- Propósito: Proteger datos de eliminación accidental
-- ============================================================
CREATE OR ALTER TRIGGER trg_prevenir_delete_contrato
ON dbo.CONTRATO
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    RAISERROR('No se permite eliminar contratos. Use el cambio de estado a cancelado.', 16, 1);
    ROLLBACK TRANSACTION;
END
GO

-- ============================================================
-- TRIGGER 3: Registrar intentos fallidos de login
-- Acción: Después de UPDATE en USUARIO (cuando aumentan intentos)
-- Propósito: Bloquear cuenta tras 5 intentos fallidos
-- ============================================================
CREATE OR ALTER TRIGGER trg_bloquear_usuario
ON dbo.USUARIO
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Bloquear por 30 minutos si llega a 5 intentos fallidos
    UPDATE dbo.USUARIO
    SET bloqueado_hasta = DATEADD(MINUTE, 30, GETDATE())
    WHERE id_usuario IN (SELECT id_usuario FROM inserted WHERE intentos_fallidos >= 5)
      AND bloqueado_hasta IS NULL;
    
    -- Resetear intentos si ya pasó el bloqueo
    UPDATE dbo.USUARIO
    SET intentos_fallidos = 0, bloqueado_hasta = NULL
    WHERE bloqueado_hasta IS NOT NULL AND bloqueado_hasta < GETDATE();
END
GO

-- ============================================================
-- TRIGGER 4: Validar carga máxima de técnico (5 reclamos)
-- Acción: Antes de UPDATE en RECLAMO (al asignar técnico)
-- Propósito: Evitar sobrecarga de trabajo
-- ============================================================
CREATE OR ALTER TRIGGER trg_validar_carga_tecnico
ON dbo.RECLAMO
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @id_tecnico INT, @id_reclamo INT, @carga INT;
    
    SELECT @id_tecnico = id_tecnico, @id_reclamo = id_reclamo 
    FROM inserted 
    WHERE id_tecnico IS NOT NULL;
    
    -- Si se está asignando un nuevo técnico
    IF @id_tecnico IS NOT NULL
    BEGIN
        SELECT @carga = COUNT(*) 
        FROM dbo.RECLAMO 
        WHERE id_tecnico = @id_tecnico AND estado <> 'Cerrado';
        
        IF @carga >= 5
        BEGIN
            RAISERROR('El técnico ha alcanzado el límite de 5 reclamos activos.', 16, 1);
            RETURN;
        END
    END
    
    -- Si pasa validación, ejecutar el UPDATE real
    UPDATE r
    SET id_tecnico = i.id_tecnico,
        estado = i.estado,
        solucion_aplicada = i.solucion_aplicada,
        fecha_cierre = i.fecha_cierre,
        calificacion = i.calificacion,
        tiempo_respuesta_min = i.tiempo_respuesta_min,
        ruta_audio = i.ruta_audio,
        duracion_audio_seg = i.duracion_audio_seg,
        hash_sha256 = i.hash_sha256
    FROM dbo.RECLAMO r
    INNER JOIN inserted i ON r.id_reclamo = i.id_reclamo;
END
GO

PRINT 'Triggers creados correctamente.';
GO

SELECT 
    t.name AS Nombre_Trigger,
    tables.name AS Tabla_Asociada,
    t.type_desc AS Tipo,
    t.is_disabled AS Esta_Desactivado
FROM sys.triggers t
INNER JOIN sys.tables tables ON t.parent_id = tables.object_id
ORDER BY Tabla_Asociada;