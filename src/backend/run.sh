# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# Use this script for building a PostgreSQL container that is pre-loaded
# with the Data Management Service schema.

if [ ! -s "$PGDATA/PG_VERSION" ]; then
  echo "Initializing PostgreSQL database directory..."
  initdb $PGDATA
fi

[ $? -ne 0 ] && exit || echo "Starting PostgreSQL service..."

pg_ctl -D "$PGDATA" -o "-c listen_addresses='*'" -w start

[ $? -ne 0 ] && exit || echo "Waiting for PostgreSQL to start..."

until pg_isready -h localhost -p 5432 -U postgres; do
  echo "Waiting for PostgreSQL to start..."
  sleep 2
done

echo "PostgreSQL is ready. Install Data Management Service schema."

/app/EdFi.DataManagementService.Backend.Installer \
    postgresql \
    "host=localhost;port=5432;username=postgres;database=edfi_dms;"

echo "Shutdown PostgreSQL"
pg_ctl -D $(psql -Xtc 'show data_directory') stop
