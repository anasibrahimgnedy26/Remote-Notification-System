import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../models/notification_model.dart';

class ApiService {
  // ⚠️ IMPORTANT: Using localtunnel to expose backend to the internet.
  // Original LAN URL: http://192.168.137.1:5000/api
  static const String baseUrl = 'https://few-berries-cross.loca.lt/api';

  /// Register device token with the backend
  Future<bool> registerDevice(String deviceToken, String deviceName) async {
    try {
      final url = '$baseUrl/device/register';
      debugPrint('=== [ApiService] POST $url');
      debugPrint('=== [ApiService] Token: ${deviceToken.substring(0, 20)}..., Name: $deviceName');
      
      final response = await http.post(
        Uri.parse(url),
        headers: {
          'Content-Type': 'application/json',
          'bypass-tunnel-reminder': 'true', // Bypass localtunnel warning page
        },
        body: jsonEncode({
          'deviceToken': deviceToken,
          'deviceName': deviceName,
        }),
      );

      debugPrint('=== [ApiService] Response status: ${response.statusCode}');
      debugPrint('=== [ApiService] Response body: ${response.body}');

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        return data['success'] == true;
      }
      
      return false;
    } catch (e, stackTrace) {
      debugPrint('=== [ApiService] ERROR registering device: $e');
      debugPrint('=== [ApiService] Stack: $stackTrace');
      return false;
    }
  }

  /// Get all notifications from the backend
  Future<List<NotificationModel>> getAllNotifications() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/notification'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        if (data['success'] == true && data['data'] != null) {
          return (data['data'] as List)
              .map((json) => NotificationModel.fromJson(json))
              .toList();
        }
      }
      return [];
    } catch (e) {
      debugPrint('Error fetching notifications: $e');
      return [];
    }
  }
}
