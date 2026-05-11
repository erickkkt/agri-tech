import { Component, OnInit } from '@angular/core';
import { ListingService } from '../services/listing.service';
import { Listing, ListingSearchQuery } from '../models/listing.model';

/**
 * Marketplace browse + filter view for buyers and investors.
 */
@Component({
  selector: 'app-marketplace-list',
  templateUrl: './marketplace-list.component.html',
  standalone: false
})
export class MarketplaceListComponent implements OnInit {
  listings: Listing[] = [];
  total = 0;
  query: ListingSearchQuery = { pageIndex: 0, pageSize: 12 };

  constructor(private listingService: ListingService) { }

  async ngOnInit() {
    await this.search();
  }

  async search() {
    const result = await this.listingService.search(this.query);
    if (result) {
      this.listings = result.items;
      this.total = result.total;
    }
  }

  resetAndSearch() {
    this.query.pageIndex = 0;
    this.search();
  }
}
