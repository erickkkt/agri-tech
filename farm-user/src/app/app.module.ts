import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { ForumListComponent } from './forum/forum-list.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MarketplaceListComponent,
    InvestmentListComponent,
    ForumListComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [
    provideHttpClient(withInterceptorsFromDi())
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
