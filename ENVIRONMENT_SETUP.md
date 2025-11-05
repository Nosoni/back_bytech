# Configuración con Variables de Entorno

Este proyecto usa variables de entorno para gestionar configuraciones sensibles.

## 🔧 DESARROLLO LOCAL

### Opción 1: Archivo .env (Recomendado)

1. Copia el archivo `.env.example` a `.env`:

   ```bash
   cp .env.example .env
   ```

2. Edita `.env` con tus valores locales

3. Ejecuta la aplicación:
   ```bash
   cd API
   dotnet run
   ```

### Opción 2: Variables de entorno del sistema

**Windows (PowerShell):**

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=postgres"
$env:Jwt__Key="tu-clave-secreta"
$env:Jwt__Issuer="byTechApi"
$env:Jwt__Audience="byTechApi-users"
dotnet run
```

**Windows (CMD):**

```cmd
set ASPNETCORE_ENVIRONMENT=Development
set ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=postgres
set Jwt__Key=tu-clave-secreta
set Jwt__Issuer=byTechApi
set Jwt__Audience=byTechApi-users
dotnet run
```

**Linux/Mac:**

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=postgres"
export Jwt__Key="tu-clave-secreta"
export Jwt__Issuer="byTechApi"
export Jwt__Audience="byTechApi-users"
dotnet run
```

### Opción 3: VS Code launch.json

Crea/edita `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/API/bin/Debug/net9.0/API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/API",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ConnectionStrings__DefaultConnection": "Host=localhost;Port=5432;Database=bytech;Username=postgres;Password=postgres",
        "Jwt__Key": "1dc65698-6646-44bb-97b0-e58420486225",
        "Jwt__Issuer": "byTechApi",
        "Jwt__Audience": "byTechApi-users"
      },
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      }
    }
  ]
}
```

---

## 🚀 PRODUCCIÓN

### Opción 1: Docker

```dockerfile
# En Dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Host=prod-db;Port=5432;Database=bytech;Username=prod_user;Password=${DB_PASSWORD}"
ENV Jwt__Key=${JWT_SECRET_KEY}
```

Ejecutar:

```bash
docker run -e DB_PASSWORD=xxx -e JWT_SECRET_KEY=yyy myapp
```

### Opción 2: Azure App Service

```bash
# Configurar desde Azure CLI
az webapp config appsettings set \
  --name myapp \
  --resource-group mygroup \
  --settings \
    ConnectionStrings__DefaultConnection="Host=..." \
    Jwt__Key="..." \
    Jwt__Issuer="byTechApi" \
    Jwt__Audience="byTechApi-users"
```

O desde el Portal de Azure: **Configuration → Application settings**

### Opción 3: AWS Elastic Beanstalk

En `.ebextensions/environment.config`:

```yaml
option_settings:
  - option_name: ASPNETCORE_ENVIRONMENT
    value: Production
  - option_name: ConnectionStrings__DefaultConnection
    value: "Host=..."
  - option_name: Jwt__Key
    value: "..."
```

### Opción 4: Kubernetes

```yaml
# secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: bytech-secrets
type: Opaque
stringData:
  ConnectionStrings__DefaultConnection: "Host=..."
  Jwt__Key: "..."
---
# deployment.yaml
spec:
  containers:
    - name: api
      envFrom:
        - secretRef:
            name: bytech-secrets
```

---

## 🔒 SEGURIDAD

### ⚠️ NUNCA SUBAS A GIT:

- ❌ `.env`
- ❌ `appsettings.Development.json` con secretos
- ❌ Contraseñas reales
- ❌ Claves API

### ✅ SÍ SUBE A GIT:

- ✅ `.env.example` (sin valores reales)
- ✅ `appsettings.json` (solo config base)
- ✅ Documentación

---

## 📖 Formato de nombres

ASP.NET Core usa `__` (doble guion bajo) para jerarquías en variables de entorno:

```json
// En appsettings.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Jwt": {
    "Key": "..."
  }
}
```

```bash
# Equivalente en variables de entorno:
ConnectionStrings__DefaultConnection="..."
Jwt__Key="..."
```

---

## 🧪 Verificar configuración

```bash
# Ver todas las variables
dotnet run --no-launch-profile

# Ver configuración cargada
curl http://localhost:5000/api/configuration
```
