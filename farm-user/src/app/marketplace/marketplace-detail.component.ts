import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Listing, ListingCategory } from '../models/listing.model';
import { ListingService } from '../services/listing.service';

const SPECIES_LABEL: Record<number, string> = {
  0: 'Hươu', 1: 'Cừu', 2: 'Bò', 3: 'Đà điểu'
};
const SPECIES_EMOJI: Record<number, string> = {
  0: '🦌', 1: '🐑', 2: '🐄', 3: '🐓'
};
const CATEGORY_LABEL: Record<number, string> = {
  0: 'Vật nuôi giống', 1: 'Nhung', 2: 'Thịt', 3: 'Trứng', 99: 'Khác'
};

/**
 * Marketplace listing detail page. Guarded by AuthGuardService — unauthenticated
 * users are bounced to /login?returnUrl=/marketplace/<id> and brought back here
 * after signing in.
 */
@Component({
  selector: 'app-marketplace-detail',
  templateUrl: './marketplace-detail.component.html',
  styleUrls: ['./marketplace-detail.component.css'],
  standalone: false
})
export class MarketplaceDetailComponent implements OnInit {

  listing: Listing | null = null;
  loading = false;
  notFound = false;
  activePhoto = 0;

  readonly speciesLabel = SPECIES_LABEL;
  readonly speciesEmoji = SPECIES_EMOJI;
  readonly categoryLabel = CATEGORY_LABEL;
  readonly ListingCategory = ListingCategory;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly listingService: ListingService
  ) { }

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.notFound = true; return; }
    this.loading = true;
    try {
      const l = await this.listingService.getById(id);
      this.listing = l ?? null;
      if (!this.listing) this.notFound = true;
    } catch {
      this.notFound = true;
    } finally {
      this.loading = false;
    }
  }

  speciesOf(): string {
    return this.listing ? SPECIES_LABEL[this.listing.species] ?? 'Khác' : '';
  }
  emojiOf(): string {
    return this.listing ? SPECIES_EMOJI[this.listing.species] ?? '🐾' : '';
  }

  backToList(): void {
    this.router.navigateByUrl('/marketplace');
  }

  contactSeller(): void {
    // Placeholder for the contact / order flow.
    alert('Tính năng liên hệ người bán sẽ ra mắt sớm.');
  }
}
