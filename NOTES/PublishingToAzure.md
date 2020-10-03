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
