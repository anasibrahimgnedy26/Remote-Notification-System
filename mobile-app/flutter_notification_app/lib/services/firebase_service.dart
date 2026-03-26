import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'api_service.dart';

class FirebaseService {
  // Singleton pattern
  static final FirebaseService _instance = FirebaseService._internal();
  factory FirebaseService() => _instance;
  FirebaseService._internal();

  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotifications =
      FlutterLocalNotificationsPlugin();
  final ApiService _apiService = ApiService();

  /// Initialize Firebase Messaging and Local Notifications
  Future<void> initialize() async {
    // Request Firebase Messaging permissions
    await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
      provisional: false,
    );

    // Set foreground notification options for iOS
    await _firebaseMessaging.setForegroundNotificationPresentationOptions(
      alert: true,
      badge: true,
      sound: true,
    );

    // Initialize local notifications for foreground display
    const androidSettings =
        AndroidInitializationSettings('@mipmap/ic_launcher');
    const iosSettings = DarwinInitializationSettings(
      requestAlertPermission: true,
      requestBadgePermission: true,
      requestSoundPermission: true,
    );
    const initSettings = InitializationSettings(
      android: androidSettings,
      iOS: iosSettings,
    );

    await _localNotifications.initialize(
      initSettings,
      onDidReceiveNotificationResponse: (details) {
        // Handle tapping on local notifications
        debugPrint('Local notification tapped: ${details.payload}');
      },
    );

    // Request Android 13+ Notification Permission via flutter_local_notifications
    if (defaultTargetPlatform == TargetPlatform.android) {
      await _localNotifications
          .resolvePlatformSpecificImplementation<
              AndroidFlutterLocalNotificationsPlugin>()
          ?.requestNotificationsPermission();
    }

    // Create notification channel for Android
    const androidChannel = AndroidNotificationChannel(
      'high_importance_channel',
      'High Importance Notifications',
      description: 'This channel is used for important notifications.',
      importance: Importance.max,
      playSound: true,
      enableVibration: true,
    );

    await _localNotifications
        .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>()
        ?.createNotificationChannel(androidChannel);

    // Handle foreground messages
    FirebaseMessaging.onMessage.listen(_handleForegroundMessage);

    // Handle background message taps
    FirebaseMessaging.onMessageOpenedApp.listen(_handleMessageOpenedApp);

    // Get and register FCM token
    await registerToken();

    // Listen for token refresh
    _firebaseMessaging.onTokenRefresh.listen((newToken) async {
      await _registerTokenWithBackend(newToken);
    });
  }

  /// Get FCM token and register with backend (public - can be called on refresh)
  Future<void> registerToken() async {
    try {
      debugPrint('=== [registerToken] Attempting to get FCM token...');
      final token = await _firebaseMessaging.getToken();
      if (token != null) {
        debugPrint('=== [registerToken] FCM Token obtained: ${token.substring(0, 20)}...');
        await _registerTokenWithBackend(token);
      } else {
        debugPrint('=== [registerToken] WARNING: FCM token is NULL! Firebase may not be configured correctly.');
      }
    } catch (e, stackTrace) {
      debugPrint('=== [registerToken] ERROR getting FCM token: $e');
      debugPrint('=== [registerToken] Stack: $stackTrace');
    }
  }

  /// Send token to backend API
  Future<void> _registerTokenWithBackend(String token) async {
    debugPrint('=== [_registerTokenWithBackend] Sending token to backend...');
    final success = await _apiService.registerDevice(token, 'Flutter App');
    if (success) {
      debugPrint('=== [_registerTokenWithBackend] SUCCESS - Device registered!');
    } else {
      debugPrint('=== [_registerTokenWithBackend] FAILED - Registration failed!');
    }
  }

  /// Handle foreground messages - show as local notification
  void _handleForegroundMessage(RemoteMessage message) {
    debugPrint('Foreground message received: ${message.notification?.title}');

    final notification = message.notification;
    if (notification != null) {
      _localNotifications.show(
        notification.hashCode,
        notification.title,
        notification.body,
        const NotificationDetails(
          android: AndroidNotificationDetails(
            'high_importance_channel',
            'High Importance Notifications',
            channelDescription:
                'This channel is used for important notifications.',
            importance: Importance.max,
            priority: Priority.high,
            icon: '@mipmap/ic_launcher',
          ),
          iOS: DarwinNotificationDetails(
            presentAlert: true,
            presentBadge: true,
            presentSound: true,
          ),
        ),
      );
    }
  }

  /// Handle when user taps on a notification
  void _handleMessageOpenedApp(RemoteMessage message) {
    debugPrint('Notification tapped: ${message.notification?.title}');
    // Can navigate to specific screen here
  }

  /// Subscribe to a topic for push notifications
  Future<bool> subscribeToTopic(String topic) async {
    try {
      await _firebaseMessaging.subscribeToTopic(topic);
      debugPrint('Subscribed to topic: $topic');
      return true;
    } catch (e) {
      debugPrint('Error subscribing to topic: $e');
      return false;
    }
  }
}
