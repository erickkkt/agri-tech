import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MarketplaceListComponent } from './marketplace/marketplace-list.component';
import { InvestmentListComponent } from './investment/investment-list.component';
import { ForumListComponent } from './forum/forum-list.component';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'marketplace', component: MarketplaceListComponent },
  { path: 'investments', component: InvestmentListComponent },
  { path: 'forum', component: ForumListComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
