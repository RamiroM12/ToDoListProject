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

---

## Variables a configurar en User-secrets

🔑 Configurar JWT para entorno local

Este proyecto utiliza autenticación basada en tokens JWT. Para poder generar y validar tokens correctamente, debés configurar las siguientes claves usando el sistema de User Secrets de .NET:

🧪 Pasos

Ejecutá los siguientes comandos en la terminal desde la raíz del proyecto:

	dotnet user-secrets set "Jwt:Key" "TU_CLAVE_SECRETA_SEGURA"
	dotnet user-secrets set "Jwt:Issuer" "https://localhost:5001"
	dotnet user-secrets set "Jwt:Audience" "https://localhost:5001"
 
Jwt:Key: Es una clave secreta (string larga) usada para firmar los tokens.
✨ Recomendado: usar una clave larga, aleatoria y segura.

Jwt:Issuer y Jwt:Audience: Deberían coincidir con la URL base de tu app durante el desarrollo.

🔐 Configurar OAuth2 (Google) para desarrollo local

Este proyecto soporta login con Google utilizando OAuth2.
Por motivos de seguridad, las credenciales (Client ID y Client Secret) no están incluidas en el repositorio.
Para utilizar esta funcionalidad en tu entorno local, seguí los siguientes pasos:

✅ Pasos para crear tus propias credenciales OAuth2
Entrá al panel de Google Cloud Console:
https://console.cloud.google.com

Creá un nuevo proyecto (o usá uno existente).

Activá la API de Google Identity Platform:

Ir a “API & Services” > “Library”.

Buscar “Google Identity” o “OAuth2” y activarla.

En “APIs & Services” > “Credentials”, hacé clic en:

➕ Create credentials > OAuth client ID

Tipo de aplicación: Web application

Agregá los siguientes URIs:

URIs autorizados de redireccionamiento (Authorized redirect URIs):

	https://localhost:5001/signin-google
(Usá el puerto que esté usando tu app.)

URIs autorizados de JavaScript (si corresponde):

	https://localhost:5001
Guardá el Client ID y Client Secret generados.

🔐 Agregarlos a los User Secrets
Usá el sistema de secretos de .NET para guardar tus credenciales localmente:

	dotnet user-secrets set "Authentication:Google:ClientId" "TU_CLIENT_ID"
	dotnet user-secrets set "Authentication:Google:ClientSecret" "TU_CLIENT_SECRET"
