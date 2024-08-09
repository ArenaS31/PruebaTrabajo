Create Database Practica;

Use Practica;

Create Table Usuarios(
	Id_Usuario int identity(1, 1) PRIMARY KEY, 
	Username varchar(50) not null, 
	Pass nvarchar(50) not null,
	Correo nvarchar(100));

Create Table Marcas(
	Id_Marca int identity(1, 1) PRIMARY KEY, 
	Nombre_Marca nvarchar(50) not null, 
	Representante varchar(50) not null
);

Create Table Productos(
	Id_Producto int identity(1, 1) PRIMARY KEY, 
	Nombre_Producto varchar(50) not null, 
	Categoria varchar(50) not null,
	id_marca int not null, 
	FOREIGN kEY (id_marca) REFERENCES Marcas(Id_Marca)
);