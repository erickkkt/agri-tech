import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { MarketplaceDetailComponent } from './marketplace/marketplace-detail.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { InvestmentCommitmentComponent } from './investment/commitment/investment-commitment.component';
import { ForumListComponent } from './forum/forum-list.component';
import { LoginComponent } from './auth/login.component';
import { AuthGuardService } from './services/auth-guard.service';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },

  { path: 'marketplace', component: MarketplaceListComponent },
  // Detail view requires login — guard redirects to /login?returnUrl=...
  {
    path: 'marketplace/:id',
    component: MarketplaceDetailComponent,
    canActivate: [AuthGuardService]
  },

  { path: 'investments', component: InvestmentListComponent },
  {
    // Commitment page shows bank account details + contract — login required.
    path: 'investments/orders/:id/commitment',
    component: InvestmentCommitmentComponent,
    canActivate: [AuthGuardService]
  },

  { path: 'forum', component: ForumListComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
