# C# Dotnet Core 3.0 App with Angular Frontend

### Note: The code is heavily commented for reference purposes.

- MVC Architecture
- Full CRUD operations
- Angular 8 Frontend

#### Dotnet Topics Covered:

- Dependency Injection and Configuration
- Using EF for Database migrations
- Exception Handling - Global Exception Handler
- Using Dotnet Secrets
- AutoMapper
- Seeding data to the database for development
- JWT Authentication

### Running the App

##### Pre-requisites/Dev Tools

- Angular CLI with Angular 8+ installed
- [Dotnet SDK 3.0 (Release 9-23-2019, v3.0.100)](https://dotnet.microsoft.com/download/dotnet-core/3.0)
  - Check version installed and bveing used with `dotnet --version`
- [DB Browser for SQLite](https://sqlitebrowser.org/) - for development

#### Development:

- `cd DatingApp/DatingApp-SPA`
- `npm i`
- `ng serve`

##### Backend:

- `cd DatingApp/DatingApp.API`
- `dotnet restore`
- `dotnet watch run`

### Prevent Prettier from formatting html incorrectly:

in settings.json for vscode:

```
"editor.formatOnSave": true,
"[html]": {
    "editor.defaultFormatter": "vscode.html-language-features"
}
```
