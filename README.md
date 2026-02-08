# .NET 8 Minimal API Mock Server

MockServer extension with FluentValidation, AutoMapper, versioning and logging middleware

## 🛠️ Dependencies

- .NET 8 Minimal API
- EF Core + SQLite (file-based)
- Bogus for seed data
- AutoMapper for DTO <-> Entity mapping
- FluentValidation for DTO validation
- Custom middleware for logging request/response
- URL-based API versioning (e.g. /api/v1/)
- Example of complex entities: 
  - Customer 1:N Order 
  - Product many-to-many Tag (join ProductTag)

## 📚 Endpoints

### Entity: Product

| Method | Endpoint |
|--------|----------|
| GET | /api/v1/products |
| GET | /api/v1/products/{id} |
| POST | /api/v1/products |
| PUT | /api/v1/products/{id} |
| DELETE | /api/v1/products/{id} |

### Entity: Customer

| Method | Endpoint |
|--------|----------|
| GET | /api/v1/customers
| POST | /api/v1/customers

### Entity: Order

| Method | Endpoint |
|--------|----------|
| GET | /api/v1/orders
| POST | /api/v1/orders

## ⚙️ How to extend with a new entity

1. Add a new entity in Models and register it in AppDbContext (OnModelCreating if configuration is needed).
2. Add DTO and AutoMapper mappings (MappingProfile).
3. Add a FluentValidation validator for Create/Update DTO.
4. (Optional) Register IEntityFaker<T> for better seeding.
5. Add routing by calling the MapEntityEndpoints function or creating custom endpoints if include/relationships are needed.

## 💡 Note

- The generic Update copies all writable properties except Id. For more complex cases, create specific endpoints or use specific DTOs.
- For validation in the minimal API routes, I used IValidator<T> and Output Results.ValidationProblem when validation fails.
- The logging middleware records the request and response body—be careful with sensitive data in production.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⭐ Give a Star

Don't forget that if you find this project helpful, please give it a ⭐ on GitHub to show your support and help others discover it.