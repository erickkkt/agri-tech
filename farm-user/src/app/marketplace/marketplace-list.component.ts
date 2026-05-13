import { Component, OnInit } from '@angular/core';
import { ListingService } from '../services/listing.service';
import { Listing, ListingCategory, ListingSearchQuery } from '../models/listing.model';

const SPECIES_LABEL: Record<number, string> = {
  0: 'Hươu',
  1: 'Cừu',
  2: 'Bò',
  3: 'Đà điểu'
};

const SPECIES_EMOJI: Record<number, string> = {
  0: '🦌', 1: '🐑', 2: '🐄', 3: '🐓'
};

const CATEGORY_LABEL: Record<number, string> = {
  0: 'Giống', 1: 'Nhung', 2: 'Thịt', 3: 'Trứng', 99: 'Khác'
};

@Component({
  selector: 'app-marketplace-list',
  templateUrl: './marketplace-list.component.html',
  styleUrls: ['./marketplace-list.component.css'],
  standalone: false
})
export class MarketplaceListComponent implements OnInit {
  listings: Listing[] = [];
  total = 0;
  loading = false;
  query: ListingSearchQuery = { pageIndex: 0, pageSize: 12 };

  // Expose constants to template
  readonly speciesLabel = SPECIES_LABEL;
  readonly speciesEmoji = SPECIES_EMOJI;
  readonly categoryLabel = CATEGORY_LABEL;
  readonly ListingCategory = ListingCategory;

  constructor(private listingService: ListingService) { }

  async ngOnInit() {
    await this.search();
  }

  async search() {
    this.loading = true;
    try {
      const result = await this.listingService.search(this.query);
      if (result) {
        this.listings = result.items;
        this.total = result.total;
      }
    } finally {
      this.loading = false;
    }
  }

  resetAndSearch() {
    this.query.pageIndex = 0;
    this.search();
  }

  clearFilters() {
    this.query = { pageIndex: 0, pageSize: 12 };
    this.search();
  }

  speciesOf(l: Listing): string {
    return SPECIES_LABEL[l.species] ?? 'Khác';
  }

  emojiOf(l: Listing): string {
    return SPECIES_EMOJI[l.species] ?? '🐾';
  }
}
