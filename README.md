# TasksAPI (.NET 8 Minimal API)

A simple, secure, and clean task management API built using .NET 8, Entity Framework Core, and JWT authentication.

## 🔧 Features

- ✅ Register and login using secure HMAC password hashing
- ✅ JWT-based token authentication
- ✅ SQLite with EF Core for storage
- ✅ Protected routes with `[Authorize]`
- ✅ Swagger/OpenAPI documentation and testing
- ✅ Minimal API approach in .NET 8

---

## 🚀 Tech Stack

- .NET 8 Minimal APIs
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger
- HMACSHA256 for password hashing

---

## 🔐 Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|----------------|-------------|
| GET | `/` | ❌ | Health check |
| POST | `/register` | ❌ | User registration |
| POST | `/login` | ❌ | User login and token issuance |
| GET | `/secure` | ✅ | Protected route (returns message if authorized) |

---

## 🧪 Testing with Swagger

1. Run the app
2. Go to `https://localhost:<port>/swagger`
3. Register a user
4. Login to get JWT token
5. Authorize using **Bearer token**
6. Access `/secure` to confirm protected route

---

## 📂 Project Structure

TasksAPI/
├── Data/ (DbContext)
├── Models/ (User model)
├── DTOs/ (Request models)
├── Program.cs (Main logic)
└── JwtSettings.cs (JWT config)


---

## 👤 Author

Imran Bhatti — Backend Developer & Product Owner  
🔗 GitHub: [@imransharp](https://github.com/imransharp)  
