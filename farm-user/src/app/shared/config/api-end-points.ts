import { Injectable } from '@angular/core';
import { ConfigurationService } from '../../services/configuration.service';
import { configuration } from '../../../environments/environment';

/**
 * Centralised endpoint URL builder. Mirrors farm-admin/ApiEndPoints but scoped
 * to the end-user surface area: marketplace listings, investment offers/orders,
 * forum, and user info.
 */
@Injectable({ providedIn: 'root' })
export class ApiEndPoints {

  constructor(private readonly configurationService: ConfigurationService) { }

  private get base(): string {
    return `${this.configurationService.apiBaseUrl}/api/${configuration.version}`;
  }

  /*********** USER ***********/
  getUserInfo() { return `${this.base}/users/info`; }
  signOut()     { return `${this.base}/users/sign-out`; }

  /*********** MARKETPLACE — LISTINGS ***********/
  searchListings() { return `${this.base}/listings`; }
  getListingById(id: string) { return `${this.base}/listings/${id}`; }

  /*********** INVESTMENT ***********/
  getInvestmentOffers() { return `${this.base}/investment/offers`; }
  getInvestmentOffer(id: string) { return `${this.base}/investment/offers/${id}`; }
  placeInvestmentOrder() { return `${this.base}/investment/orders`; }
  getMyInvestmentOrders() { return `${this.base}/investment/orders/me`; }
  getAnimalUpdates(animalId: string) { return `${this.base}/investment/animal-updates/${animalId}`; }

  /*********** FORUM ***********/
  getForumThreads() { return `${this.base}/forum/threads`; }
  getForumThread(id: string) { return `${this.base}/forum/threads/${id}`; }
  createForumThread() { return `${this.base}/forum/threads`; }
  getForumPosts(threadId: string) { return `${this.base}/forum/threads/${threadId}/posts`; }
  createForumPost() { return `${this.base}/forum/posts`; }
}
