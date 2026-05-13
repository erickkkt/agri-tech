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

  /**
   * "1 vật nuôi = 1 cơ hội đầu tư" — investor commits the FULL offer amount on
   * a specific animal. The backend share mechanism is hidden behind a single
   * "investment amount" displayed to end-users.
   */
  totalAmount(o: InvestmentOffer): number {
    return o.totalShares * o.pricePerShare;
  }

  /** True if the offer hasn't been picked up yet (no shares sold). */
  isAvailable(o: InvestmentOffer): boolean {
    return o.availableShares > 0;
  }

  openInvestModal(offer: InvestmentOffer) {
    if (!this.authService.hasValidToken()) {
      this.authService.login();
      return;
    }
    this.selectedOffer = offer;
  }

  closeModal() { this.selectedOffer = null; }

  async confirmInvest() {
    if (!this.selectedOffer) return;
    const offer = this.selectedOffer;

    this.placingOrder = true;
    try {
      // Single-animal investment: buy ALL available shares so the entire offer
      // is committed to this one investor.
      await this.investmentService.placeOrder(offer.id, offer.availableShares);
      this.closeModal();
      await this.loadOffers();
      window.alert('🎉 Đặt đầu tư thành công! Cảm ơn bạn đã tham gia.');
    } catch (e: any) {
      window.alert('Lỗi: ' + (e?.error?.error ?? e?.message ?? 'Vui lòng thử lại'));
    } finally {
      this.placingOrder = false;
    }
  }
}
