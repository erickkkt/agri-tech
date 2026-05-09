import { Component } from '@angular/core';
import { NavService } from '../side-nav/nav.service';
import { MatDialogConfig } from '@angular/material/dialog';
import { AuthService } from '../../../services/auth.service';
import { UserProfile } from '../../../models/user-profile.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: false,
})

export class HeaderComponent {
  loading: boolean = true;
  screenType: any;
  displaySearch: boolean = true;
  userProfile: UserProfile | undefined;

  public userName: string = 'Authenticating...';
  firstName: string = '';
  lastName: string = '';
  configUserDialog = new MatDialogConfig();

  constructor(
    private authService: AuthService,
    private navService: NavService) {

    this.configUserDialog.position = {
      top: 65 + 'px',
      right: '0px'
    };
    this.configUserDialog.width = '600px';

    this.authService.isAuthenticated$.subscribe(authenticated => {
      if (authenticated) {
        this.authService.userInfo$.subscribe(user => {
          this.userProfile = user !== null ? user : undefined;
          if (this.userProfile && this.userProfile['userName']) {
            this.loading = false;
            this.userName = 'Welcome ' + this.userProfile['userName'];
          }
        });
      }
    });
  }

  public onMenuButtonClick() {
    setTimeout(() => {
      this.navService.NavClick();
    }, 10);
  }
}
