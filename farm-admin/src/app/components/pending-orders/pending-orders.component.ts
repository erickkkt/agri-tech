import { Component, OnInit } from '@angular/core';
import { HttpBaseService } from '../../shared/services/http-base.service';
import { ApiEndPoints } from '../../shared/config/api-end-points';
import { DialogService } from '../../shared/services/dialog.service';

interface PendingOrder {
  orderId: string;
  createdAt: string;
  transferReference: string;
  totalAmount: number;
  bankTransferConfirmedAt?: string;
  investorUserId: string;
  investorUserName: string;
  farmId: string;
  farmName: string;
  farmBankName?: string;
  farmBankAccountNumber?: string;
  animalId: string;
  animalCode: string;
  animalName: string;
  offerTitle: string;
}

/**
 * Admin page listing investment orders awaiting bank-transfer confirmation.
 * Admin checks their bank app, matches the AGRI-XXXXX reference, then clicks
 * "Xác nhận đã nhận tiền" to issue the share certificate.
 */
@Component({
  selector: 'app-pending-orders',
  templateUrl: './pending-orders.component.html',
  styleUrls: ['./pending-orders.component.css'],
  standalone: false
})
export class PendingOrdersComponent implements OnInit {
  orders: PendingOrder[] = [];
  loading = false;
  confirmingId: string | null = null;

  constructor(
    private readonly http: HttpBaseService,
    private readonly api: ApiEndPoints,
    private readonly dialogService: DialogService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.load();
  }

  async load(): Promise<void> {
    this.loading = true;
    try {
      this.orders = (await this.http.getDataAsync<PendingOrder[]>(this.api.listPendingOrders())) ?? [];
    } finally {
      this.loading = false;
    }
  }

  async confirm(o: PendingOrder): Promise<void> {
    this.dialogService.openConfirmationDialog({
      title: 'Xác nhận đã nhận chuyển khoản',
      message: `Bạn đã kiểm tra app ngân hàng và xác nhận đã nhận ${this.fmt(o.totalAmount)} VND với mã ${o.transferReference}?`
    }).subscribe(async (ok: any) => {
      if (!ok) return;
      this.confirmingId = o.orderId;
      try {
        await this.http.postDataAsync(this.api.confirmBankTransfer(o.orderId), {});
        this.orders = this.orders.filter(x => x.orderId !== o.orderId);
      } catch (e: any) {
        this.dialogService.openErrorDialog({ title: 'Lỗi', message: e?.error?.error ?? e?.message ?? 'Không xác nhận được' });
      } finally {
        this.confirmingId = null;
      }
    });
  }

  fmt(n: number): string {
    return new Intl.NumberFormat('vi-VN').format(n);
  }
}
