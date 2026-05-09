import { Component, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';
import { NavService } from './nav.service';
import { NavItem } from './nav-item.model';
import { HttpBaseService } from '../../services/http-base.service';
import { Observable } from 'rxjs';
import { ApiEndPoints } from '../../config/api-end-points';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'app-side-nav',
    templateUrl: './side-nav.component.html',
    styleUrls: ['./side-nav.component.scss'],
    standalone: false,
})

export class SideNavComponent implements OnInit, AfterViewInit {
    ngOnInit(): void {
        this.authService.isAuthenticated$.subscribe(authenticated => {
            if (authenticated) {
                this.authService.userInfo$.subscribe(user => {
                    if (user && user['userName']) {
                        this.roles = [user['role']];
                    }
                });

                this.navItems$ = this.httpService.getData<NavItem[]>(this.api.getAuthorizedNavItems());
            }
        });


    }
    roles: string[] = [];
    navItems$: Observable<NavItem[]> | undefined;
    constructor(private navService: NavService, private authService: AuthService, private httpService: HttpBaseService,
        private api: ApiEndPoints) {

    }
    public get userRoles() {
        return this.roles;
    }

    @ViewChild('appDrawer') appDrawer: ElementRef | undefined;

    ngAfterViewInit(): void {
        this.navService.appDrawer = this.appDrawer;
    }
}
