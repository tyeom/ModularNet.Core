# ModularNet Sample Application

이 샘플 애플리케이션은 ModularNet 프레임워크의 주요 기능들을 보여줍니다.

## 프로젝트 구조

```
ModularNet.Sample/
├── Controllers/        # API 엔드포인트
├── Services/          # 비즈니스 로직
├── Models/            # 도메인 모델 및 DTO
├── Modules/           # 기능별 모듈
├── Interceptors/      # AOP 스타일 Interceptor
└── Program.cs         # 애플리케이션 진입점
```

## 모듈 구조

### 1. AppModule (루트 모듈)
- **역할**: 애플리케이션의 진입점
- **Import**: ProductModule, AuthModule
- **Controllers**: WeatherController, UserController
- **Providers**: WeatherService, LoggingInterceptor

### 2. ProductModule
- **역할**: 제품 관리 기능
- **Controllers**: ProductController
- **Providers**: ProductService
- **기능**:
  - 제품 목록 조회
  - 제품 상세 조회
  - 제품 생성 (검증 포함)
  - 제품 수정
  - 제품 삭제

### 3. AuthModule
- **역할**: 인증/인가 기능 제공
- **Providers**: AuthInterceptor, CachingInterceptor
- **기능**:
  - API Key 기반 인증
  - 캐싱 기능

## API 엔드포인트

### Weather API (캐싱 적용)
```
GET /weather?count=5              # 날씨 예보 조회
GET /weather/{days}               # N일간 날씨 예보
```

### User API
```
GET /users/{id}                   # 사용자 조회
GET /users?limit=10&offset=0     # 사용자 목록 (페이징)
POST /users                       # 사용자 생성
PUT /users/{id}                   # 사용자 수정
DELETE /users/{id}                # 사용자 삭제
```

### Product API (인증 필요)
```
GET /products                     # 제품 목록 조회
GET /products/{id}                # 제품 상세 조회
POST /products                    # 제품 생성
PUT /products/{id}                # 제품 수정
DELETE /products/{id}             # 제품 삭제
```

## 주요 기능 예제

### 1. Pipe를 사용한 파라미터 변환 및 검증

```csharp
[Get("{id}")]
public Product GetProductById([Pipe(typeof(ParseIntPipe))] int id)
{
    // ParseIntPipe가 자동으로 string을 int로 변환
}

[Post]
public Product CreateProduct([Pipe(typeof(ValidationPipe))] CreateProductDto dto)
{
    // ValidationPipe가 자동으로 DataAnnotations 검증 수행
}
```

### 2. Interceptor를 사용한 횡단 관심사 처리

```csharp
// 단일 Interceptor 적용
[Controller("products")]
[UseInterceptors(typeof(AuthInterceptor))]
public class ProductController { }

// 여러 Interceptor 조합
[Controller("weather")]
[UseInterceptors(typeof(LoggingInterceptor), typeof(CachingInterceptor))]
public class WeatherController { }
```

### 3. 모듈 Import로 기능 조합

```csharp
[Module(
    Imports = [typeof(ProductModule), typeof(AuthModule)],
    Controllers = [typeof(WeatherController), typeof(UserController)],
    Providers = [typeof(WeatherService), typeof(LoggingInterceptor)]
)]
public class AppModule : ModuleBase { }
```

## 인증 테스트

Product API는 AuthInterceptor가 적용되어 있어 API Key가 필요합니다.

### 인증 없이 요청 (실패)
```bash
curl http://localhost:5000/products
# Response: 401 Unauthorized
```

### 인증과 함께 요청 (성공)
```bash
curl -H "X-API-Key: secret-api-key-12345" http://localhost:5000/products
# Response: 200 OK with product list
```

## 캐싱 동작 확인

WeatherController는 CachingInterceptor가 적용되어 있습니다.

```bash
# 첫 번째 요청 - DB/서비스에서 데이터 조회
curl http://localhost:5000/weather?count=3

# 두 번째 요청 (5분 이내) - 캐시에서 반환
curl http://localhost:5000/weather?count=3
```

로그를 확인하면 캐시 히트 여부를 확인할 수 있습니다.

## 검증 예제

CreateProductDto에 DataAnnotations가 적용되어 있어 자동 검증됩니다.

### 유효하지 않은 데이터
```bash
curl -X POST http://localhost:5000/products \
  -H "X-API-Key: secret-api-key-12345" \
  -H "Content-Type: application/json" \
  -d '{"name": "AB", "price": -10}'
# Response: 400 Bad Request (이름이 너무 짧고, 가격이 음수)
```

### 유효한 데이터
```bash
curl -X POST http://localhost:5000/products \
  -H "X-API-Key: secret-api-key-12345" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "New Product",
    "description": "A great product",
    "price": 99.99,
    "stock": 100
  }'
# Response: 200 OK with created product
```

## ModularNet의 장점 (이 샘플에서 확인 가능)

1. **모듈화**: ProductModule, AuthModule로 기능 분리
2. **선언적 라우팅**: `[Controller("products")]`, `[Get("{id}")]`
3. **재사용 가능한 Pipe**: ParseIntPipe, ValidationPipe
4. **조합 가능한 Interceptor**: 로깅 + 캐싱 + 인증
5. **명확한 의존성**: Module의 Imports로 명시적 의존성 관리
6. **보일러플레이트 감소**: ControllerBase 상속 불필요

## 실행 방법

```bash
dotnet run --project ModularNet.Sample
```

애플리케이션은 `http://localhost:5000`에서 실행됩니다.
