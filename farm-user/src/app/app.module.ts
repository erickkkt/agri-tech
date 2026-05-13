import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { InvestmentCommitmentComponent } from './investment/commitment/investment-commitment.component';
import { ForumListComponent } from './forum/forum-list.component';

import { ConfigurationService } from './services/configuration.service';
import { AuthGuardService } from './services/auth-guard.service';
import { StandardHeaderInterceptor } from './shared/interceptors/standard-header.interceptor';

/** Tokens persist in sessionStorage so they survive tab reloads but not browser restart. */
export function storageFactory(): OAuthStorage {
  return sessionStorage;
}

/**
 * APP_INITIALIZER factory — runs before the app bootstraps so /assets/config.json is
 * loaded and the OAuth allow-list is populated by the time any service is constructed.
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
    InvestmentListComponent,
    InvestmentCommitmentComponent,
    ForumListComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: [],
        sendAccessToken: true
      }
    })
  ],
  providers: [
    ConfigurationService,
    AuthGuardService,
    { provide: APP_INITIALIZER, useFactory: appInitializerFn, multi: true, deps: [ConfigurationService] },
    { provide: HTTP_INTERCEPTORS, useClass: StandardHeaderInterceptor, multi: true },
    { provide: OAuthStorage, useFactory: storageFactory },
    provideHttpClient(withInterceptorsFromDi())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
