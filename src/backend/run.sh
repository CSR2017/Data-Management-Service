until PGPASSWORD=$POSTGRES_PASSWORD psql -h localhost -p 5432 -U $POSTGRES_USER -c '\q';
do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 10
done
>&2 echo "Postgres is up - executing command"
exec $cmd
# Run the .NET application
dotnet /app/installer/EdFi.DataManagementService.Backend.Installer.dll postgresql "host=localhost;port=5432;username=postgres;password=password;database=EdFi.DataManagementService;"

