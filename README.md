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
   - Create a `appsettings.json` file in the root directory and add the following:
     ```appsettings.json
      {
        "ConnectionStrings": {
          "DefaultConnection": "Host=localhost;Database=ArticleReviewSystem;Username=your_username;Password=your_password"
        },
        "JwtSettings": {
          "SecretKey": "your_jwt_secret_key",
          "Issuer": "https://localhost:5186",
          "Audience": "https://localhost:5186"
        },
        "Logging": {
          "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
          }
        },
      "AllowedHosts": "*"
      }
     ```
   - Replace `your_username`, `your_password`, and `your_jwt_secret_key` with your PostgreSQL credentials and a secure JWT secret key.

2. **Database Setup**:
   - Ensure PostgreSQL is running and accessible.
   - Update the connection string in `appsettings.json` if necessary.

---

### **Endpoints**
Here are some key API endpoints:


## **Base URL**
All API endpoints are relative to the base URL:
```
http://localhost:5186/api
```

For production environments, replace `localhost:5186` with your deployed API's domain.

---

## **Authentication**

### **1. Register a New User**
- **Endpoint**: `POST /auth/register`
- **Description**: Registers a new user in the system.
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "password": "Password@123",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Author",
    "bio": "Sample bio",
    "location": "New York",
    "institution": "Example University",
    "specialization": "Computer Science"
  }
  ```
- **Response**:
  ```json
  {
    "message": "User registered successfully."
  }
  ```

---

### **2. Login and Get JWT Token**
- **Endpoint**: `POST /auth/login`
- **Description**: Authenticates a user and returns a JWT token.
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "password": "Password@123"
  }
  ```
- **Response**:
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
  ```

> **Note**: Include the `Authorization: Bearer <token>` header in subsequent requests for authenticated endpoints.

---

## **Users**

### **1. Get All Users (Admin Only)**
- **Endpoint**: `GET /admin/users`
- **Description**: Retrieves a list of all users (accessible only by admins).
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  [
    {
      "id": 1,
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Author"
    },
    {
      "id": 2,
      "email": "reviewer@example.com",
      "firstName": "Jane",
      "lastName": "Smith",
      "role": "Reviewer"
    }
  ]
  ```

---

### **2. Create a New User (Admin Only)**
- **Endpoint**: `POST /admin/users`
- **Description**: Creates a new user (accessible only by admins).
- **Headers**:
  ```
  Authorization: Bearer <token>
  Content-Type: application/json
  ```
- **Request Body**:
  ```json
  {
    "email": "newuser@example.com",
    "password": "Password@123",
    "firstName": "Alice",
    "lastName": "Johnson",
    "role": "Reviewer",
    "specialization": "Data Science",
    "institution": "Tech University"
  }
  ```
- **Response**:
  ```json
  {
    "id": 3,
    "email": "newuser@example.com",
    "firstName": "Alice",
    "lastName": "Johnson",
    "role": "Reviewer"
  }
  ```

---

### **3. Delete a User (Admin Only)**
- **Endpoint**: `DELETE /admin/users/{id}`
- **Description**: Deletes a user by ID (accessible only by admins).
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "User deleted successfully."
  }
  ```

---

### **4. Block a User (Admin Only)**
- **Endpoint**: `PUT /admin/users/{id}/block`
- **Description**: Blocks a user by ID (accessible only by admins).
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "User blocked successfully."
  }
  ```

---

## **Articles**

### **1. Get Articles by Current Author**
- **Endpoint**: `GET /articles/my`
- **Description**: Retrieves articles submitted by the currently logged-in author.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  [
    {
      "id": 1,
      "title": "Advanced Machine Learning Techniques",
      "category": "Technology",
      "status": "Submitted"
    }
  ]
  ```

---

### **2. Create a New Article**
- **Endpoint**: `POST /articles`
- **Description**: Submits a new article (PDF file upload required).
- **Headers**:
  ```
  Authorization: Bearer <token>
  Content-Type: multipart/form-data
  ```
- **Request Body**:
  - Form Data:
    - `file`: PDF file
    - `title`: "New Article Title"
    - `category`: "Technology"
    - `tags`: `["AI", "Machine Learning"]`
- **Response**:
  ```json
  {
    "message": "Article created successfully."
  }
  ```

---

### **3. Delete an Article**
- **Endpoint**: `DELETE /articles/{id}`
- **Description**: Deletes an article by ID.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "Article deleted successfully."
  }
  ```

---

## **Reviews**

### **1. Get Completed Reviews (Admin Only)**
- **Endpoint**: `GET /reviews/completed`
- **Description**: Retrieves all completed reviews (accessible only by admins).
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  [
    {
      "id": 1,
      "articleId": 2,
      "rating": 4,
      "recommendation": "AcceptWithMinorRevisions",
      "technicalMerit": "Excellent research methodology",
      "originality": "Novel approach to the problem",
      "presentationQuality": "Well-structured paper",
      "commentsToAuthors": "Consider expanding the literature review",
      "status": "Submitted"
    }
  ]
  ```

---

### **2. Delete a Review (Admin Only)**
- **Endpoint**: `DELETE /reviews/{id}`
- **Description**: Deletes a review by ID and sets the associated article status to `"Pending"`.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "Review deleted successfully. Article status set to Pending."
  }
  ```

---

## **Review Requests**

### **1. Get New Review Requests**
- **Endpoint**: `GET /reviewrequests/new`
- **Description**: Retrieves new review requests for the logged-in reviewer.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  [
    {
      "id": 1,
      "articleId": 2,
      "reviewerId": 3,
      "dueDate": "2025-12-31T00:00:00Z",
      "expectedTime": "4-5 hours",
      "pages": 15
    }
  ]
  ```

---

### **2. Accept a Review Request**
- **Endpoint**: `PUT /reviewrequests/{id}/accept`
- **Description**: Accepts a review request by ID.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "Review request accepted successfully."
  }
  ```

---

### **3. Decline a Review Request**
- **Endpoint**: `PUT /reviewrequests/{id}/decline`
- **Description**: Declines a review request by ID.
- **Headers**:
  ```
  Authorization: Bearer <token>
  ```
- **Response**:
  ```json
  {
    "message": "Review request declined successfully."
  }
  ```

---

## **Error Handling**

If an error occurs, the API will return a JSON object with an error message. For example:

```json
{
  "error": "User not found."
}
```

