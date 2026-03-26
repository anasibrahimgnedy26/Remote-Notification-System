import { Component } from '@angular/core';
import { NotificationFormComponent } from './components/notification-form/notification-form.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NotificationFormComponent],
  template: '<app-notification-form></app-notification-form>',
  styles: [`
    :host {
      display: block;
      margin: 0;
      padding: 0;
    }
  `]
})
export class App {}
