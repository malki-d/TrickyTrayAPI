# Tricky Tray API

מערכת API לניהול מכירות והגרלות Tricky Tray.

## 🚀 התקנה והפעלה

### דרישות מקדימות
- .NET 8.0 SDK
- SQL Server
- חשבון Gmail (לשליחת מיילים)
- Google OAuth Client ID (להתחברות עם Google)

### שלב 1: Clone הפרויקט
```bash
git clone <repository-url>
cd TrickyTrayAPI
```

### שלב 2: הגדרת קובץ appsettings.Local.json

צור קובץ בשם `appsettings.Local.json` בתיקייה `TrickyTrayAPI/`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com"
    }
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password",
    "SenderName": "מערכת הגרלות Tricky Tray"
  }
}
```

### שלב 3: הגדרת Gmail App Password

1. גש ל-[Google Account Security](https://myaccount.google.com/security)
2. הפעל **2-Step Verification**
3. גש ל-[App Passwords](https://myaccount.google.com/apppasswords)
4. צור App Password חדש בשם "Tricky Tray"
5. העתק את הסיסמה (16 תווים) ל-`SenderPassword` בקובץ `appsettings.Local.json`

### שלב 4: הגדרת Google OAuth

1. גש ל-[Google Cloud Console](https://console.cloud.google.com/)
2. צור פרויקט חדש או בחר קיים
3. הפעל את Google+ API
4. צור OAuth 2.0 Client ID
5. הוסף `http://localhost:4200` ל-Authorized JavaScript origins
6. העתק את ה-Client ID ל-`appsettings.Local.json`

### שלב 5: הגדרת מסד נתונים

עדכן את connection string ב-`Program.cs` (שורה ~96):
```csharp
options.UseSqlServer("Server=YOUR_SERVER;Database=TrickyTrayDB;Trusted_Connection=True;TrustServerCertificate=True;")
```

הרץ את ה-migrations:
```bash
dotnet ef database update
```

### שלב 6: הרצת השרת

```bash
dotnet run
```

השרת יעלה על: https://localhost:7260

## 📧 פיצ'רים

- ✅ הרשמה והתחברות (רגילה + Google OAuth)
- ✅ ניהול מתנות וקטגוריות
- ✅ מערכת עגלה ורכישות
- ✅ הגרלות אוטומטיות
- ✅ שליחת מיילים:
  - מייל ברוכים הבאים בהרשמה
  - מייל זכייה בהגרלה
- ✅ ייצוא דוחות (CSV/Excel)

## 🔒 אבטחה

⚠️ **חשוב!** קובץ `appsettings.Local.json` מכיל סיסמאות ומפתחות סודיים.
הקובץ הזה **לא** צריך להיות ב-Git (הוא ב-.gitignore).

## 📝 מבנה הפרויקט

```
TrickyTrayAPI/
├── Controllers/        # API Controllers
├── Services/          # Business Logic
├── Repositories/      # Data Access Layer
├── Models/           # Database Models
├── DTOs/             # Data Transfer Objects
└── Migrations/       # EF Core Migrations
```

## 🛠️ טכנולוגיות

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Google OAuth 2.0
- Serilog (Logging)
- SMTP Email Service

## 📮 יצירת קשר

לשאלות ותמיכה, פנה למפתחים.
