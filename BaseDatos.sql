create database ControlCalificaciones

--abrir la base de datos
use ControlCalificaciones

--creacion de tablas
--tabla estudiantes
if object_ID('estudiantes','U')is null
create table estudiantes
(IdEstudiante int identity(1,1),
Nombre nvarchar(100) not null,
Nota decimal(5,2) not null check(nota >=0 and nota <=100)
constraint pk_estudiantes primary key (IdEstudiante)
)
go
--procedimiento para insertar
alter procedure sp_InsertarEstudiante
@Nombre nvarchar(100),
@Nota decimal(5,2)
as
begin
insert into estudiantes(Nombre,Nota)
values(@Nombre,@Nota);
end
go

--procedimiento para listar
create  procedure sp_ListaEstudiantes
as
begin
select IdEstudiante,Nombre,Nota from estudiantes order by IdEstudiante;
end
go

--procedimiento para eliminar
create  procedure sp_EliminarEstudiante
@IdEstudiante int
as 
begin
delete from estudiantes
where IdEstudiante=@IdEstudiante;
end
go
USE ControlCalificaciones;
GO