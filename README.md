## To Do List API

Una API REST desarrollada con .NET que permite gestionar una lista de tareas almacenadas en una DB de MySQL. Cuenta con operaciones CRUD, permite ordenar tareas por prioridad y tiene un sistema de login.

## ✨ Funcionalidades

- Registro e inicio de sesión de usuarios
- Creación, edición y eliminación de tareas
- Tareas filtradas por usuario autenticado
- Ordenamiento por prioridad
- Persistencia de datos usando MySQL

--- 

## 🛠 Tecnologías Utilizadas

- C# con .NET 8
- Minimal API
- Entity Framework Core + MySQL
- Autenticación con JWT (tokens en cookies)
- FluentValidation (validación de DTOs)

---

## 💠 Instalación

1. Clonar Repo

		 git clone https://github.com/RamiroM12/ToDoListProject.git

2.  Configurar Conexión con MySQL

		{
		  "ConnectionStrings": {
			  "DefaultConnection": "Server=localhost;Database=ToDoDb;User=root;Password=tu_clave;"
			}
		}

3.  Ejecuta las migraciones y crear la base de datos

		 dotnet ef database update

4.  Ejecutar API

		dotnet run
