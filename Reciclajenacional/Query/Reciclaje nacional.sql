USE reciclaje_nacional;
GO

IF COL_LENGTH('usuario', 'contrasena') IS NULL
BEGIN
    ALTER TABLE usuario
    ADD contrasena VARCHAR(8) NOT NULL
        CONSTRAINT DF_usuario_contrasena DEFAULT '1234';
END
GO

IF COL_LENGTH('usuario', 'idcentro') IS NULL
BEGIN
    ALTER TABLE usuario
    ADD idcentro INT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'San José'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje San José',
     'San José',
     'Avenida Central, San José',
     'Lunes a Viernes 8:00-17:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Alajuela'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Alajuela',
     'Alajuela',
     'Calle Central, Alajuela',
     'Lunes a Sábado 8:00-17:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Cartago'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Cartago',
     'Cartago',
     'Avenida 2, Cartago',
     'Lunes a Viernes 8:00-16:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Heredia'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Heredia',
     'Heredia',
     'Calle Central, Heredia',
     'Lunes a Viernes 8:00-17:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Guanacaste'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Guanacaste',
     'Guanacaste',
     'Liberia, Guanacaste',
     'Lunes a Viernes 8:00-17:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Puntarenas'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Puntarenas',
     'Puntarenas',
     'Calle Central, Puntarenas',
     'Lunes a Viernes 8:00-17:00');
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM centroreciclaje
    WHERE provincia COLLATE Latin1_General_CI_AI = 'Limón'
)
BEGIN
    INSERT INTO centroreciclaje
    (nombre, provincia, direccion, horario)
    VALUES
    ('Centro de Reciclaje Limón',
     'Limón',
     'Calle Central, Limón',
     'Lunes a Viernes 8:00-17:00');
END
GO

UPDATE u
SET u.idcentro = c.idcentro
FROM usuario u
INNER JOIN centroreciclaje c
    ON u.provincia COLLATE Latin1_General_CI_AI =
       c.provincia COLLATE Latin1_General_CI_AI;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Usuario_Centro'
)
BEGIN
    ALTER TABLE usuario
    ADD CONSTRAINT FK_Usuario_Centro
    FOREIGN KEY (idcentro)
    REFERENCES centroreciclaje(idcentro);
END
GO

SELECT
    u.idusuario,
    u.nombre,
    u.correo,
    u.contrasena,
    u.provincia,
    u.idcentro,
    c.nombre AS centro_asignado,
    u.puntos
FROM usuario u
LEFT JOIN centroreciclaje c
    ON u.idcentro = c.idcentro;
GO

SELECT
    idcentro,
    nombre,
    provincia,
    direccion,
    horario
FROM centroreciclaje
ORDER BY idcentro;
GO