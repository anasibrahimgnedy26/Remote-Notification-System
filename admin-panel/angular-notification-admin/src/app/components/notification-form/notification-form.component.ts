import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { NotificationRequest, NotificationResponse } from '../../models/notification.model';

@Component({
  selector: 'app-notification-form',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './notification-form.component.html',
  styleUrl: './notification-form.component.css'
})
export class NotificationFormComponent {
  title = '';
  body = '';
  targetType = 'All';
  targetValue = '';
  isLoading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');
  notifications = signal<NotificationResponse[]>([]);

  constructor(private notificationService: NotificationService) {
    this.loadNotifications();
  }

  sendNotification(): void {
    if (!this.title.trim() || !this.body.trim()) {
      this.errorMessage.set('Title and Body are required.');
      return;
    }

    this.isLoading.set(true);
    this.successMessage.set('');
    this.errorMessage.set('');

    const request: NotificationRequest = {
      title: this.title,
      body: this.body,
      sentBy: 'Admin Panel',
      targetType: this.targetType,
      targetValue: this.targetType === 'All' ? undefined : this.targetValue
    };

    this.notificationService.sendNotification(request).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        if (response.success) {
          this.successMessage.set('✅ Notification sent successfully!');
          this.title = '';
          this.body = '';
          this.targetValue = '';
          this.loadNotifications();
        } else {
          this.errorMessage.set(response.message || 'Failed to send notification.');
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set(error.message || 'Failed to send notification.');
      }
    });
  }

  loadNotifications(): void {
    this.notificationService.getAllNotifications().subscribe({
      next: (response) => {
        if (response.success) {
          this.notifications.set(response.data);
        }
      },
      error: (error) => {
        console.error('Failed to load notifications:', error);
      }
    });
  }

  dismissSuccess(): void {
    this.successMessage.set('');
  }

  dismissError(): void {
    this.errorMessage.set('');
  }
}
