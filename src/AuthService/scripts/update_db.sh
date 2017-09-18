#!/bin/bash

ROOT_DIR="$( cd "$(dirname "$0")/.." ; pwd -P )"

cd ${ROOT_DIR}
rm -rf Migrations 
dotnet1.1 ef migrations add m1 
dotnet1.1 ef migrations script -o AuthServiceDbSchema.sql

db="AuthServiceDb"
echo "Destroying database ${db}"
mysql --execute "DROP DATABASE ${db};"

echo "Recreating database ${db}"
mysql --execute "CREATE DATABASE ${db};"

echo "Loading db schema."
mysql --database ${db} < AuthServiceDbSchema.sql

echo "Loading main data."
mysql --database AuthServiceDb < AuthServiceDbData.sql


cd -