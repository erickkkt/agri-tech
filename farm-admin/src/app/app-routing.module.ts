import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthService } from './services/auth.service';
import { AuthGuardService } from './services/auth-guard.service';
import { HomeComponent } from './components/home/home.component';
import { ErrorHandlerComponent } from './shared/error-handler/error-handler.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { AccessDeniedComponent } from './shared/components/access-denied/access-denied.component';
import { AnimalManagementComponent } from './components/animal-management/animal-management.component';
import { AnimalDetailComponent } from './components/animal-management/animal-detail/animal-detail.component';
import { FarmManagementComponent } from './components/farm-management/farm-management.component';
import { CageManagementComponent } from './components/cage-management/cage-management.component';

// Phase 1 components
import { VaccineManagementComponent } from './components/vaccine-management/vaccine-management.component';
import { FeedManagementComponent } from './components/feed-management/feed-management.component';
import { GrowthLogComponent } from './components/growth-log/growth-log.component';
import { AlertsComponent } from './components/alerts/alerts.component';
import { ReportsComponent } from './components/reports/reports.component';
import { DiseaseManagementComponent } from './components/disease-management/disease-management.component';

import { Roles } from './models/enum/role.enum';

const routes: Routes = [
  { path: '', redirectTo: '/app', pathMatch: 'full' },
  {
    path: 'app',
    data: { roles: [Roles.HOSYSADMIN] },
    children: [
      {
        path: '', component: HomeComponent,
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        pathMatch: 'full'
      },
      {
        path: 'animals',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: AnimalManagementComponent
      },
      {
        path: 'animals/:id',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: AnimalDetailComponent
      },
      {
        path: 'farms',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: FarmManagementComponent
      },
      {
        path: 'cages',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: CageManagementComponent
      }
      ,
      {
        path: 'users',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: UserManagementComponent
      },

      // ===== Phase 1 =====
      {
        path: 'vaccines',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: VaccineManagementComponent
      },
      {
        path: 'feeds',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: FeedManagementComponent
      },
      {
        path: 'growth-logs',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: GrowthLogComponent
      },
      {
        path: 'alerts',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: AlertsComponent
      },
      {
        path: 'reports',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: ReportsComponent
      },
      {
        path: 'diseases',
        data: { roles: [Roles.HOSYSADMIN] },
        canLoad: [AuthGuardService],
        canActivate: [AuthGuardService],
        canActivateChild: [AuthGuardService],
        component: DiseaseManagementComponent
      }
    ]
  },
  { path: 'error', component: ErrorHandlerComponent, data: { roles: [Roles.HOSYSADMIN] } },
  {
    path: 'access-denied', component: AccessDeniedComponent
  },
  //ALLWAYS AT THE LAST ONE
  {
    path: '**', // Wildcard route for all other invalid paths
    redirectTo: '/app',
    pathMatch: 'full'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [
    AuthService,
    AuthGuardService
  ]
})
export class AppRoutingModule { }
