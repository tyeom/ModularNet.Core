# Publishing ModularNet to NuGet.org

This guide explains how to publish ModularNet.Core to NuGet.org.

## Prerequisites

1. **NuGet.org Account**
   - Create an account at https://www.nuget.org/
   - Verify your email address

2. **API Key**
   - Go to https://www.nuget.org/account/apikeys
   - Click "Create"
   - Set the following:
     - Key Name: `ModularNet Publishing`
     - Expiration: 365 days (or your preference)
     - Select Scopes: `Push new packages and package versions`
     - Glob Pattern: `ModularNet.*`
   - Click "Create"
   - **IMPORTANT**: Copy the API key immediately (it won't be shown again)

## Build the Package

The package has already been built and is located at:
```
./nupkgs/ModularNet.Core.1.0.0.nupkg
```

To rebuild the package:
```bash
dotnet pack ModularNet.Core/ModularNet.Core.csproj --configuration Release --output ./nupkgs
```

## Test the Package Locally (Optional but Recommended)

Before publishing to NuGet.org, test the package locally:

### 1. Create a local NuGet feed
```bash
# Create a test project
dotnet new console -n TestModularNet
cd TestModularNet

# Add your local package
dotnet add package ModularNet.Core --version 1.0.0 --source ../nupkgs

# Verify it works
dotnet restore
dotnet build
```

### 2. Test in a real ASP.NET Core project
```bash
dotnet new webapi -n TestWebApi
cd TestWebApi
dotnet add package ModularNet.Core --version 1.0.0 --source ../nupkgs
```

## Publish to NuGet.org

### Method 1: Using dotnet CLI (Recommended)

```bash
# Navigate to the repository root
cd /mnt/c/Users/Banaple75/source/repos/ModularNet

# Push to NuGet.org
dotnet nuget push ./nupkgs/ModularNet.Core.1.0.0.nupkg \
  --api-key YOUR_API_KEY_HERE \
  --source https://api.nuget.org/v3/index.json
```

Replace `YOUR_API_KEY_HERE` with the API key you created.

### Method 2: Using NuGet.org Web Interface

1. Go to https://www.nuget.org/packages/manage/upload
2. Click "Browse..." and select `./nupkgs/ModularNet.Core.1.0.0.nupkg`
3. Click "Upload"
4. Verify the package details
5. Click "Submit"

## After Publishing

### 1. Verify the Package
- Go to https://www.nuget.org/packages/ModularNet.Core
- Wait 10-15 minutes for the package to be indexed
- Check the README, license, and metadata

### 2. Test Installation
```bash
# Create a new project
dotnet new console -n TestFromNuGet
cd TestFromNuGet

# Install from NuGet.org
dotnet add package ModularNet.Core

# Verify
dotnet restore
```

### 3. Update GitHub Repository
- Create a GitHub release tagged `v1.0.0`
- Add release notes from CHANGELOG.md
- Link to the NuGet package

## Publishing Future Versions

### 1. Update Version Number

Edit `ModularNet.Core/ModularNet.Core.csproj`:
```xml
<Version>1.0.1</Version>
```

### 2. Update CHANGELOG.md

Add new version section:
```markdown
## [1.0.1] - 2025-XX-XX

### Fixed
- Bug fixes here

### Added
- New features here
```

### 3. Build and Publish
```bash
# Clean previous builds
rm -rf nupkgs/*

# Build new version
dotnet pack ModularNet.Core/ModularNet.Core.csproj --configuration Release --output ./nupkgs

# Push to NuGet.org
dotnet nuget push ./nupkgs/ModularNet.Core.1.0.1.nupkg \
  --api-key YOUR_API_KEY_HERE \
  --source https://api.nuget.org/v3/index.json
```

## Security Best Practices

1. **Never commit API keys to Git**
   - Store API key in environment variable:
     ```bash
     export NUGET_API_KEY="your-api-key-here"
     dotnet nuget push ./nupkgs/ModularNet.Core.1.0.0.nupkg \
       --api-key $NUGET_API_KEY \
       --source https://api.nuget.org/v3/index.json
     ```

2. **Use .gitignore**
   - Ensure `nupkgs/` is in .gitignore
   - Never commit `.nupkg` files

3. **Rotate API keys regularly**
   - Set reasonable expiration dates
   - Revoke old keys after creating new ones

## Troubleshooting

### Package Already Exists
If you get an error that the package version already exists:
- You cannot republish the same version
- Increment the version number and rebuild

### Invalid Package
If NuGet.org rejects the package:
- Check that all metadata is filled out
- Ensure README.md is included
- Verify license is specified

### Authentication Failed
If you get authentication errors:
- Verify your API key is correct
- Check that the API key hasn't expired
- Ensure you have the right permissions

## Package Validation

Before publishing, run local validation:
```bash
# Install NuGet Package Explorer (optional)
dotnet tool install -g NuGetPackageExplorer

# Or extract and inspect manually
unzip -l ./nupkgs/ModularNet.Core.1.0.0.nupkg
```

Expected contents:
- `lib/net10.0/ModularNet.Core.dll`
- `lib/net10.0/ModularNet.Core.xml` (documentation)
- `README.md`
- `.nuspec` file with correct metadata

## Continuous Integration (Future Enhancement)

Consider setting up GitHub Actions to automate publishing:

```yaml
# .github/workflows/publish-nuget.yml
name: Publish to NuGet

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - name: Pack
        run: dotnet pack ModularNet.Core/ModularNet.Core.csproj --configuration Release --output ./nupkgs
      - name: Push to NuGet
        run: dotnet nuget push ./nupkgs/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

Store your API key in GitHub Secrets:
- Go to Settings > Secrets and variables > Actions
- Add a new secret named `NUGET_API_KEY`
- Paste your NuGet API key

## Support

After publishing, users can:
- Report issues at: https://github.com/Banaple75/ModularNet/issues
- View documentation in README.md
- Check examples in ModularNet.Sample

## References

- [NuGet.org Documentation](https://docs.microsoft.com/en-us/nuget/)
- [Creating NuGet Packages](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package)
- [Publishing to NuGet.org](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
