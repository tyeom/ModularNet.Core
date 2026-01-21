# Changelog

All notable changes to ModularNet will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.1] - 2025-01-22

### Changed
- Updated NuGet package metadata with correct author information (tyeom)
- Updated repository URL to https://github.com/tyeom/ModularNet.Core
- Updated copyright information

## [1.0.0] - 2025-01-22

### Added
- Initial release of ModularNet framework
- Module system inspired by NestJS
  - `[Module]` attribute for organizing application structure
  - Support for `Imports`, `Controllers`, and `Providers`
  - Module composition and dependency management
- Enhanced Dependency Injection
  - `[Injectable]` attribute for automatic service registration
  - Support for Singleton, Scoped, and Transient service lifetimes
  - Automatic interface registration
- Interceptor pattern for cross-cutting concerns
  - `IInterceptor` interface for implementing interceptors
  - `[UseInterceptors]` attribute for declarative interceptor application
  - Support for multiple interceptors on controllers and methods
  - Built-in interceptors: LoggingInterceptor, AuthInterceptor, CachingInterceptor
- Pipe system for parameter transformation and validation
  - `IPipeTransform` interface for custom pipes
  - `[Pipe]` attribute for declarative parameter transformation
  - Built-in pipes:
    - `ParseIntPipe` - String to integer conversion with optional default values
    - `ParseBoolPipe` - String to boolean conversion
    - `ParseDoublePipe` - String to double conversion
    - `ValidationPipe` - DataAnnotations validation
- Declarative routing
  - `[Controller]` attribute for defining route prefix
  - HTTP method attributes: `[Get]`, `[Post]`, `[Put]`, `[Delete]`, `[Patch]`
  - Route parameter and query string automatic binding
- Exception handling middleware
  - Global exception handler with automatic HTTP status code mapping
  - Built-in exceptions:
    - `BadRequestException` (400)
    - `UnauthorizedException` (401)
    - `NotFoundException` (404)
    - `HttpException` (custom status codes)
  - Consistent error response format
- Sample application demonstrating all features
  - ProductModule with CRUD operations
  - AuthModule with API key authentication
  - WeatherController with caching
  - UserController with basic routing
  - Complete API test suite

### Documentation
- Comprehensive README with examples and comparisons
- Sample application with working examples
- API test file (api-tests.http) for REST Client

[1.0.1]: https://github.com/tyeom/ModularNet.Core/releases/tag/v1.0.1
[1.0.0]: https://github.com/tyeom/ModularNet.Core/releases/tag/v1.0.0
