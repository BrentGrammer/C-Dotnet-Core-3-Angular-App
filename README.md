# C# Dotnet Core 3.0 App with Angular Frontend

### Note: The code is heavily commented for reference purposes.

- MVC Architecture
- Full CRUD operations
- Angular 8 Frontend

#### Dotnet Topics Covered:

- Adding services to Startup.cs for Dependency Injection and Configuration
- Using EF for Database migrations
- EF Fluent API for configuring Entity Relationships
- Making Changes to the database in development:
  - `dotnet ef migrations add MyChange`
  - `dotnet ef database update`
- Exception Handling - Global Exception Handler
- Using Dotnet Secrets for development
- AutoMapper for mapping models and Dtos
- Seeding data to the database for development
- JWT Authentication
- File Upload and Storage using [Cloudinary](https://cloudinary.com)
  - Docs: https://cloudinary.com/documentation/dotnet_integration
- Action Filters for running an action when a group of routes are hit

### Running the App

##### Pre-requisites/Dev Tools

- Angular CLI with Angular 8+ installed
- [Dotnet SDK 3.0 (Release 9-23-2019, v3.0.100)](https://dotnet.microsoft.com/download/dotnet-core/3.0)
  - Check version installed and bveing used with `dotnet --version`
- [DB Browser for SQLite](https://sqlitebrowser.org/) - for development
- Visual Studio Code extensions:
  - C# for Visual Studio Code (powered by OmniSharp)
  - C# IDE Extensions for VSCode by jchannon
  - NuGet Package Manager
- Dotnet tool for EF CLI: `dotnet tool install --global dotnet-ef --version 3.0.0`

  - May need to restart VS Code for path to update

#### Setup for development on first run:

- Set user secrets with `scripts/./set-secrets.sh`
- for now, create a appsettings.json in dotnet proj folder for the connection string and Cloudinary creds(login to Cloudinary and get them from the dashboard):

```javascript
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=datingapp.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    },
    "AllowedHosts": "*",
      "CloudinarySettings": {
        "CloudName": "",
        "ApiKey": "",
        "ApiSecret": ""
      }
  }
}
```

- `cd {ProjectFolder}/DatingApp.API`
- `dotnet restore`
- Update the database with latest migration on first run: `dotnet ef database update`
- `cd {ProjectFolder}/DatingApp-SPA`
- `npm i`

#### Run Frontend:

- `{ProjectFolder}/./frontend.sh`

##### Run Backend:

- `{ProjectFolder}/./backend.sh`

### Prevent Prettier from formatting html incorrectly:

in settings.json for vscode:

```
"editor.formatOnSave": true,
"[html]": {
    "editor.defaultFormatter": "vscode.html-language-features"
}
```

### Nuget packages:

- Microsoft.AspNetCore.Mvc.NewtonSoftJson to replace default Core 3.0 serializer
- AutoMapper.Extensions.Microsoft.DependencyInjection v7.0.0 (use latest)
- CloudinaryDotnet v1.80

### Angular Packages:

- Angular2Jwt (https://github.com/auth0/angular2-jwt) for automatically adding auth token headers to all requests
- @kolkov/ngx-gallery for photo gallery
- ng2-file-upload v1.4.0 (https://github.com/valor-software/ng2-file-upload) for uploading photos on the front end
- ngx-timeago library for displaying minutes user was last active: https://github.com/ihym/ngx-timeago

#### Angular Notes:

- Create component in a folder with CLI: `ng g c folderRelativeToAppFolder/[optionalComponentFolder]/component-name --flat`

### Debugging:

- start the backend (`./backend.sh` in root folder)
- Start debugger in VsCode and use .NET Core Attach (create configuration if needed)
- Search for `DatingApp` to attach to that exe or dotnet/dll process and then you can insert breakpoints
