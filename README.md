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
│   ├── Authorization/                  # Sistema de autorización por permisos
│   │   ├── PermissionRequirement.cs
│   │   ├── PermissionAuthorizationHandler.cs
│   │   └── RequirePermissionAttribute.cs
│   ├── Controllers/
│   │   ├── AuthController.cs           # Endpoints de autenticación
│   │   ├── UsersController.cs          # Gestión de usuarios
│   │   ├── RolesController.cs          # Gestión de roles
│   │   └── PermissionsController.cs    # Gestión de permisos
│   ├── Program.cs                      # Configuración y punto de entrada
│   ├── appsettings.json                # Configuración base
│   └── appsettings.Development.json    # Configuración desarrollo
│
├── Application/
│   ├── Common/
│   │   ├── Result.cs                   # Wrapper de respuestas
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs   # Comportamiento de validación
│   ├── DTOs/
│   │   ├── Auth/                       # DTOs de autenticación
│   │   ├── Users/                      # DTOs de usuarios
│   │   ├── Roles/                      # DTOs de roles
│   │   └── Permissions/                # DTOs de permisos
│   ├── Features/                       # Casos de uso (CQRS con MediatR)
│   │   ├── Auth/                       # Commands de autenticación
│   │   ├── Users/                      # Commands/Queries de usuarios
│   │   ├── Roles/                      # Commands/Queries de roles
│   │   └── Permissions/                # Queries de permisos
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   ├── ITokenService.cs
│   │   └── IUserRoleService.cs
│   └── Services/
│       ├── TokenService.cs             # Servicio de generación JWT (con permisos)
│       └── UserRoleService.cs          # Servicio de gestión de roles
│
├── Domain/
│   ├── Entities/
│   │   ├── ApplicationUser.cs          # Entidad Usuario
│   │   ├── ApplicationRole.cs          # Entidad Rol
│   │   ├── ApplicationPermission.cs    # Entidad Permiso
│   │   └── BaseEntity.cs               # Entidad base
│   └── Interfaces/
│       └── ISoftDelete.cs              # Interface para borrado lógico
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

## 🔒 Sistema de Autorización Basado en Permisos

El sistema de autorización ha sido migrado de **basado en roles** a **basado en permisos**, permitiendo un control de acceso más granular y flexible.

### Estructura de Datos

- **Users** → **UserRoles** → **Roles** → **RolePermissions** → **Permissions**
- Cada usuario puede tener múltiples roles
- Cada rol puede tener múltiples permisos
- Los endpoints están protegidos por permisos, no por roles

### Componentes

#### 1. PermissionRequirement

Define qué permiso se necesita para acceder a un recurso.

#### 2. PermissionAuthorizationHandler

Valida si el usuario autenticado tiene el permiso requerido leyendo los claims del JWT.

#### 3. RequirePermissionAttribute

Atributo personalizado para aplicar protección en endpoints.

**Uso:**

```csharp
[RequirePermission("Users.Create")]
public async Task<IActionResult> CreateUser(UserRequest request)
```

### Flujo de Autorización

1. **Login** → Usuario se autentica
2. **Token** → Se genera JWT con claims de permisos basados en roles
3. **Request** → Usuario hace request con el token
4. **Validación** → El handler verifica si el token contiene el permiso requerido
5. **Acceso** → Se concede o deniega acceso

### Convención de Nombres

Formato: `{Recurso}.{Acción}`

**Ejemplos:**

- `Users.Create` - Crear usuarios
- `Users.Read` - Leer/listar usuarios
- `Users.Update` - Actualizar usuarios
- `Users.Delete` - Eliminar usuarios
- `Roles.Assign` - Asignar roles
- `Reports.Export` - Exportar reportes

### Permisos Configurados

- `Users.Create`, `Users.Read`, `Users.Update`, `Users.Delete`
- `Roles.Create`, `Roles.Read`, `Roles.Update`, `Roles.Delete`
- `Permissions.Create`, `Permissions.Read`, `Permissions.Update`, `Permissions.Delete`

### Ventajas

✅ **Granularidad** - Control fino sobre cada operación  
✅ **Flexibilidad** - Los permisos se reutilizan entre roles  
✅ **Escalabilidad** - Agregar permisos no requiere cambios de código  
✅ **Mantenibilidad** - Permisos gestionados desde la base de datos  
✅ **Separación** - Lógica de negocio desacoplada de autorización

### Agregar Nuevos Permisos

1. **Insertar en BD:**

```sql
INSERT INTO "Permissions" ("Name", "Description", "IsActive")
VALUES ('Products.Create', 'Permite crear productos', true);
```

2. **Registrar en Program.cs:**

```csharp
var permissions = new[] { "Users.Create", "Products.Create", ... };
```

3. **Aplicar en Controller:**

```csharp
[RequirePermission("Products.Create")]
public async Task<IActionResult> CreateProduct(ProductRequest request) { }
```

4. **Asignar a un Rol:**

```sql
INSERT INTO "RolePermissions" ("RoleId", "PermissionId", "IsActive")
VALUES ('role-id', 'permission-id', true);
```

### Notas Importantes

⚠️ Los permisos en `Program.cs` deben coincidir con los nombres en la BD  
⚠️ Los tokens incluyen todos los permisos del usuario  
⚠️ Si cambias permisos de un rol, el usuario debe volver a hacer login

Para más detalles, ver [PERMISSION_BASED_AUTHORIZATION.md](./API/PERMISSION_BASED_AUTHORIZATION.md)

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
