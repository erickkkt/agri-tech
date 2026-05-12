import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { UserInfo } from 'angular-oauth2-oidc';

import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent implements OnInit {
  title = 'Agri-Tech Farm User';

  isAuthenticated$: Observable<boolean>;
  userInfo$: Observable<UserInfo | null>;

  constructor(private readonly authService: AuthService) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.userInfo$ = this.authService.userInfo$;
  }

  async ngOnInit(): Promise<void> {
    await this.authService.runInitialLoginSequence();
  }

  login(): void {
    this.authService.login();
  }

  logout(): void {
    this.authService.logout();
  }
}
