# Article-review-system-api
The **Article Review System API** is a backend service built using ASP.NET Core and Entity Framework. It provides RESTful endpoints for managing users, articles, reviews, and review requests in an article review system.

## Table of Contents
1. [Features](#features)
2. [Prerequisites](#prerequisites)
3. [Installation](#installation)
4. [Configuration](#configuration)
5. [Endpoints](#endpoints)

---

### **Features**
- User authentication and role-based authorization (Admin, Author, Reviewer).
- CRUD operations for managing users, articles, reviews, and review requests.
- File upload support for article submissions (PDF files).
- Admin dashboard for managing users, articles, and reviews.
- Integration with PostgreSQL database.

---

### **Prerequisites**
Before running the API, ensure you have the following installed:
- [.NET SDK 6 or later](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/).

---

### **Installation**
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/article-review-system-api.git
   cd article-review-system-api
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run migrations to set up the database:
   ```bash
   dotnet ef database update
   ```

5. Start the API:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:5186`.

---

### **Configuration**
1. **Environment Variables**:
   - Create a `.env` file in the root directory and add the following:
     ```env
     ConnectionStrings__DefaultConnection=Host=localhost;Database=ArticleReviewSystem;Username=your_username;Password=your_password
     JwtSettings__SecretKey=your_jwt_secret_key
     ```
   - Replace `your_username`, `your_password`, and `your_jwt_secret_key` with your PostgreSQL credentials and a secure JWT secret key.

2. **Database Setup**:
   - Ensure PostgreSQL is running and accessible.
   - Update the connection string in `.env` if necessary.

---

### **Endpoints**
Here are some key API endpoints:

#### Authentication
- **Register a new user**:
  ```bash
  POST /api/auth/register
  ```
- **Login and get JWT token**:
  ```bash
  POST /api/auth/login
  ```

#### Articles
- **Get all articles**:
  ```bash
  GET /api/admin/articles
  ```
- **Create a new article**:
  ```bash
  POST /api/articles
  ```
- **Delete an article**:
  ```bash
  DELETE /api/articles/{id}
  ```

#### Reviews
- **Get completed reviews**:
  ```bash
  GET /api/reviews/completed
  ```
- **Delete a review**:
  ```bash
  DELETE /api/reviews/{id}
  ```




