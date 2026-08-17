# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade Source\CharMapPlus.Core\CharMapPlus.Core.csproj
4. Upgrade Source\CharMapPlus.Infrastructure\CharMapPlus.Infrastructure.csproj
5. Upgrade Tests\CharMapPlus.Infrastructure.IntegrationTests\CharMapPlus.Infrastructure.IntegrationTests.csproj
6. Upgrade Tests\CharMapPlus.Infrastructure.Tests\CharMapPlus.Infrastructure.Tests.csproj
7. Upgrade Source\CharMapPlus\CharMapPlus.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| (none)                                         |                            |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.Extensions.DependencyInjection |   9.0.9         |  10.0.0     | Recommended for .NET 10.0                      |
| Microsoft.Extensions.Logging.Abstractions |   9.0.10        |  10.0.0     | Security/compatibility recommended update      |
| System.Text.Encoding.CodePages     |   9.0.9         |  10.0.0     | Recommended for .NET 10.0                      |
| System.Text.Encodings.Web          |   9.0.9         |  10.0.0     | Recommended for .NET 10.0                      |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### Source\CharMapPlus.Core\CharMapPlus.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - (none reported for this project)

Other changes:
  - Verify code compiles against .NET 10.0 and address any API breaking changes discovered by the compiler.

#### Source\CharMapPlus.Infrastructure\CharMapPlus.Infrastructure.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.Extensions.Logging.Abstractions` should be replaced from `9.0.10` to `10.0.0` (*recommended for .NET 10.0*)

Other changes:
  - Run application and integration tests to validate behavior after package update.

#### Tests\CharMapPlus.Infrastructure.IntegrationTests\CharMapPlus.Infrastructure.IntegrationTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - (none reported for this project)

Other changes:
  - Ensure test runner and test SDK are compatible with .NET 10.0.

#### Tests\CharMapPlus.Infrastructure.Tests\CharMapPlus.Infrastructure.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - (none reported for this project)

Other changes:
  - Ensure test runner and test SDK are compatible with .NET 10.0.

#### Source\CharMapPlus\CharMapPlus.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0-windows10.0.22000.0` to `net10.0-windows10.0.22000.0`

NuGet packages changes:
  - `Microsoft.Extensions.DependencyInjection` should be replaced from `9.0.9` to `10.0.0` (*recommended for .NET 10.0*)
  - `System.Text.Encoding.CodePages` should be replaced from `9.0.9` to `10.0.0` (*recommended for .NET 10.0*)
  - `System.Text.Encodings.Web` should be replaced from `9.0.9` to `10.0.0` (*recommended for .NET 10.0*)

Other changes:
  - Verify WinRT/AOT-related attributes and CommunityToolkit usage for compatibility with .NET 10.0.

