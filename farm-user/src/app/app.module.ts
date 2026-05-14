import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { MarketplaceDetailComponent } from './marketplace/marketplace-detail.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { InvestmentCommitmentComponent } from './investment/commitment/investment-commitment.component';
import { ForumListComponent } from './forum/forum-list.component';
import { LoginComponent } from './auth/login.component';

import { ConfigurationService } from './services/configuration.service';
import { AuthGuardService } from './services/auth-guard.service';
import { StandardHeaderInterceptor } from './shared/interceptors/standard-header.interceptor';
import { AuthInterceptor } from './shared/interceptors/auth.interceptor';

/**
 * APP_INITIALIZER factory — runs before the app bootstraps so /assets/config.json is
 * loaded by the time any service is constructed.
 */
const appInitializerFn = (configurationService: ConfigurationService) => {
  return async () => {
    const clientConfig = await configurationService.loadRuntimeConfig();
    if (!clientConfig) {
      console.error('Client configuration could not be loaded.');
    }
  };
};

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MarketplaceListComponent,
    MarketplaceDetailComponent,
    InvestmentListComponent,
    InvestmentCommitmentComponent,
    ForumListComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [
    ConfigurationService,
    AuthGuardService,
    { provide: APP_INITIALIZER, useFactory: appInitializerFn, multi: true, deps: [ConfigurationService] },
    // Order matters: AuthInterceptor attaches Bearer first, then StandardHeader handles
    // content-type and 401 redirect.
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: StandardHeaderInterceptor, multi: true },
    provideHttpClient(withInterceptorsFromDi())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
