# Guardia - Smart Personnel & Factory Management System

**Guardia** is an integrated **Personnel and Factory Management System** developed as a group project for the **Directed Study** course in the Computer Programming Department at **Doğuş University**.

The project aims to digitalize Human Resources and Legal Department workflows, improve interdepartmental communication, and increase operational efficiency through a centralized management platform.

> **Note:** This project is currently under active development. Core modules are functional, while improvements and additional features are still in progress.

---

## 🚀 Core Modules

### 1. Human Resources (HR) Management Panel

* **Employee Management:** Centralized employee records, departments, and contact information.
* **Payroll Management:** Salary, overtime, and deduction calculations with automated distribution.
* **Leave Management:** Approval and rejection of leave requests via mobile application.
* **Announcement System:** Company-wide and department-based announcements.

---

### 2. Legal Management Panel

* **Case & Hearing Tracking:** Monitor legal cases and upcoming hearings.
* **Contract Management:** Track contracts, expiration dates, and risk status.
* **Legislation Tracking:** Notify departments about legal updates.
* **Digital Archive:** Secure document storage compliant with data protection regulations (KVKK).

---

### 3. Employee Portal (Mobile & Web)

* **QR Code Attendance System:** Fast check-in/check-out using dynamic QR codes.
* **Fault Reporting System:** Submit technical issues with images, location, and priority level.
* **Self-Service Features:** View payslips, cafeteria menus, and update personal data.

---

### 4. 🤖 AI Assistant (Mistral AI)

A chatbot powered by **Mistral AI** provides automated support for payroll, leave policies, shift schedules, and system-related inquiries using a custom system prompt.

---

## 🛠️ Technologies Used

### Backend

* C#
* ASP.NET Core Web API (.NET 9)

### Database

* Microsoft SQL Server
* Entity Framework Core 9

### Frontend (Admin Panels)

* HTML5
* CSS3
* JavaScript (ES6+)
* Fetch API

### Mobile

* Flutter
* Dart

### AI

* Mistral AI API

### Security & Architecture

* N-Tier Architecture
* JWT Authentication
* BCrypt Password Hashing
* Role-Based Access Control (RBAC)

---

## 🧪 Testing & Quality Assurance

### Unit Testing

* Business logic tested with **xUnit**, including payroll, leave workflows, and authentication-related services.

### Mocking & Isolation

* **Moq** used for dependency isolation.
* Custom **MockContextBuilder** implemented to simulate **Entity Framework Core AppDbContext** using in-memory data.

### API & Integration Testing

* Endpoints validated via **Swagger UI**.
* Covered JWT authentication, HTTP status codes, request/response structure, and JSON integrity.
