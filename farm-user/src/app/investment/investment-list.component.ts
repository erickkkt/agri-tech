import { Component, OnInit } from '@angular/core';
import { InvestmentService } from '../services/investment.service';
import { InvestmentOffer } from '../models/investment.model';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-investment-list',
  templateUrl: './investment-list.component.html',
  styleUrls: ['./investment-list.component.css'],
  standalone: false
})
export class InvestmentListComponent implements OnInit {
  offers: InvestmentOffer[] = [];
  loading = false;
  selectedOffer: InvestmentOffer | null = null;
  shareQty = 1;
  placingOrder = false;

  constructor(
    private readonly investmentService: InvestmentService,
    private readonly authService: AuthService
  ) {}

  async ngOnInit() {
    await this.loadOffers();
  }

  async loadOffers() {
    this.loading = true;
    try {
      this.offers = (await this.investmentService.getOpenOffers()) ?? [];
    } finally {
      this.loading = false;
    }
  }

  openInvestModal(offer: InvestmentOffer) {
    if (!this.authService.hasValidToken()) {
      this.authService.login();
      return;
    }
    this.selectedOffer = offer;
    this.shareQty = 1;
  }

  closeModal() {
    this.selectedOffer = null;
  }

  async confirmInvest() {
    if (!this.selectedOffer) return;
    const offer = this.selectedOffer;
    if (this.shareQty <= 0 || this.shareQty > offer.availableShares) return;

    this.placingOrder = true;
    try {
      await this.investmentService.placeOrder(offer.id, this.shareQty);
      this.closeModal();
      await this.loadOffers();
      // Lightweight success toast via alert; can swap to a real toast later
      window.alert('🎉 Đặt mua thành công! Cảm ơn bạn đã tham gia.');
    } catch (e: any) {
      window.alert('Lỗi: ' + (e?.error?.error ?? e?.message ?? 'Vui lòng thử lại'));
    } finally {
      this.placingOrder = false;
    }
  }

  progressPct(o: InvestmentOffer): number {
    if (!o.totalShares) return 0;
    return Math.round(((o.totalShares - o.availableShares) / o.totalShares) * 100);
  }

  get totalCost(): number {
    if (!this.selectedOffer) return 0;
    return this.shareQty * this.selectedOffer.pricePerShare;
  }
}
