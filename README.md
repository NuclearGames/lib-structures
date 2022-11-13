# Structures

## Net6 Version
Developed on Net-6.0. You can install with command:

```
dotnet add PROJECT package NuGet.Structures-NetSixZero --version 1.1.0
```

### Workflow

#### Tests

Tests should be run automaticly on creating pull-request to `main` branch. Workflow will handle trigger on pull-request to `main` branch from any other, that include key `net6` in it's name. So, `**/net6/*` is recommended name of branch to handle test execution on pull-request.

#### Build

To create build you should push a tag with name pattern `Net6_v#.#.#` in exitsed in `main` branch commit

## Unity Version
Developed on a version of unity #2020.3. You can add https://github.com/NuclearGames/Structures.git?path=Unity/Structrues-Unity/Packages/Structures-Unity to Package Manager

If you want to set a target version, 'Structures' uses the *.*.* release tag so you can specify a version like #1.1.0. For example https://github.com/NuclearGames/Structures.git?path=Unity/Structrues-Unity/Packages/Structures-Unity#1.1.0.

#### Tests

Tests should be run automaticly on creating pull-request to `main` branch. Workflow will handle trigger on pull-request to `main` branch from any other, that include key `unity` in it's name. So, `**/unity/*` is recommended name of branch to handle test execution on pull-request.

If you wanna deploy changes for both versions, `net6` and `unity`, you should include both key-words in branch name (i.e., `**/net6_unity/*`)

#### Build

Build will be created automaticly on merge to `main` branch. The version of package will be handled from specific field from `Unity/Structrues-Unity/Packages/Structures-Unity.package.json` file
