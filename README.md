# User Management System (ASP.NET Web Forms + ADO.NET)

This is a basic User Management System built using **ASP.NET Web Forms**, **C#**, **ADO.NET**, and **SQL Server (SSMS)**. It allows you to manage users and their related orders, store them in a database, and display them in a GridView.

---

## 🔧 Features

- Add new users with:
  - Name
  - Email
  - Age
- Add orders linked to a user:
  - Order Date
- Display user and order data in a GridView
- Store and retrieve data using **ADO.NET**
- SQL Server database integration

---

## 🧱 Tech Stack

- ASP.NET Web Forms
- C# (.NET Framework)
- ADO.NET
- SQL Server Management Studio (SSMS)
- HTML / CSS
- GridView Control

---

## 📁 Folder Structure

```

/UserManagementSystem/
├── App\_Code/
│   └── DataAccess.cs            # ADO.NET SQL logic
├── Users.aspx                   # User form + GridView
├── Orders.aspx                  # Order form + GridView
├── Web.config                   # Connection string config
├── README.md                    # This file

````

---

## 🛠️ Setup Instructions

1. **Clone the project** or download the ZIP.
2. Open in **Visual Studio**.
3. Make sure your **SQL Server** is running.
4. Update the **connection string** in `Web.config`:

```xml
<connectionStrings>
  <add name="UserDb"
       connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=UserDB;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
````

5. Run the SQL script to create the database and tables.

---

## 🗃️ SQL Database Structure

```sql
-- Create User table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Age INT
);

-- Create Order table
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATE
);
```

---

## ✏️ How It Works

* The **Users.aspx** form allows entering new user data.
* The **Orders.aspx** form lets you add orders for a user.
* Both forms use **ADO.NET** in `DataAccess.cs` to interact with the SQL Server database.
* Data is displayed in **GridView** for easy viewing.

---

## 💡 Future Improvements

* Add editing and deleting functionality for users and orders
* Use Stored Procedures instead of inline SQL
* Add dropdowns to link users in the order form
* Add validation and error handling

---

## 📩 Contact

For any questions or suggestions, feel free to contact:

**Vishal Waghmode**
Email: vishalwaghmode247@gmail.com.
---

> You can copy this into a `README.md` file and customize the email, GitHub, and any optional notes based on your project setup.

Would you like me to help you generate the SQL script file too?
```
