export interface NotificationRequest {
  title: string;
  body: string;
  sentBy: string;
  targetType: string;
  targetValue?: string;
}

export interface NotificationResponse {
  id: number;
  title: string;
  body: string;
  createdAt: string;
  sentBy: string;
  isSent: boolean;
  targetType: string;
  targetValue?: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}
