
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';

import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule, MatIconRegistry } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';

import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatRippleModule, MAT_DATE_LOCALE, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';

import { MY_FORMATS } from './shared/constants/constants';

import { AuthGuardService } from './services/auth-guard.service';

import { AppComponent } from './app.component';
import { NavigationComponent } from './shared/components/navigation/navigation.component';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { SideNavComponent } from './shared/components/side-nav/side-nav.component';
import { MenuListItemComponent } from './shared/components/menu-list-item/menu-list-item.component';
import { NavService } from './shared/components/side-nav/nav.service';
import { AppRoutingModule } from './app-routing.module';

import { AnimalManagementComponent } from './components/animal-management/animal-management.component';
import { CageManagementComponent } from './components/cage-management/cage-management.component';
import { FarmManagementComponent } from './components/farm-management/farm-management.component';
import { UserManagementComponent } from './components/user-management/user-management.component';

// Phase 1 components
import { VaccineManagementComponent } from './components/vaccine-management/vaccine-management.component';
import { VaccinePopupComponent } from './components/vaccine-management/vaccine-popup/vaccine-popup.component';
import { VaccineSchedulePopupComponent } from './components/vaccine-management/vaccine-schedule-popup/vaccine-schedule-popup.component';
import { FeedManagementComponent } from './components/feed-management/feed-management.component';
import { FeedItemPopupComponent } from './components/feed-management/feed-item-popup/feed-item-popup.component';
import { FeedTransactionPopupComponent } from './components/feed-management/feed-transaction-popup/feed-transaction-popup.component';
import { GrowthLogComponent } from './components/growth-log/growth-log.component';
import { AlertsComponent } from './components/alerts/alerts.component';
import { ReportsComponent } from './components/reports/reports.component';
import { DiseaseManagementComponent } from './components/disease-management/disease-management.component';
import { DiseasePopupComponent } from './components/disease-management/disease-popup/disease-popup.component';
import { TreatmentPopupComponent } from './components/disease-management/treatment-popup/treatment-popup.component';


import { MaxLengthMessage } from './shared/pipes/max-length-error-message.pipe';
import { SafeHtmlPipe } from './shared/pipes/sanitize-html.pipe';
import { ConfigurationService } from './services/configuration.service';
import { DialogComponent } from './shared/components/dialog/dialog.component';
import { StandardHeaderInterceptor } from './shared/interceptors/standard-header.interceptor';
import { HomeComponent } from './components/home/home.component';
import { FarmPopupComponent } from './components/farm-management/farm-popup/farm-popup.component';
import { CagePopupComponent } from './components/cage-management/cage-popup/cage-popup.component';
import { AnimalPopupComponent } from './components/animal-management/animal-popup/animal-popup.component';

export function storageFactory(): OAuthStorage {
  return sessionStorage;
}

const appInitializerFn = (configurationService: ConfigurationService) => {
  return async () => {
    const clientConfiguration = await configurationService.loadRuntimeConfig();
    if (!clientConfiguration) {
      console.error('Client configuration could not be loaded.');
      return;
    }
    await configurationService.loadConfig(clientConfiguration ? clientConfiguration.apiUrl : '');
  };
};

@NgModule({
  declarations: [
    // Component
    AppComponent,
    NavigationComponent,
    HeaderComponent,
    FooterComponent,
    SideNavComponent,
    DialogComponent,
    MenuListItemComponent,
    AnimalManagementComponent,
    CageManagementComponent,
    FarmManagementComponent,
    UserManagementComponent,

    UserManagementComponent,

    //Pipe
    // MaxLengthMessage,
    // SafeHtmlPipe,
    HomeComponent,
    FarmPopupComponent,
    CagePopupComponent,
    AnimalPopupComponent,

    // Phase 1
    VaccineManagementComponent,
    VaccinePopupComponent,
    VaccineSchedulePopupComponent,
    FeedManagementComponent,
    FeedItemPopupComponent,
    FeedTransactionPopupComponent,
    GrowthLogComponent,
    AlertsComponent,
    ReportsComponent,
    DiseaseManagementComponent,
    DiseasePopupComponent,
    TreatmentPopupComponent,
  ],
  bootstrap: [AppComponent], imports: [BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: [],
        sendAccessToken: true
      }
    }),
    AppRoutingModule,
    MatToolbarModule,
    MatIconModule,
    MatListModule,
    MatGridListModule,
    MatButtonToggleModule,
    MatButtonModule,
    MatTooltipModule,
    MatStepperModule,
    MatInputModule,
    MatFormFieldModule,
    MatRadioModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatSlideToggleModule,
    MatCardModule,
    MatDividerModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatTableModule,
    MatRippleModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatDialogModule,
    MatTabsModule,
    MatSidenavModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatAutocompleteModule,
    MatExpansionModule,
  ],
  providers: [
    ConfigurationService,
    MaxLengthMessage,
    SafeHtmlPipe,
    { provide: APP_INITIALIZER, useFactory: appInitializerFn, multi: true, deps: [ConfigurationService] },
    AuthGuardService,
    NavService,
    { provide: MAT_DATE_LOCALE, useValue: 'en-US' },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: StandardHeaderInterceptor,
      multi: true,
    },
    { provide: OAuthStorage, useFactory: storageFactory },
    {
      provide: MatDialogRef,
      useValue: {}
    },
    CurrencyPipe,
    provideHttpClient(withInterceptorsFromDi())
  ]
})

export class AppModule {
  constructor(private readonly matIconRegistry: MatIconRegistry, domSanitizer: DomSanitizer) {
    this.matIconRegistry.addSvgIconSet(domSanitizer.bypassSecurityTrustResourceUrl('../../assets/img/icons/mdi.svg'));
  }
}
