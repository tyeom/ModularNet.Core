# ModularNet

A NestJS-inspired modular framework for ASP.NET Core that brings declarative programming, modular architecture, and enhanced developer experience to .NET web applications.

[![NuGet Version](https://img.shields.io/nuget/v/ModularNet.Core.svg)](https://www.nuget.org/packages/ModularNet.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ModularNet.Core.svg)](https://www.nuget.org/packages/ModularNet.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/github/stars/tyeom/ModularNet.Core?style=social)](https://github.com/tyeom/ModularNet.Core)

## 🎯 Overview

ModularNet is a lightweight framework built on top of ASP.NET Core that provides:

- **Modular Architecture**: Organize your application into feature modules with clear boundaries
- **Declarative Programming**: Use attributes to define routes, interceptors, and pipes
- **Enhanced DI**: Automatic service registration with `[Injectable]` attribute
- **Interceptors**: AOP-style cross-cutting concerns (logging, caching, authentication)
- **Pipes**: Reusable parameter transformation and validation
- **Reduced Boilerplate**: No need for `ControllerBase`, `IActionResult`, or verbose route definitions

## 📦 Installation

Install ModularNet.Core via NuGet:

```bash
dotnet add package ModularNet.Core
```

Or via Package Manager Console:

```powershell
Install-Package ModularNet.Core
```

## 🚀 Quick Start

### 1. Create a Service

```csharp
[Injectable(ServiceScope.Singleton)]
public class ProductService : IProductService
{
    public IEnumerable<Product> GetAll()
    {
        // Business logic here
    }
}
```

### 2. Create a Controller

```csharp
[Controller("products")]
[UseInterceptors(typeof(LoggingInterceptor))]
public class ProductController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Get]
    public IEnumerable<Product> GetAllProducts()
    {
        return _productService.GetAll();
    }

    [Get("{id}")]
    public Product GetById([Pipe(typeof(ParseIntPipe))] int id)
    {
        return _productService.GetById(id);
    }

    [Post]
    public Product Create([Pipe(typeof(ValidationPipe))] CreateProductDto dto)
    {
        return _productService.Create(dto);
    }
}
```

### 3. Create a Module

```csharp
[Module(
    Controllers = [typeof(ProductController)],
    Providers = [typeof(ProductService)]
)]
public class ProductModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddScoped<IProductService, ProductService>();
    }
}
```

### 4. Bootstrap Application

```csharp
// Program.cs
var app = ModularAppFactory.CreateApp<AppModule>(args);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.Run();
```

## ✨ Key Features

### 1. Module System

Organize your application into self-contained, reusable modules inspired by NestJS.

```csharp
[Module(
    Imports = [typeof(ProductModule), typeof(AuthModule)],
    Controllers = [typeof(WeatherController), typeof(UserController)],
    Providers = [typeof(WeatherService), typeof(LoggingInterceptor)]
)]
public class AppModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        // Module-specific configuration
    }

    public override void ConfigureApp(IApplicationBuilder app)
    {
        base.ConfigureApp(app);
        // Module-specific middleware
    }
}
```

**Benefits:**
- Clear module boundaries for large applications
- Reusable feature modules
- Explicit dependency management via `Imports`
- Better team collaboration with isolated modules

### 2. Enhanced Dependency Injection

Automatic service registration using the `[Injectable]` attribute.

```csharp
[Injectable(ServiceScope.Singleton)]
public class CachingService : ICachingService
{
    // Automatically registered as Singleton
}

[Injectable(ServiceScope.Scoped)]
public class UserService : IUserService
{
    // Automatically registered as Scoped
}

[Injectable(ServiceScope.Transient)]
public class TransientService
{
    // Automatically registered as Transient
}
```

**Supported Scopes:**
- `ServiceScope.Singleton` - Single instance for application lifetime
- `ServiceScope.Scoped` - Instance per HTTP request
- `ServiceScope.Transient` - New instance every time

### 3. Interceptors (AOP)

Implement cross-cutting concerns with a clean, composable interceptor pattern.

```csharp
[Injectable(ServiceScope.Scoped)]
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        var methodName = $"{context.ControllerType.Name}.{context.Method.Name}";

        _logger.LogInformation("Before executing {MethodName}", methodName);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await next.HandleAsync();
            stopwatch.Stop();

            _logger.LogInformation("After executing {MethodName} - took {ElapsedMs}ms",
                methodName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {MethodName}", methodName);
            throw;
        }
    }
}
```

**Apply interceptors at different levels:**

```csharp
// Controller level - applies to all methods
[Controller("products")]
[UseInterceptors(typeof(AuthInterceptor), typeof(LoggingInterceptor))]
public class ProductController { }

// Combine multiple interceptors
[Controller("weather")]
[UseInterceptors(typeof(LoggingInterceptor), typeof(CachingInterceptor))]
public class WeatherController { }
```

**Example: Authentication Interceptor**

```csharp
[Injectable(ServiceScope.Scoped)]
public class AuthInterceptor : IInterceptor
{
    private const string API_KEY_HEADER = "X-API-Key";

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKey))
        {
            throw new UnauthorizedException("API key is required");
        }

        if (!IsValidApiKey(apiKey))
        {
            throw new UnauthorizedException("Invalid API key");
        }

        return await next.HandleAsync();
    }
}
```

**Example: Caching Interceptor**

```csharp
[Injectable(ServiceScope.Singleton)]
public class CachingInterceptor : IInterceptor
{
    private readonly ConcurrentDictionary<string, (object? Result, DateTime Expiry)> _cache = new();

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        if (context.HttpContext.Request.Method != "GET")
            return await next.HandleAsync();

        var cacheKey = GenerateCacheKey(context);

        if (_cache.TryGetValue(cacheKey, out var cached) && cached.Expiry > DateTime.UtcNow)
        {
            return cached.Result;
        }

        var result = await next.HandleAsync();
        _cache[cacheKey] = (result, DateTime.UtcNow.AddMinutes(5));

        return result;
    }
}
```

### 4. Pipes (Parameter Transformation & Validation)

Reusable parameter transformation and validation logic.

**Built-in Pipes:**
- `ParseIntPipe` - String to integer conversion with optional default value
- `ParseBoolPipe` - String to boolean conversion
- `ParseDoublePipe` - String to double conversion
- `ValidationPipe` - DataAnnotations validation

```csharp
// Type conversion with default value
[Get]
public IEnumerable<User> GetUsers(
    [Pipe(typeof(ParseIntPipe), 10)] int limit,    // default: 10
    [Pipe(typeof(ParseIntPipe), 0)] int offset)    // default: 0
{
    return _userService.GetAll(limit, offset);
}

// Automatic validation
[Post]
public Product Create([Pipe(typeof(ValidationPipe))] CreateProductDto dto)
{
    // dto is automatically validated using DataAnnotations
    return _productService.Create(dto);
}

// Multiple pipes on same parameter
[Put("{id}")]
public Product Update(
    [Pipe(typeof(ParseIntPipe))] int id,
    [Pipe(typeof(ValidationPipe))] UpdateProductDto dto)
{
    return _productService.Update(id, dto);
}
```

**Custom Pipe Example:**

```csharp
public class TrimStringPipe : IPipeTransform
{
    public Task<object?> TransformAsync(object? value, Type targetType)
    {
        if (value is string str)
        {
            return Task.FromResult<object?>(str.Trim());
        }
        return Task.FromResult(value);
    }
}
```

### 5. Declarative Routing

Clean, intuitive routing with minimal boilerplate.

```csharp
[Controller("api/users")]
public class UserController
{
    [Get]                           // GET /api/users
    public IEnumerable<User> GetAll() { }

    [Get("{id}")]                   // GET /api/users/{id}
    public User GetById([Pipe(typeof(ParseIntPipe))] int id) { }

    [Post]                          // POST /api/users
    public User Create(CreateUserDto dto) { }

    [Put("{id}")]                   // PUT /api/users/{id}
    public User Update(
        [Pipe(typeof(ParseIntPipe))] int id,
        UpdateUserDto dto) { }

    [Delete("{id}")]                // DELETE /api/users/{id}
    public void Delete([Pipe(typeof(ParseIntPipe))] int id) { }

    [Patch("{id}")]                 // PATCH /api/users/{id}
    public User Patch(
        [Pipe(typeof(ParseIntPipe))] int id,
        PatchUserDto dto) { }
}
```

### 6. Exception Handling

Centralized exception handling with appropriate HTTP status codes.

```csharp
// Built-in exceptions
throw new BadRequestException("Invalid input data");          // 400
throw new UnauthorizedException("Invalid API key");           // 401
throw new NotFoundException("Product not found");             // 404
throw new HttpException(409, "Resource already exists");      // 409

// Automatic error response format
{
  "statusCode": 400,
  "message": "Invalid input data",
  "type": "BadRequestException"
}
```

### 7. Parameter Binding

Automatic parameter binding from multiple sources.

```csharp
[Get("{id}")]
public Product Get(
    [Pipe(typeof(ParseIntPipe))] int id,        // From route
    [Pipe(typeof(ParseIntPipe), 10)] int limit, // From query string
    string? search)                              // From query string
{
    // Automatic binding and conversion
}

[Post]
public Product Create(CreateProductDto dto)  // From request body (JSON)
{
    // Automatic deserialization
}
```

## 🎨 Architecture

```
┌─────────────────────────────────────────────────┐
│              Application Layer                   │
│                 (Program.cs)                     │
└────────────────────┬────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────┐
│              Root Module                         │
│               (AppModule)                        │
├─────────────────────────────────────────────────┤
│  Imports: [ProductModule, AuthModule]           │
│  Controllers: [WeatherController, UserController]│
│  Providers: [Services, Interceptors]            │
└────────┬────────────────────────┬────────────────┘
         │                        │
         ▼                        ▼
┌─────────────────┐      ┌─────────────────┐
│ ProductModule   │      │   AuthModule    │
├─────────────────┤      ├─────────────────┤
│ Controllers     │      │ Interceptors    │
│ Services        │      │ Guards          │
│ Models          │      │ Strategies      │
└─────────────────┘      └─────────────────┘

Request Flow:
HTTP Request → Middleware → Module Router → Interceptor Chain
→ Parameter Binding → Pipe Transformation → Controller Method
→ Interceptor Chain → Response
```

## 📊 Comparison with ASP.NET Core

| Feature | Traditional ASP.NET Core | ModularNet |
|---------|-------------------------|------------|
| **Module Organization** | Manual organization | Built-in module system with explicit imports |
| **Routing** | `[Route]`, `[HttpGet]` attributes | `[Controller]`, `[Get]` - more concise |
| **Base Class** | Must inherit `ControllerBase` | Plain classes - no inheritance required |
| **Return Types** | `IActionResult`, `ActionResult<T>` | Direct type return - cleaner signatures |
| **DI Registration** | Manual in `Program.cs` | Automatic with `[Injectable]` |
| **Cross-cutting Concerns** | Action Filters, Middleware | Composable Interceptors (cleaner AOP) |
| **Parameter Validation** | ModelState, ActionFilters | Reusable Pipes |
| **Boilerplate** | High (many attributes, base classes) | Low (minimal attributes) |

**Traditional ASP.NET Core:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _service.GetById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}

// In Program.cs
builder.Services.AddScoped<IProductService, ProductService>();
```

**ModularNet:**
```csharp
[Controller("products")]
public class ProductController
{
    [Get("{id}")]
    public Product GetById([Pipe(typeof(ParseIntPipe))] int id)
    {
        return _service.GetById(id); // throws NotFoundException if null
    }
}

// Automatic registration with [Injectable]
[Injectable(ServiceScope.Scoped)]
public class ProductService : IProductService { }
```

## 🌟 Advantages

### 1. **Better Code Organization**
- Feature modules keep related code together
- Clear module boundaries improve maintainability
- Explicit dependencies via `Imports`

### 2. **Reduced Boilerplate**
- No `ControllerBase` inheritance
- No `IActionResult` wrapping
- Automatic service registration
- Concise routing attributes

### 3. **Declarative Programming**
- Express intent clearly through attributes
- Less imperative plumbing code
- Self-documenting API structure

### 4. **Composable Cross-cutting Concerns**
- Interceptors are easier to compose than Filters
- Clean separation of concerns
- Reusable across modules

### 5. **Developer Experience**
- Familiar to NestJS developers
- Lower learning curve for Node.js → .NET transitions
- Consistent patterns across the framework

### 6. **Reusability**
- Pipes are highly reusable
- Modules can be packaged and shared
- Interceptors work across different contexts

### 7. **Testability**
- Plain classes are easier to test
- Interceptors can be tested in isolation
- Module boundaries enable focused testing

## 📦 Sample Application

The `ModularNet.Sample` project demonstrates all features:

### Project Structure
```
ModularNet.Sample/
├── Controllers/
│   ├── ProductController.cs     # CRUD with auth & validation
│   ├── WeatherController.cs     # Caching example
│   └── UserController.cs        # Basic routing
├── Services/
│   ├── ProductService.cs        # Business logic
│   └── WeatherService.cs        # Data provider
├── Models/
│   ├── Product.cs               # Domain model
│   ├── CreateProductDto.cs      # DTO with validation
│   └── UpdateProductDto.cs      # Partial update DTO
├── Modules/
│   ├── AppModule.cs             # Root module
│   ├── ProductModule.cs         # Product feature module
│   └── AuthModule.cs            # Authentication module
├── Interceptors/
│   ├── LoggingInterceptor.cs    # Request/response logging
│   ├── AuthInterceptor.cs       # API key authentication
│   └── CachingInterceptor.cs    # GET request caching
└── api-tests.http               # REST client test file
```

### Running the Sample

```bash
dotnet run --project ModularNet.Sample
```

The application runs at `http://localhost:5116` (or your configured port).

## 🧪 API Examples

### Weather API (with Caching)
```bash
# Get weather forecasts (cached for 5 minutes)
GET http://localhost:5116/weather?count=5

# Get forecast by days
GET http://localhost:5116/weather/7
```

### User API (Basic CRUD)
```bash
# Get user by ID
GET http://localhost:5116/users/123

# List users with pagination
GET http://localhost:5116/users?limit=10&offset=0

# Create user
POST http://localhost:5116/users
Content-Type: application/json

{
  "name": "John Doe"
}

# Update user
PUT http://localhost:5116/users/123
Content-Type: application/json

{
  "name": "Jane Doe"
}

# Delete user
DELETE http://localhost:5116/users/123
```

### Product API (with Authentication & Validation)

```bash
# Get all products (requires API key)
GET http://localhost:5116/products
X-API-Key: secret-api-key-12345

# Get product by ID
GET http://localhost:5116/products/1
X-API-Key: secret-api-key-12345

# Create product (with validation)
POST http://localhost:5116/products
X-API-Key: secret-api-key-12345
Content-Type: application/json

{
  "name": "Gaming Laptop",
  "description": "High-performance laptop for gamers",
  "price": 1299.99,
  "stock": 10
}

# Update product
PUT http://localhost:5116/products/1
X-API-Key: secret-api-key-12345
Content-Type: application/json

{
  "price": 1199.99,
  "stock": 8
}

# Delete product
DELETE http://localhost:5116/products/1
X-API-Key: secret-api-key-12345
```

### Error Handling Examples

```bash
# 401 - Unauthorized (missing API key)
GET http://localhost:5116/products

# Response:
{
  "statusCode": 401,
  "message": "API key is required",
  "type": "UnauthorizedException"
}

# 404 - Not Found
GET http://localhost:5116/products/999
X-API-Key: secret-api-key-12345

# Response:
{
  "statusCode": 404,
  "message": "Product with ID 999 not found",
  "type": "NotFoundException"
}

# 400 - Validation Error
POST http://localhost:5116/products
X-API-Key: secret-api-key-12345
Content-Type: application/json

{
  "name": "AB",
  "price": -10
}

# Response:
{
  "statusCode": 400,
  "message": "Validation failed: Name must be at least 3 characters, Price must be greater than 0",
  "type": "BadRequestException"
}
```

## 🔧 Advanced Features

### Custom Pipes

Create your own pipes for specific transformation logic:

```csharp
public class ToUpperCasePipe : IPipeTransform
{
    public Task<object?> TransformAsync(object? value, Type targetType)
    {
        if (value is string str)
        {
            return Task.FromResult<object?>(str.ToUpperInvariant());
        }
        return Task.FromResult(value);
    }
}

// Usage
[Get]
public string Search([Pipe(typeof(ToUpperCasePipe))] string query)
{
    // query is automatically converted to uppercase
}
```

### Module Composition

Build complex applications by composing modules:

```csharp
// Shared module
[Module(Providers = [typeof(EmailService), typeof(SmsService)])]
public class NotificationModule : ModuleBase { }

// Feature modules
[Module(
    Imports = [typeof(NotificationModule)],
    Controllers = [typeof(OrderController)],
    Providers = [typeof(OrderService)]
)]
public class OrderModule : ModuleBase { }

[Module(
    Imports = [typeof(NotificationModule)],
    Controllers = [typeof(PaymentController)],
    Providers = [typeof(PaymentService)]
)]
public class PaymentModule : ModuleBase { }

// Root module
[Module(Imports = [typeof(OrderModule), typeof(PaymentModule)])]
public class AppModule : ModuleBase { }
```

### Global Interceptors

Apply interceptors to all controllers:

```csharp
public class AppModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        // Register global interceptors
        services.AddScoped<IInterceptor, GlobalLoggingInterceptor>();
    }
}
```

## 🛠️ Development

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio 2022 or VS Code

### Building the Project
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

## 📝 Use Cases

**ModularNet is ideal for:**
- Microservices architecture with clear module boundaries
- Teams familiar with NestJS wanting to transition to .NET
- Projects requiring strong separation of concerns
- Applications with many cross-cutting concerns (auth, logging, caching)
- When you prefer declarative over imperative code

**Stick with traditional ASP.NET Core if:**
- Maximum performance is critical (ModularNet uses reflection)
- You need full control over every aspect
- Your team is deeply invested in ASP.NET patterns
- You're building a simple CRUD API with minimal abstractions

## 🤝 Contributing

Contributions are welcome! This is an educational/experimental framework showcasing alternative patterns for .NET web development.

## 📄 License

MIT License - feel free to use this in your own projects.

## 🙏 Acknowledgments

Inspired by [NestJS](https://nestjs.com/) - A progressive Node.js framework for building efficient and scalable server-side applications.

---

**Note**: ModularNet is an experimental framework built on top of ASP.NET Core. It demonstrates alternative architectural patterns and may not be suitable for production use without thorough testing and performance evaluation.
