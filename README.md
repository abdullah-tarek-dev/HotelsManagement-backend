# 🏨 HotelsManagement-backend

A hotel management system built using **ASP.NET Core Web API** and **PostgreSQL**.  
This backend project provides RESTful APIs to manage hotel rooms, bookings, customers, and more.

> ⚠️ This project is currently under development (Work In Progress)

---

## 💡 Project Overview

This API aims to simplify hotel operations by providing a backend that supports:

- Room management (Create / Update / Delete)
- Booking management
- Customer information handling
- Room availability checking
- Future support for staff, payments, reviews, etc.

---

## 🛠️ Tech Stack

| Area              | Technology / Tool               |
|-------------------|----------------------------------|
| Backend Framework | ASP.NET Core Web API             |
| Language          | C#                               |
| ORM               | Entity Framework Core            |
| Database          | PostgreSQL                       |
| Documentation     | Swagger                          |
| Development IDE   | Visual Studio / VS Code          |
| API Testing       | Postman / Swagger UI             |
| Version Control   | Git & GitHub                     |

---

## 📁 Project Structure

```bash
HotelsManagement-backend/
│
├── Hotel_Managment.sln            # Solution file
└── Hotel_Managment/               # Main project folder
    ├── Controllers/               # API Controllers
    ├── Models/                    # Entity Models / DTOs
    ├── Services/                  # Business Logic Layer
    ├── Repositories/             # Data Access Layer
    ├── Data/                      # DbContext and configuration
    ├── appsettings.json           # App configuration (DB, etc.)
    ├── Program.cs                 # App entry point
    └── ...                        # Other supporting files




🚀 Getting Started (Run Locally)
✅ Prerequisites

.NET SDK (6 or higher)

PostgreSQL (running locally or remotely)

Visual Studio / VS Code

EF Core CLI

⚙️ Setup Instructions

Clone the repository

git clone https://github.com/abdullah-tarek-dev/HotelsManagement-backend.git
cd HotelsManagement-backend


Configure the database connection

Open the file: Hotel_Managment/appsettings.json and update the connection string:

"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=HotelsDB;Username=postgres;Password=yourpassword"
}


✅ Make sure a database named HotelsDB exists in your PostgreSQL instance.

Apply Migrations

If migrations already exist:

dotnet ef database update


If not, create the initial migration:

dotnet ef migrations add InitialCreate
dotnet ef database update


Run the application

dotnet run --project Hotel_Managment


Access Swagger UI

Open your browser:

https://localhost:<port>/swagger


You can explore and test the API from there.

🔌 Example API Endpoints
Method	Endpoint	Description
GET	/api/rooms	Get all rooms
GET	/api/rooms/{id}	Get room details by ID
POST	/api/rooms	Create a new room
PUT	/api/rooms/{id}	Update room information
DELETE	/api/rooms/{id}	Delete a room
POST	/api/bookings	Create a booking
GET	/api/bookings	Get all bookings
GET	/api/customers	Get all customers

More endpoints and documentation will be added as development progresses.

📌 Roadmap / TODO

 Implement JWT Authentication & Authorization

 Add user roles (Admin, Staff, Guest)

 Enable image upload for room listings

 Add filtering, sorting, and pagination

 Write Unit and Integration Tests

 Deploy production-ready version

 Connect with a Frontend (React / Blazor / etc.)

📸 Screenshots (Coming Soon)

Swagger UI screenshots and request examples will be added here soon.

🤝 Contributing

Contributions, issues, and suggestions are welcome!
Feel free to open an Issue
 or submit a Pull Request.

📄 License

No license has been specified yet.
Intended for educational and development purposes.
An open-source license (MIT / Apache 2.0) may be added in the future.

📬 Contact

GitHub: @abdullah-tarek-dev

(Add your LinkedIn or email here if you'd like)

© 2025 – Abdullah Tarek


---

### ✅ What You Need to Do Now:

1. Create a file in the root of your repo called `README.md`  
2. Paste everything above into it  
3. Commit and push:

```bash
git add README.md
git commit -m "Add full project README"
git push origin main
