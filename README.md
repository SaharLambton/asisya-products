# Asisya Products API

Prueba técnica para la posición **Dev II en Asisya**. Es un sistema de gestión de productos con API REST en .NET 8 y frontend en React.

[![CI Pipeline](https://github.com/SaharLambton/asisya-products/actions/workflows/ci.yml/badge.svg)](https://github.com/SaharLambton/asisya-products/actions)

---

## ¿Qué hay en este proyecto?

- API REST con autenticación JWT
- CRUD completo de productos y categorías
- Carga masiva de hasta 100.000 productos
- Frontend en React con login, listado, filtros y formularios
- Todo dockerizado y con pipeline de CI en GitHub Actions

---

## Arquitectura

Decidí usar arquitectura limpia separada en 4 capas. La idea principal era que el dominio y la lógica de negocio no dependan de frameworks ni de la base de datos, lo que hace las pruebas unitarias mucho más sencillas.

```
API  →  Application  →  Domain
              ↑
       Infrastructure
```

- **Domain:** entidades y contratos (interfaces de repositorios)
- **Application:** servicios, DTOs, mappers
- **Infrastructure:** EF Core, repositorios, migraciones
- **API:** controladores, middleware, configuración JWT

Los DTOs los mapeé de forma explícita con métodos estáticos en vez de usar AutoMapper. Prefiero ver exactamente qué se está mapeando sin depender de convenciones que a veces fallan silenciosamente.

También usé un `ServiceResult<T>` como wrapper de respuestas en los servicios para no lanzar excepciones en flujos normales como un 404 o un conflicto.

---

## Stack

| Parte | Tecnología |
|---|---|
| Backend | .NET 8 / C# |
| Base de datos | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Auth | JWT Bearer |
| Contraseñas | BCrypt.Net |
| Frontend | React 18 + TypeScript + Vite |
| HTTP client | Axios |
| Formularios | React Hook Form |
| Pruebas | xUnit + Moq + FluentAssertions + Testcontainers |
| Infra | Docker + docker-compose + GitHub Actions |

---

## Cómo correrlo con Docker

Lo más rápido es con docker-compose, solo necesitas tener Docker instalado:

```bash
git clone https://github.com/YOUR_USER/asisya-products.git
cd asisya-products

docker compose up --build
```

Con eso levanta todo:
- API → http://localhost:8080
- Swagger → http://localhost:8080/swagger
- Frontend → http://localhost:3000
- PostgreSQL → localhost:5432

Al iniciar por primera vez se corren las migraciones automáticamente y se crea un usuario admin por defecto: `admin / Admin@1234!`

Si quieres también abrir pgAdmin para inspeccionar la base de datos:

```bash
docker compose --profile tools up --build
# pgAdmin en http://localhost:5050
```

---

## Cómo correrlo en local (sin Docker)

Necesitas .NET 8 SDK, Node.js 20+ y PostgreSQL corriendo.

**Backend:**
```bash
# Actualiza la cadena de conexión en:
# src/Asisya.Products.API/appsettings.Development.json

dotnet restore
dotnet run --project src/Asisya.Products.API
# API en http://localhost:5000
# Swagger en http://localhost:5000/swagger
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
# App en http://localhost:3000
```

---

## Endpoints principales

Todos los endpoints excepto `/api/auth/*` requieren JWT:
```
Authorization: Bearer <token>
```

**Auth**
```
POST /api/auth/login      → obtener token
POST /api/auth/register   → registrar usuario
```

**Categorías**
```
GET    /api/category
GET    /api/category/{id}
POST   /api/category
PUT    /api/category/{id}
DELETE /api/category/{id}
```

**Productos**
```
GET    /api/products               → paginado, con filtros
GET    /api/products/{id}          → detalle con foto de categoría
POST   /api/products               → crear uno
POST   /api/products/bulk          → carga masiva (hasta 100k)
PUT    /api/products/{id}
DELETE /api/products/{id}
```

Parámetros del listado: `page`, `pageSize` (máx 100), `search`, `categoryId`, `isActive`.

**Ejemplo — crear categorías requeridas:**
```bash
curl -X POST http://localhost:8080/api/category \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"name":"SERVIDORES","imageUrl":"https://example.com/servers.png"}'
```

**Ejemplo — carga masiva:**
```bash
curl -X POST http://localhost:8080/api/products/bulk \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"count":100000,"categoryId":"<guid>"}'
```

Para los 100k productos usé inserciones por lotes de 1.000 registros con `ChangeTracker.Clear()` entre cada batch, así el uso de memoria se mantiene constante sin importar el total.

---

## Pruebas

```bash
# Unitarias (no necesitan base de datos)
dotnet test tests/Asisya.Products.Tests.Unit

# Integración (Testcontainers levanta un PostgreSQL real automáticamente)
dotnet test tests/Asisya.Products.Tests.Integration

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

Para las pruebas de integración usé Testcontainers porque prefiero probar contra una base de datos real en vez de simularla con SQLite. Así se prueban las queries reales, los índices y las constraints. Se instala sola, no hay que configurar nada.

---

## Escalabilidad horizontal

La API es stateless gracias a JWT, así que escalar horizontalmente es directo: se agregan réplicas detrás de un balanceador de carga y cualquiera puede atender cualquier request.

Para un entorno cloud real haría:

- **Load balancer** (AWS ALB o nginx) repartiendo tráfico entre pods
- **PostgreSQL administrado** (RDS, Cloud SQL) con réplicas de lectura para los GET de productos
- **PgBouncer** para manejar el pool de conexiones cuando hay muchas réplicas
- **Redis** para cachear las consultas paginadas más frecuentes
- **HPA en Kubernetes** para auto-escalar según CPU o RPS
- Para inserciones muy grandes (+100k), mover el bulk a una cola (RabbitMQ / SQS) y procesarlo en background

---

## CI/CD

El pipeline en `.github/workflows/ci.yml` tiene 4 jobs que corren en orden:

1. Build y pruebas unitarias del backend
2. Pruebas de integración (Testcontainers)
3. Lint y build del frontend
4. Build de las imágenes Docker (solo en `main`)

Se puede ver en la pestaña Actions del repositorio.
