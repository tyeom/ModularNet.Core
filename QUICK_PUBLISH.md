# Quick Publish Guide

## ✅ Package is Ready!

Your NuGet package has been successfully built:
- **Location**: `./nupkgs/ModularNet.Core.1.0.0.nupkg`
- **Size**: 24KB
- **Status**: Ready to publish

## 🚀 Publish to NuGet.org in 3 Steps

### Step 1: Get Your NuGet API Key

1. Go to https://www.nuget.org/ and sign in (or create account)
2. Go to https://www.nuget.org/account/apikeys
3. Click "Create" button
4. Fill in:
   - **Key Name**: `ModularNet Publishing`
   - **Expiration**: 365 days
   - **Select Scopes**: Check `Push new packages and package versions`
   - **Glob Pattern**: `ModularNet.*`
5. Click "Create" and **COPY THE KEY** (you won't see it again!)

### Step 2: Publish the Package

Open PowerShell or CMD and run:

```bash
cd C:\Users\Banaple75\source\repos\ModularNet

dotnet nuget push .\nupkgs\ModularNet.Core.1.0.0.nupkg ^
  --api-key YOUR_API_KEY_HERE ^
  --source https://api.nuget.org/v3/index.json
```

**Replace `YOUR_API_KEY_HERE` with your actual API key**

### Step 3: Verify

1. Wait 10-15 minutes for package indexing
2. Go to https://www.nuget.org/packages/ModularNet.Core
3. Your package should be live!

## 🧪 Test Before Publishing (Optional)

Create a test project:
```bash
cd C:\temp
dotnet new console -n TestModularNet
cd TestModularNet
dotnet add package ModularNet.Core --version 1.0.0 --source C:\Users\Banaple75\source\repos\ModularNet\nupkgs
dotnet build
```

## 📦 What's Included in the Package

- ✅ ModularNet.Core.dll (framework library)
- ✅ XML documentation file
- ✅ README.md (visible on NuGet.org)
- ✅ MIT License
- ✅ All metadata (version, authors, tags, etc.)

## 📝 Package Information

- **Package ID**: ModularNet.Core
- **Version**: 1.0.0
- **Authors**: Banaple75
- **License**: MIT
- **Tags**: aspnetcore, nestjs, modular, framework, dependency-injection, interceptor, pipe, aop
- **Target Framework**: .NET 10.0
- **Repository**: https://github.com/Banaple75/ModularNet

## 🔄 Publishing Updates

When you want to publish version 1.0.1 or later:

1. Update version in `ModularNet.Core/ModularNet.Core.csproj`:
   ```xml
   <Version>1.0.1</Version>
   ```

2. Update `CHANGELOG.md` with changes

3. Rebuild and publish:
   ```bash
   dotnet pack ModularNet.Core/ModularNet.Core.csproj --configuration Release --output ./nupkgs
   dotnet nuget push .\nupkgs\ModularNet.Core.1.0.1.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

## 🛡️ Security Tips

- ❌ **NEVER** commit your API key to Git
- ✅ Store API key in environment variable:
  ```powershell
  $env:NUGET_API_KEY = "your-key-here"
  dotnet nuget push .\nupkgs\ModularNet.Core.1.0.0.nupkg --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json
  ```
- ✅ The `nupkgs/` folder is already in `.gitignore`

## 📚 For Detailed Instructions

See `PUBLISHING.md` for comprehensive guide including:
- CI/CD automation with GitHub Actions
- Troubleshooting common issues
- Package validation
- And more...

## ❓ Common Issues

**"Package already exists"**
- You can't republish the same version
- Increment version number and rebuild

**"Authentication failed"**
- Check your API key is correct
- Verify it hasn't expired

**"Package contains invalid metadata"**
- All required fields are already filled
- This shouldn't happen

## 🎉 After Publishing

1. Create a GitHub release (tag: `v1.0.0`)
2. Add release notes from CHANGELOG.md
3. Share the NuGet package link!

Users can then install your package:
```bash
dotnet add package ModularNet.Core
```

---

**Need help?** Check `PUBLISHING.md` for detailed instructions!
