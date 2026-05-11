import { Component, OnInit } from '@angular/core';
import { InvestmentService } from '../services/investment.service';
import { InvestmentOffer } from '../models/investment.model';

/**
 * Investor portal - browse open investment offers.
 * On click of "Đầu tư", places an order via /api/v1/investment/orders.
 */
@Component({
  selector: 'app-investment-list',
  templateUrl: './investment-list.component.html',
  standalone: false
})
export class InvestmentListComponent implements OnInit {
  offers: InvestmentOffer[] = [];

  constructor(private investmentService: InvestmentService) { }

  async ngOnInit() {
    this.offers = (await this.investmentService.getOpenOffers()) ?? [];
  }

  async invest(offer: InvestmentOffer) {
    const qtyStr = window.prompt(`Bạn muốn mua bao nhiêu share? (còn ${offer.availableShares})`);
    if (!qtyStr) return;
    const qty = parseInt(qtyStr, 10);
    if (!qty || qty <= 0 || qty > offer.availableShares) {
      window.alert('Số share không hợp lệ');
      return;
    }
    try {
      await this.investmentService.placeOrder(offer.id, qty);
      window.alert('Đặt mua thành công!');
      this.offers = (await this.investmentService.getOpenOffers()) ?? [];
    } catch (e: any) {
      window.alert('Lỗi: ' + (e?.error?.error ?? e?.message ?? 'unknown'));
    }
  }
}
