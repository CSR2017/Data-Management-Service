FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.18@sha256:2a8dca3af111071172b1629c12eefaeca0d6c2954887c4489195771c9e90833c as build
WORKDIR /source

COPY EdFi.DataManagementService.Backend.Installer/*.csproj EdFi.DataManagementService.Backend.Installer/
COPY EdFi.DataManagementService.Backend/*.csproj EdFi.DataManagementService.Backend/
COPY EdFi.DataManagementService.Backend.Mssql/*.csproj EdFi.DataManagementService.Backend.Mssql/
COPY EdFi.DataManagementService.Backend.Postgresql/*.csproj EdFi.DataManagementService.Backend.Postgresql/

RUN dotnet restore EdFi.DataManagementService.Backend.Installer

COPY EdFi.DataManagementService.Backend.Installer/ EdFi.DataManagementService.Backend.Installer/
COPY EdFi.DataManagementService.Backend/ EdFi.DataManagementService.Backend/
COPY EdFi.DataManagementService.Backend.Mssql/ EdFi.DataManagementService.Backend.Mssql/
COPY EdFi.DataManagementService.Backend.Postgresql/ EdFi.DataManagementService.Backend.Postgresql/
COPY .editorconfig .

RUN dotnet build -c Release -r linux-x64 --no-restore EdFi.DataManagementService.Backend.Installer

RUN dotnet publish -c Release -o /app -r linux-x64 -p:PublishSingleFile=true \
    --no-build --no-restore EdFi.DataManagementService.Backend.Installer

FROM postgres:16.2-alpine3.19@sha256:b89a4e92591810eac1fbce6107485d7c6b9449df51c1ccfcfed514a7fdd69955

WORKDIR /app
ENV POSTGRES_DB=postgres
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=password

COPY --from=build /app .
USER root
COPY run.sh .
RUN chmod +x ./run.sh && ./run.sh
EXPOSE 5432
USER postgres
CMD ["postgres"]
