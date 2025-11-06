# byTech - Backend API

API REST desarrollada con ASP.NET Core 9.0 para el sistema byTech. Implementa autenticación JWT, arquitectura limpia y Entity Framework Core con PostgreSQL.

## 📋 Tabla de Contenidos

- [Tecnologías](#-tecnologías)
- [Arquitectura](#-arquitectura)
- [Requisitos](#-requisitos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [API Documentation](#-api-documentation)
- [Migraciones](#-migraciones)
- [Testing](#-testing)

## 🛠 Tecnologías

- **.NET 9.0** - Framework principal
- **ASP.NET Core** - Framework web
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Base de datos
- **ASP.NET Core Identity** - Sistema de usuarios y autenticación
- **JWT Bearer** - Autenticación basada en tokens
- **DotNetEnv** - Manejo de variables de entorno

## 🏗 Arquitectura

El proyecto sigue una **arquitectura limpia** (Clean Architecture) con separación de responsabilidades:

```
back/
├── API/                    # Capa de presentación (Controllers, Middleware)
├── Application/            # Lógica de aplicación (Services, DTOs, Interfaces)
├── Domain/                 # Entidades de dominio
└── Infrastructure/         # Acceso a datos (DbContext, Repositories)
```

### Capas

- **API**: Controladores HTTP, configuración de servicios, middleware
- **Application**: Lógica de negocio, servicios, DTOs, interfaces
- **Domain**: Entidades del dominio, modelos de negocio
- **Infrastructure**: Implementación de acceso a datos, DbContext, migrations

## 📦 Requisitos

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

## 🚀 Instalación

1. **Clonar el repositorio**

```bash
git clone <url-del-repositorio>
```

2. **Restaurar dependencias**

```bash
dotnet restore
```

3. **Configurar PostgreSQL**

Asegúrate de tener PostgreSQL corriendo y crea una base de datos:

```sql
CREATE DATABASE bytech;
```

## ⚙ Configuración

### Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto `/back`:

```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=tu_password
Jwt__Key=tu-clave-secreta-super-segura-de-al-menos-32-caracteres
Jwt__Issuer=byTechApi
Jwt__Audience=byTechApi-users
```

**Nota:** El archivo `.env` está en `.gitignore` por seguridad. Nunca lo subas al repositorio.

### Configuración Alternativa

También puedes usar `appsettings.Development.json` (NO recomendado para datos sensibles):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "tu-clave-secreta",
    "Issuer": "byTechApi",
    "Audience": "byTechApi-users"
  }
}
```

Ver [ENVIRONMENT_SETUP.md](./ENVIRONMENT_SETUP.md) para más opciones de configuración.

## 🏃 Ejecución

### Aplicar Migraciones

```bash
cd API
dotnet ef database update
```

### Ejecutar la aplicación

```bash
cd API
dotnet run
```

La API estará disponible en:

- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

### Ejecutar en modo watch (desarrollo)

```bash
cd API
dotnet watch run
```

### Compilar para producción

```bash
dotnet publish -c Release -o ./publish
```

## 📁 Estructura del Proyecto

```
back/
├── API/
│   ├── Controllers/
│   │   └── AuthController.cs          # Endpoints de autenticación
│   ├── Program.cs                      # Configuración y punto de entrada
│   ├── appsettings.json                # Configuración base
│   └── appsettings.Development.json    # Configuración desarrollo
│
├── Application/
│   ├── Common/
│   │   └── Result.cs                   # Wrapper de respuestas
│   ├── DTOs/
│   │   ├── Auth/                       # DTOs de autenticación
│   │   │   ├── AuthResponse.cs
│   │   │   ├── AuthRequest.cs
│   │   │   └── RegisterRequest.cs
│   │   └── Users/                      # DTOs de usuarios
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── ITokenService.cs
│   └── Services/
│       ├── AuthService.cs              # Servicio de autenticación
│       └── TokenService.cs             # Servicio de generación JWT
│
├── Domain/
│   └── Entities/
│       ├── ApplicationUser.cs          # Entidad Usuario
│       ├── ApplicationRole.cs          # Entidad Rol
│       └── ApplicationPermission.cs    # Entidad Permiso
│
└── Infrastructure/
    ├── Data/
    │   └── ApplicationDbContext.cs     # Contexto de EF Core
    └── Migrations/                     # Migraciones de BD
```

## � API Documentation

La documentación completa de los endpoints está disponible a través de **OpenAPI**:

- **OpenAPI JSON**: `http://localhost:5000/openapi/v1.json`

## 🗄 Migraciones

### Crear una nueva migración

```bash
cd API
dotnet ef migrations add NombreDeLaMigracion
```

### Aplicar migraciones

```bash
dotnet ef database update
```

### Revertir última migración

```bash
dotnet ef database update NombreMigracionAnterior
```

### Eliminar última migración (si no se aplicó)

```bash
dotnet ef migrations remove
```

## 🧪 Testing

### Ejecutar tests

```bash
dotnet test
```

### Tests con cobertura

```bash
dotnet test /p:CollectCoverage=true
```

## 🔐 Seguridad

### Políticas de Contraseñas

- Mínimo 8 caracteres
- Al menos 1 dígito
- Al menos 1 minúscula
- Al menos 1 mayúscula
- 1 carácter único

### Lockout

- Máximo 5 intentos fallidos
- Bloqueo de 5 minutos tras exceder intentos

### JWT

- Tokens con tiempo de expiración
- Validación de firma, issuer y audience
- ClockSkew en cero (sin tolerancia de tiempo)

## 📝 Convenciones

### Nomenclatura

- **Controllers**: `NombreController.cs`
- **Services**: `NombreService.cs` e `INombreService.cs`
- **Entities**: `NombreEntidad.cs` (singular, PascalCase)
- **DTOs**: `NombreRequest.cs`, `NombreResponse.cs`

### Respuestas API

Todas las respuestas usan el wrapper `Result<T>`:

```csharp
{
  "success": bool,
  "message": string,
  "data": T,
  "errors": string[]
}
```

Las propiedades se serializan en **camelCase** en JSON.

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -m 'Agrega nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es privado y confidencial.

## 👥 Autores

- **Tu Nombre** - Desarrollo inicial

## 📞 Soporte

Para soporte, contacta a [tu-email@example.com]
