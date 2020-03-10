# GitHub Command

This tool is can be use to clean up unwanted packages from GitHub Package Registry.

## 1. Pre-requirements
* [.NET Core 3.0+](https://dotnet.microsoft.com/download/dotnet-core/3.0) is required to run this package.
* This tool provides as an .NetCore 3.0 tool package.

## 2. Install GitHub-Tools

To install this tool. please run following command:
```bash
dotnet tool install --global Oolong.GitHub.CommandLine
```

## 3. Available sub commands

### 3.1. [delete-package] Clean up older packages

After installing the tool you can run `dotnet github-tool` to clean packages.

* Following command removes all package version for `Oolong.GitHub.CommandLine` from repository `GitHub.CommandLine` but keeps the latest 5 versions
```bash
dotnet github-tool delete-package -t ${{secrets.github_token}} -o wuganhao -r GitHub.CommandLine -p Oolong.GitHub.CommandLine -k 5 -m all
```
* Following command removes all package version for `Oolong.GitHub.CommandLine` with version number `4.0.0.*` and keeps only the latest 5 versions.
```bash
dotnet github-tool delete-package -t ${{secrets.github_token}} -o wuganhao -r GitHub.CommandLine -p Oolong.GitHub.CommandLine -k 5 -m current -v 4.0.0
```
* For using this tool in GitHub Actions, You can take the example in this repository. This repository use the same tool to cleanup itself. (*When using this inside GitHub Actions, owner and repository name can be emitted*)


```yaml
    - name: Setup .NET Core CLI
      uses: actions/setup-dotnet@v1.1.0

    - name: Remove older packages and keep only latest 5 versions
      run: |
        dotnet tool install --global Oolong.GitHub.CommandLine
        dotnet github delete-package -t ${{secrets.github_token}} -k 5 -m all
```

* To get details about how to use this commands, Try `github delete-package --help`.

### 3.2. [auto-merge] Automatically merge from all previous branches

* Run following command line for automatically merging all branch from pre-4.0.0 to 4.0.0. Break on any conflict, allow user to resolve conflicts and re-run this command line again.
```bash
github auto-merge -t 4.0.0 -c break
# or
dotnet github aut-merge -t 4.0.0 -c break
```

* Run following command line for automatically merging all branch from pre-4.0.0 to 4.0.0. Skip on any conflict, and continue merging the all other version that has no conflicts. (**This might cause more conflicts, as normally we have to merge from lower version then higher**)
```bash
github auto-merge -t 4.0.0 -c skip
# or
dotnet github auto-merge -t 4.0.0 -c skip
```
* To get details about how to use this commands, Try `github delete-package --help`.