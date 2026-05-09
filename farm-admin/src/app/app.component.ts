import { Component, OnInit } from '@angular/core';

import { AuthService } from './services/auth.service';
import { RealtimeService } from './services/realtime.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {

  title = 'Farm Management Admin';

  constructor(
    private authService: AuthService,
    private realtimeService: RealtimeService
  ) {

    this.authService.runInitialLoginSequence();
  }

  ngOnInit(): void {
    // Connect SignalR once the user is authenticated. Reconnect on token refresh.
    this.authService.isAuthenticated$.subscribe(async authenticated => {
      if (authenticated) {
        await this.realtimeService.connect(() => this.authService.accessToken);
      } else {
        await this.realtimeService.disconnect();
      }
    });
  }
}
