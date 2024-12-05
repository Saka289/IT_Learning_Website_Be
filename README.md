## AspnetCore Monolithic:
An ASP.NET Core monolithic application is a unified architecture where all components (UI, business logic, data access) are integrated and deployed together. This simplifies development and deployment but can become challenging to scale and maintain as the application grows. It's ideal for smaller projects.

## Prepare environment

* Install dotnet core version in file `global.json`
* IDE: Visual Studio 2022+, Rider, Visual Studio Code
* Docker Desktop

## Warning:

Some docker images are not compatible with Apple Chip (M1, M2). You should replace them with appropriate images. Suggestion images below:
- sql server: mcr.microsoft.com/azure-sql-edge
- mysql: arm64v8/mysql:oracle
---
## How to run the project

Run command for build project
```Powershell
dotnet build
```
Go to folder contain file `docker-compose`

1. Using docker-compose
```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```
2. Using Visual Studio 2022
- Open LW-BE.sln - `LW_BE.sln`
- Run Compound to start multi projects
---

## Application URLs - LOCAL Environment:
- Grade API: https://localhost:7000/api/grade
- Document API: https://localhost:7000/api/document
- Topic API: https://localhost:7000/api/topic

## Docker Application URLs - LOCAL Environment (Docker Container):
- Elasticsearch: http://localhost:9200 - username: elastic ; pass: admin1234
- Kibana: http://localhost:5601 - username: elastic ; pass: admin1234
- Redis: http://localhost:6379 - pass: admin@1234
- MySQL: http://localhost:3306 - username: root ; pass: saka2892002
- PostgreSQL: http://localhost:5432 - username: judge0 ; pass: admin@1234

## Application URLs - DEVELOPMENT Environment (Docker Container):
- Grade API: https://localhost:8000/api/grade
- Document API: https://localhost:8000/api/document
- Topic API: https://localhost:8000/api/topic

## Application URLs - PRODUCTION Environment:
- Grade API: https://lw-api.azurewebsites.net/api/grade
- Document API: https://lw-api.azurewebsites.net/api/document
- Topic API: https://lw-api.azurewebsites.net/api/topic

## Packages References
- https://www.nuget.org/packages/FluentValidation/11.9.2
- https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design/6.0.3
- https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/6.0.3
- https://www.nuget.org/packages/Microsoft.VisualStudio.Web.CodeGeneration.Design/6.0.16
- https://www.nuget.org/packages/Swashbuckle.AspNetCore/6.5.0

## Install Environment

- https://dotnet.microsoft.com/download/dotnet/6.0
- https://visualstudio.microsoft.com/

## References URLS
- ASP.NET Core Documentation: https://learn.microsoft.com/en-us/aspnet/core/
- Docker Documentation: https://docs.docker.com/
- Entity Framework Core Documentation: https://learn.microsoft.com/en-us/ef/core/
- Elasticsearch Documentation: https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html
- Redis Documentation: https://redis.io/docs/
- Swashbuckle (Swagger) Documentation: https://swagger.io/docs/
- FluentValidation Documentation: https://docs.fluentvalidation.net/en/latest/
- Visual Studio Documentation: https://learn.microsoft.com/en-us/visualstudio/

## Docker Commands: (cd into folder contain file `docker-compose.yml`, `docker-compose.override.yml`)

- Up & running:
```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans --build
```
- Stop & Removing:
```Powershell
docker-compose down
```

## Useful commands:

- ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
- dotnet watch run --environment "Development"
- dotnet restore
- dotnet build
- Migration commands for Sample API:
  - cd into Sample folder
  - dotnet ef migrations add "SampleMigration" -p Libraries/Sample.Data --startup-project Sample.API --output-dir Persistence/Migrations
  - dotnet ef migrations remove -p Libraries/Sample.Data --startup-project Sample.API
  - dotnet ef database update -p Libraries/Sample.Data --startup-project Sample.API