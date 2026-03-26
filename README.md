# 🔔 Remote Notification System

A complete push notification system consisting of a .NET Core Web API backend, Angular Admin Panel, and Flutter Mobile Application integrated with Firebase Cloud Messaging.

---

## 📐 Architecture

```
Angular Admin Panel
     │
     │ POST /api/notification/send
     ▼
.NET Core Web API (N-Tier)
     │
     ├─── Service Layer (saves to DB)
     │         │
     │         ▼
     │    Firebase Cloud Messaging
     │         │
     │         ▼
     │    Flutter App (receives push)
     │
     └─── GET /api/notification
               │
               ▼
          Flutter App (notification list)
```

> 📖 **Note:** For a detailed breakdown of the system components, data sequences, and architecture diagrams, please read:
> - [Architecture Overview](docs/architecture.md)
> - [System Flow & Life Cycle](docs/system-flow.md)

### Backend (N-Tier Architecture)
| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Core** | `NotificationSystem.Core` | Entities, Interfaces |
| **Data** | `NotificationSystem.Data` | DbContext, Repositories (EF Core) |
| **Business** | `NotificationSystem.Business` | Services, DTOs, AutoMapper, FirebaseService |
| **API** | `NotificationSystem.API` | Controllers, Middleware, DI, Swagger |

### Mobile App (MVVM)
| Layer | Folder | Responsibility |
|-------|--------|---------------|
| **Models** | `lib/models/` | Data classes |
| **Services** | `lib/services/` | API calls, Firebase |
| **ViewModels** | `lib/viewmodels/` | State management (ChangeNotifier) |
| **Views** | `lib/views/` | UI screens |

---

## 🛠 Professional Features

- ✅ **N-Tier Architecture** — Clean separation of concerns
- ✅ **Repository Pattern** — Abstracted data access
- ✅ **DTOs** — `NotificationRequestDTO`, `NotificationResponseDTO`, `DeviceRegistrationDTO`
- ✅ **AutoMapper** — Clean object mapping
- ✅ **Global Exception Middleware** — Structured JSON error responses
- ✅ **Swagger API Documentation** — Interactive API explorer
- ✅ **MVVM Pattern** — Business logic separated from UI in Flutter
- ✅ **Provider State Management** — Reactive UI updates
- ✅ **FCM Device Token Registration** — Auto-registers with backend
- ✅ **Foreground & Background Notifications** — Full push notification lifecycle

---

## 🚀 Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Flutter SDK 3.2+
- SQL Server (LocalDB or Express)
- Firebase project with Cloud Messaging enabled

### 1. Firebase Setup
1. Go to [Firebase Console](https://console.firebase.google.com)
2. Create a new project (or use existing)
3. Enable **Cloud Messaging**
4. Get the **Server Key**: Project Settings → Cloud Messaging → Server Key
5. For Android: Download `google-services.json` → place in `mobile-app/flutter_notification_app/android/app/`
6. For iOS: Download `GoogleService-Info.plist` → place in `mobile-app/flutter_notification_app/ios/Runner/`

### 2. Backend Setup
```bash
cd RemoteNotificationSystem/backend

# Update connection string in NotificationSystem.API/appsettings.json
# Update Firebase ServerKey in NotificationSystem.API/appsettings.json

# Restore packages
dotnet restore

# Run the API
dotnet run --project NotificationSystem.API

# API will be available at http://localhost:5000
# Swagger UI at http://localhost:5000
```

### 3. Angular Admin Panel
```bash
cd RemoteNotificationSystem/admin-panel/angular-notification-admin

# Install dependencies
npm install

# Start development server
ng serve --open

# Admin panel at http://localhost:4200
```

### 4. Flutter Mobile App
```bash
cd RemoteNotificationSystem/mobile-app/flutter_notification_app

# Get dependencies
flutter pub get

# Run on connected device/emulator
flutter run
```

> **Note:** For Android emulator, the API URL uses `10.0.2.2:5000`. For a physical device, update the `baseUrl` in `lib/services/api_service.dart` to your machine's local IP.

---

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/device/register` | Register a mobile device FCM token |
| `POST` | `/api/notification/send` | Send a push notification |
| `GET` | `/api/notification` | Get all notifications |

### Example: Send Notification
```json
POST /api/notification/send
{
  "title": "Hello World",
  "body": "This is a test notification",
  "sentBy": "Admin"
}
```

### Example: Register Device
```json
POST /api/device/register
{
  "deviceToken": "fcm_token_here",
  "deviceName": "Pixel 7"
}
```

---

## 🎬 Demo Scenario

1. **Start the backend** → `dotnet run --project NotificationSystem.API`
2. **Start Angular admin panel** → `ng serve`
3. **Run Flutter app** on device/emulator → `flutter run`
4. Flutter app automatically registers its FCM token with the backend
5. In the Angular admin panel, enter a **Title** and **Body**, click **Send Notification**
6. ✅ Push notification appears on the Flutter app
7. In Flutter, tap **"View All Notifications"** to see the notification list fetched from the API

---

## 📁 Project Structure

```
RemoteNotificationSystem/
├── backend/
│   ├── NotificationSystem.sln
│   ├── NotificationSystem.API/          # Controllers, Middleware, Program.cs
│   ├── NotificationSystem.Business/     # Services, DTOs, AutoMapper
│   ├── NotificationSystem.Data/         # DbContext, Repositories
│   └── NotificationSystem.Core/         # Entities, Interfaces
├── admin-panel/
│   └── angular-notification-admin/      # Angular 19 app
│       └── src/app/
│           ├── components/notification-form/
│           ├── models/notification.model.ts
│           └── services/notification.service.ts
├── mobile-app/
│   └── flutter_notification_app/        # Flutter MVVM app
│       └── lib/
│           ├── models/notification_model.dart
│           ├── services/api_service.dart & firebase_service.dart
│           ├── viewmodels/notification_viewmodel.dart
│           └── views/home_screen.dart & notification_list_screen.dart
└── README.md
```

---

## 📄 License

This project is for educational and demonstration purposes.
