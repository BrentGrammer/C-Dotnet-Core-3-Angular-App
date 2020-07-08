#!/bin/bash
cd .. 
cd DatingApp.API
dotnet user-secrets set "AppSettings:Token" "my dev secret token" 
dotnet user-secrets list;