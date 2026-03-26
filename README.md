# Remote Notification System

A full-stack remote notification delivery system optimized for production environments. This system securely handles pushing notifications to devices, leveraging cross-platform compatibility and enterprise-grade backend architecture.

## 🚀 Tech Stack

- **Backend:** .NET 8 / ASP.NET Core
- **Frontend / Admin Panel:** Angular (or React, depending on configuration)
- **Mobile App:** Flutter
- **Database:** SQL Server
- **Notification Provider:** Firebase Cloud Messaging (FCM)

## 📦 Features

- **Cross-Platform:** Mobile client built in Flutter for iOS and Android.
- **Robust Backend:** .NET 8 API with secure, clean architecture.
- **Admin Dashboard:** Web-based centralized administration.
- **Firebase Integration:** Uses robust FCM APIs for reliable delivery and tracking.

## ⚙️ Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js & npm (for Admin Panel)
- Flutter SDK (for mobile app)
- SQL Server
- Firebase Project configured with FCM

### Backend
1. Navigate to the `backend/NotificationSystem.API` directory.
2. Update the `appsettings.Development.json` (or Environment variables) with your Database string and Firebase server keys.
3. Run `dotnet restore` and `dotnet build`.
4. Run `dotnet run` to start the backend service.

### Admin Panel
1. Navigate to the `admin-panel/angular-notification-admin` directory.
2. Run `npm install` to restore dependencies.
3. Start the server with `npm start` or `ng serve`.

### Mobile App (Flutter)
1. Navigate to the `mobile-app/flutter_notification_app` directory.
2. Run `flutter pub get` to fetch Dart packages.
3. Add your `google-services.json` (Android) and `GoogleService-Info.plist` (iOS) to the respective directories.
4. Run `flutter run` on an emulator or physical device.

## 🔒 Security

All sensitive credentials and environment files have been explicitly removed and ignored from version control for security purposes. Please request access or generate new keys to run this project locally.
