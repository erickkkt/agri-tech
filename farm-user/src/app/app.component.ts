import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { AuthService, AuthUser } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent {
  title = 'Agri-Tech Farm User';

  isAuthenticated$: Observable<boolean>;
  currentUser$: Observable<AuthUser | null>;

  constructor(private readonly authService: AuthService) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.currentUser$ = this.authService.currentUser$;
  }

  login(): void {
    this.authService.redirectToLogin();
  }

  logout(): void {
    this.authService.logout('/');
  }
}
