# Publishing to Azure

## Azure Devops

- Repo is at `https://brentonmarquez@dev.azure.com/brentonmarquez/dotnet-practice/_git/dotnet-practice`
  - Repo is private: sign in is alt gmail addr with regular pass, old addr!
- To transfer an existing repo, go to Repos and use the example shown to use the command line. can use:
  - `git remote add azure https://brentonmarquez@dev.azure.com/brentonmarquez/dotnet-practice/_git/dotnet-practice`
  - `git push origin azure`
    - Or you can set the repo to origin and set it as upstream alternatively (this is done here to maintain the code on Github)

## Deploying to Azure Portal

- (https://portal.azure.com/)
- Go to `Create Resource` -> `Web app`
  - **NOTE** Another option is to search marketplace for `Web app + Sql` which comes bundled with a database setup
  - Select a subscription (should have one by default)
  - Name a resource group (just a collection of resources i.e. `DatingAppResourceGroup`)
    - The resource group helps you associate resources with a project
  - Enter a unique name for the url
  - Select publish as Code or Docker Container(if using containerization)
  - Choose the .NET Core runtime stack
  - Select `Linux` for Operating System(can be faster and cheaper on Linux)
  - Select or create an existing Plan
  - **Change Sku and size** - select the free or lowest cost option if desired - a more expensive option is selected by default
    - Select the Dev/Testing Tier box and the free plan
  - Select `Review + Create` button, review your choices and then click `Create`
- When done, you can click the notifications icon in the top right of the screen and select `pin to dashboard` for easy access
- If you visit the url for your project you should get a notice that your service is running and you can now deploy your code.

### Publishing a release

#### In VS Code:

- `dotnet publish -c Release`
  - latest code changes will be in a `publish` directory
  - Before doing this you need to have built a prod build of your angular app and configured the output of the files to be sent to a folder in the root of your dotnet project (see commit in repo for changes to make)
- Can add the Azure App Service management extension by Microsoft to aid in publishing to Azure from VS Code (Also installs Azure Account extension for login)
  - can click on new Azure tab in left panel to sign in to Azure
  - You want to populate the `Files` directory in your project under the subscription/project in the side panel after signing in
    - Click the `Deploy to Web app` icon in the panel (upload symbol), click `Browse...` and navigate to and select the `publish` folder in your project where your files were output with the publish command
    - select the web app to deploy to and confirm prompt to deploy

## Database Setup

- You can remove the Connection string in your production app settings file since it will be in Configuration of Application Settings as an environment variable on Azure
  - (You need to set that connection string up on Azure of course)
- Change database provider in startup.cs to `x.UseSqlServer(...)`

## Troubleshooting

- It may be easiest to just insert the `app.UseDeveloperExceptionPage()` in your prod config in `Startup.cs` to detect if something goes wrong on deployment or staging when testing and comment out any custom exception handling there as well (`app.useEcveptionHandler`).
  - Otherwise, you need to setup Application Insights and get detailed logging setup in Azure
