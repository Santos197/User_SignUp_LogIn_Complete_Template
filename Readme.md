
##User Sign-Up and Login N-Tier Project

Overview
This project demonstrates a complete user sign-up and login system using an N-tier architecture with ASP.NET Core Web API 8 for the backend and Angular 
15 for the frontend. The system includes functionality for user registration, login, and JWT authentication.

Table of Contents
Overview
Architecture
Technologies Used
Prerequisites
Installation
Configuration
Running the Application
API Endpoints

The project follows an N-tier architecture with the following layers:

Presentation Layer: Angular 15 application
Business Logic Layer: ASP.NET Core Web API 8
Data Access Layer: Entity Framework Core for database interactions
Database: SQL Server

Technologies Used
Backend:
ASP.NET Core Web API 8
Entity Framework Core
JWT for authentication
SQL Server

Frontend:
Angular 15

Prerequisites
.NET SDK 8
Node.js (with npm)
SQL Server
Angular CLI

Installation
Backend
Clone the repository:
git clone https://github.com/Santos197/User_SignUp_LogIn.git
cd your-repo/Backend

Restore dependencies:
dotnet restore
Update the database:
dotnet ef database update

Frontend
cd ../Frontend
Install dependencies:
npm install

Configuration
Backend
Update the appsettings.json file in the Backend project with your SQL Server connection string and JWT settings.

Frontend
Update the environment configuration file (src/environments/environment.ts) with the API base URL.

Running the Application
Backend
cd Backend
dotnet run

Frontend
cd Frontend
ng serve
The Angular application will run on http://localhost:4200 and the ASP.NET Core Web API will run on http://localhost:5000.

API Endpoints
POST /api/Account/Register: User registration
POST /api/Account/Login: User login

Frontend Structure
src/app/components: Contains the Angular components for registration and login
src/app/services: Contains services for API interactions
src/app/models: Contains TypeScript models for user data





