# Test Técnico Nubox

Aplicación .NET 8 Web API diseñada como **puente de integración entre sistemas de control de asistencia y sistema de calculo de sueldos Nubox**, permitiendo la sincronización de datos de asistencia y nómina mediante archivos Excel o APIs externas.

---

## Características principales

- **Subida de asistencias vía Excel (`multipart/form-data`)**
- **Sincronización de asistencias directa con API Partner**
- **Persistencia de resúmenes de asistencias y ejecuciones(para trazabilidad)** en SQL Server
- **Autenticación mediante API Key**
- **Configuración con Polly** para resiliencia HTTP (reintentos y circuit breaker)
- **Arquitectura por capas (DDD)**
---

## Estructura de proyecto
Nubox.BridgeApp/
├─ 1. Api/
│ └─ Nubox.BridgeApp.WebAPI/
│ ├─ Connected Services/
│ ├─ Properties/
│ ├─ Controllers/
│ ├─ Models/
│ ├─ Security/
│ │ └─ ApiKeyAuthenticationHandler.cs
│ ├─ appsettings.json
│ ├─ Nubox.BridgeApp.WebAPI.http
│ └─ Program.cs
│
├─ 2. Application/
│ └─ Nubox.BridgeApp.Application/
│ ├─ Dto/
│ │ ├─ PartnerAsistenciaDto.cs
│ │ └─ ResumenAsistenciaDto.cs
│ ├─ Exceptions/
│ │ └─ AppValidationException.cs
│ ├─ Interfaces/
│ │ └─ IPartnerAsistenciaClient.cs
│ └─ Services/
│ ├─ AsistenciaQueryService.cs
│ ├─ AsistenciaSyncService.cs
│ └─ ExcelUploadService.cs
│
├─ 3. Domain/
│ └─ Nubox.BridgeApp.Domain/
│ ├─ Entities/
│ │ ├─ ResumenAsistencia.cs
│ │ └─ Sincronizacion.cs
│ ├─ Interfaces/
│ │ ├─ IResumenAsistenciaRepository.cs
│ │ └─ ISincronizacionRepository.cs
│ ├─ Services/
│ │ └─ CalculadorAsistencia.cs
│ └─ ValueObjects/
│ └─ PeriodoClave.cs
│
└─ 4. Infrastructure/
└─ Nubox.BridgeApp.Infrastructure/
├─ Data/
│ ├─ AppDbContext.cs
│ ├─ ResumenAsistenciaConfiguration.cs
│ └─ SincronizacionConfiguration.cs
├─ Migrations/
│ ├─ 20251106211029_InitialCreate.cs
│ └─ AppDbContextModelSnapshot.cs
├─ Repositories/
│ ├─ ResumenAsistenciaRepository.cs
│ └─ SincronizacionRepository.cs
└─ Services/
└─ PartnerAsistenciaClient.cs


