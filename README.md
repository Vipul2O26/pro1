# 🎓 Paper Drawing System

A web-based **Question Paper Drawing System** built using **ASP.NET MVC** and **SQL Server**, designed to help faculty easily generate question papers by selecting or randomly drawing questions from a question bank.

---

## 📌 Project Description

The Paper Drawing System helps colleges and universities manage and generate question papers more efficiently. Faculty members can either manually select questions or randomly draw them from a pre-populated question bank categorized by units and semesters.

---

## 👨‍💻 Roles & Permissions

- **Admin**
  - Manage users (Faculty)
  - View audit logs and history
  - Upload subjects, units, and question banks

- **Faculty**
  - View assigned subjects and units
  - Add MCQs or Descriptive questions
  - Generate question papers (manual or random mode)
  - Download papers as **PDF** or **Word**
  - View history of generated papers

---

## 🧩 Key Features

- Authentication with role-based access (Admin & Faculty)
- Upload MCQ and Descriptive questions via form or CSV
- Categorization by Semester, Subject, and Unit
- Random question drawing feature
- Preview and export papers in PDF and Word format
- History tracking of generated question papers
- Audit logs for admin to monitor faculty actions

---

## 🛠️ Technologies Used

- **Backend**: ASP.NET MVC (C#)
- **Frontend**: HTML5, CSS3, Bootstrap 5, Razor Views
- **Database**: SQL Server Management Studio (SSMS)
- **PDF Generation**: Rotativa / iTextSharp (choose one)
- **Authentication**: ASP.NET Identity or Custom Login Logic
- **Version Control**: Git + GitHub

---

## 🔧 Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/pro1.git
   cd pro1





2. Open the solution

  >-Use Visual Studio 2022 or later

  >-Open .sln file

3. Update DB connection

In appsettings.json or web.config, update your SQL Server connection string

4. Apply Migrations

   >-Update-Database
   >-Run the project


5. Create account for different user like given below for testing 
Admin Login:
Email: admin@example.com
Password: Admin@123

Faculty Login:
Email: faculty1@example.com
Password: Faculty@123

📸 Screenshots
Include screenshots of:

Login page
<img width="1879" height="939" alt="image" src="https://github.com/user-attachments/assets/c8831bf1-9ca8-43f0-8775-136ca2ac31f0" />



Faculty dashboard
<img width="1027" height="441" alt="image" src="https://github.com/user-attachments/assets/47ed2558-721f-466e-8572-095c69d6191d" />


MCQ upload
<img width="1027" height="441" alt="image" src="https://github.com/user-attachments/assets/c7058de5-5967-4b7a-ae98-087facaa7918" />


Random paper generation
<img width="1027" height="441" alt="image" src="https://github.com/user-attachments/assets/17f8038b-b30c-4bea-99f0-ec6dde7a04cc" />


Generated paper preview
<img width="1027" height="441" alt="image" src="https://github.com/user-attachments/assets/cc3485ee-22b5-4720-985c-215f035eb739" />



📚 Future Enhancements
Add Student role with practice/mock tests

Enable file upload for question images

Multi-language paper support (e.g., Gujarati)

Integration with email for sending generated papers




👨‍🎓 Developed By
Vipul Sodhaparmar
Web Developer | ASP.NET | Angular | SQL | PHP
GitHub | LinkedIn
