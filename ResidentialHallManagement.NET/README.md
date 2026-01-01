# BIJOY-24 Residential Hall Management System

A comprehensive web-based hostel/residential hall management system built with ASP.NET Core MVC for university accommodation management.

---

## 🎯 Project Overview

This system digitizes and streamlines the entire process of managing university residential halls, from student registration to room allocation, maintenance requests, and seat booking.

---

## ✨ Key Features

### 👨‍🎓 Student Portal
- **Account Registration & Login** - Secure authentication with ASP.NET Identity
- **Interactive Seat Booking** - Visual room layout with clickable seats
- **Room Application** - Apply for room allocation
- **Room Change Requests** - Request transfers to different rooms
- **Maintenance Requests** - Report issues and track resolution
- **Dashboard** - View allocation status, bookings, and notifications

### 🏢 Hall Admin Portal
- **Student Management** - View and manage students in assigned hall
- **Room Management** - Manage rooms, capacity, and availability
- **Application Processing** - Approve/reject room and seat applications
- **Maintenance Tracking** - Handle maintenance requests
- **Hall Statistics** - Overview of occupancy and activities

### 🔧 System Admin Portal
- **Multi-Hall Management** - Oversee all residential halls
- **Hall Admin Management** - Create and manage hall administrators
- **System-wide Reports** - Analytics across all halls
- **User Management** - Manage all system users

---

## 🛏️ Seat Booking System

### Interactive Room Layout
- **4 Seats per Room** - 2 window-side, 2 door-side
- **Visual Selection** - Click to select available seats
- **Real-time Status** - Color-coded availability indicators
  - 🟢 Green - Available
  - 🔴 Red - Booked
  - 🔵 Blue - Your Selection
  - 🟡 Yellow - Temporarily Held

### Booking Lock System
- **10-minute Hold** - Seats are reserved while you complete booking
- **Auto-release** - Expired holds are automatically freed
- **Double-booking Prevention** - Server-side validation

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|------------|
| **Backend** | ASP.NET Core 8.0 MVC |
| **Database** | MySQL with Entity Framework Core |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | Bootstrap 5, Bootstrap Icons |
| **AJAX** | jQuery for real-time updates |

---

## 📁 Project Structure

```
ResidentialHallManagement.NET/
├── ResidentialHallManagement.Core/     # Domain entities & interfaces
│   ├── Entities/                       # Data models
│   └── Interfaces/                     # Service contracts
├── ResidentialHallManagement.Data/     # Database context & migrations
├── ResidentialHallManagement.Web/      # Web application
│   ├── Controllers/                    # MVC controllers
│   ├── Views/                          # Razor views
│   ├── ViewModels/                     # View models
│   └── wwwroot/                        # Static files (CSS, JS)
├── scripts/                            # SQL migration scripts
└── tests/                              # Integration tests
```

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- MySQL Server
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ResidentialHallManagement.NET
   ```

2. **Configure database connection**
   Update `appsettings.json` with your MySQL connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=hall_management;User=root;Password=yourpassword;"
   }
   ```

3. **Run database migrations**
   ```bash
   # Run SQL scripts from the scripts/ folder
   mysql -u root -p hall_management < scripts/20251228_create_seats_tables.sql
   ```

4. **Build and run**
   ```bash
   dotnet build
   dotnet run --project ResidentialHallManagement.Web
   ```

5. **Access the application**
   - Open browser: `https://localhost:5001` or `http://localhost:5000`

---

## 👥 User Roles

| Role | Access Level |
|------|--------------|
| **Student** | Personal dashboard, seat booking, maintenance requests |
| **Hall Admin** | Manage assigned hall, process applications |
| **System Admin** | Full system access, manage all halls and admins |

---

## 📱 Screenshots

The system features:
- Modern responsive design
- Professional university-style color scheme
- Interactive visual elements
- Mobile-friendly interface

---

## 🔒 Security Features

- Role-based authorization
- CSRF protection on all forms
- Secure password hashing
- Session management
- Input validation and sanitization

---

## 📄 License

This project is developed for educational purposes as a university final year project.

---

## 👨‍💻 Developer

**BIJOY-24 Residential Hall Management System**  
*A comprehensive solution for university hostel management*

---

*Built with ❤️ using ASP.NET Core*
