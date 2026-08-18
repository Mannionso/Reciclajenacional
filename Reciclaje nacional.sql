create database reciclaje_nacional;
go

use reciclaje_nacional;
go

create table usuario (
    idusuario int primary key identity(1,1),
    nombre varchar(100) not null,
    correo varchar(100) not null,
    provincia varchar(50) not null,
    puntos int not null default 0
);
go

create table material (
    idmaterial int primary key identity(1,1),
    nombre varchar(50) not null,
    descripcion varchar(200),
    puntosporkg decimal(10,2) not null
);
go

create table centroreciclaje (
    idcentro int primary key identity(1,1),
    nombre varchar(100) not null,
    provincia varchar(50) not null,
    direccion varchar(200) not null,
    horario varchar(100) not null
);
go

create table registroreciclaje (
    idregistro int primary key identity(1,1),
    idusuario int not null,
    idmaterial int not null,
    idcentro int not null,
    cantidadkg decimal(10,2) not null,
    fecha datetime not null,
    puntosobtenidos int not null,

    foreign key (idusuario) references usuario(idusuario),
    foreign key (idmaterial) references material(idmaterial),
    foreign key (idcentro) references centroreciclaje(idcentro)
);
go

create table recompensa (
    idrecompensa int primary key identity(1,1),
    nombre varchar(100) not null,
    descripcion varchar(200),
    puntosnecesarios int not null,
    cantidaddisponible int not null
);
go

create table canje (
    idcanje int primary key identity(1,1),
    idusuario int not null,
    idrecompensa int not null,
    fecha datetime not null,
    puntosutilizados int not null,

    foreign key (idusuario) references usuario(idusuario),
    foreign key (idrecompensa) references recompensa(idrecompensa)
);
go