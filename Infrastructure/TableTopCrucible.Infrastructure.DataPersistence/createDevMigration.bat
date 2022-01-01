echo off
dotnet ef migrations remove
dotnet ef migrations add DEV
pause