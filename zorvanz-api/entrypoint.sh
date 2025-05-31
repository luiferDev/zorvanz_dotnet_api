#!/bin/bash

# Ejecutar migraciones
dotnet ef database update

# Iniciar la aplicación
exec dotnet zorvanz-api.dll
