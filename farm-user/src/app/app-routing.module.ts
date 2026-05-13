import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { InvestmentCommitmentComponent } from './investment/commitment/investment-commitment.component';
import { ForumListComponent } from './forum/forum-list.component';
import { AuthGuardService } from './services/auth-guard.service';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'marketplace', component: MarketplaceListComponent },
  { path: 'investments', component: InvestmentListComponent },
  {
    // Requires login — the commitment page shows bank account details + contract
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
