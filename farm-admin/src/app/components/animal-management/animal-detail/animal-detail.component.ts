import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Animal } from '../../../models/animal.model';
import {
  AnimalInvestmentHistoryItem,
  AnimalSaleHistoryItem,
  AnimalService
} from '../../../services/animal.service';
import { ConfigurationService } from '../../../services/configuration.service';
import { SellAnimalDialogComponent } from './sell-animal-dialog/sell-animal-dialog.component';
import { OpenInvestmentDialogComponent } from './open-investment-dialog/open-investment-dialog.component';

const LISTING_STATUS = ['Bản nháp', 'Đang bán', 'Đã bán', 'Đã đóng'];
const OFFER_STATUS = ['Bản nháp', 'Đang mở', 'Đã đóng', 'Đã khai thác'];

@Component({
  selector: 'app-animal-detail',
  templateUrl: './animal-detail.component.html',
  styleUrls: ['./animal-detail.component.css'],
  standalone: false
})
export class AnimalDetailComponent implements OnInit {
  animal: Animal | null = null;
  loading = true;
  notFound = false;

  salesHistory: AnimalSaleHistoryItem[] = [];
  investmentHistory: AnimalInvestmentHistoryItem[] = [];

  readonly LISTING_STATUS = LISTING_STATUS;
  readonly OFFER_STATUS = OFFER_STATUS;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly animalService: AnimalService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
    private readonly configService: ConfigurationService
  ) {}

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.notFound = true;
      this.loading = false;
      return;
    }
    await this.loadAll(id);
  }

  private async loadAll(id: string): Promise<void> {
    this.loading = true;
    try {
      const [animal, sales, invests] = await Promise.all([
        this.animalService.getAnimalById(id),
        this.animalService.getSalesHistory(id),
        this.animalService.getInvestmentHistory(id)
      ]);
      if (!animal) {
        this.notFound = true;
        return;
      }
      this.animal = animal;
      this.salesHistory = sales ?? [];
      this.investmentHistory = invests ?? [];
    } finally {
      this.loading = false;
    }
  }

  openSellDialog(): void {
    if (!this.animal) return;
    const cfg = new MatDialogConfig();
    cfg.disableClose = false;
    cfg.width = '600px';
    cfg.data = { animal: this.animal };
    const ref = this.dialog.open(SellAnimalDialogComponent, cfg);
    ref.afterClosed().subscribe(async (result: any) => {
      if (result && result.success && this.animal) {
        await this.loadAll(this.animal.id);
        this.showListingSuccessSnackbar(result.categoryLabel);
      }
    });
  }

  private showListingSuccessSnackbar(categoryLabel: string): void {
    const userAppUrl = this.configService.userAppUrl;
    const message = `🎉 Đã đăng tin "${categoryLabel}" lên sàn giao dịch!`;
    const ref = this.snackBar.open(message, userAppUrl ? 'Xem trên sàn ↗' : 'OK', {
      duration: 8000,
      panelClass: 'sell-success-snackbar'
    });
    if (userAppUrl) {
      ref.onAction().subscribe(() => {
        window.open(`${userAppUrl.replace(/\/+$/, '')}/marketplace`, '_blank', 'noopener');
      });
    }
  }

  openInvestDialog(): void {
    if (!this.animal) return;
    const cfg = new MatDialogConfig();
    cfg.disableClose = false;
    cfg.width = '560px';
    cfg.data = { animal: this.animal };
    const ref = this.dialog.open(OpenInvestmentDialogComponent, cfg);
    ref.afterClosed().subscribe(async created => {
      if (created && this.animal) await this.loadAll(this.animal.id);
    });
  }

  goBack(): void { this.router.navigateByUrl('/app/animals'); }

  /** Persist a new status on an existing listing (e.g. Active → Sold). */
  async changeListingStatus(item: AnimalSaleHistoryItem, newStatus: number): Promise<void> {
    if (item.status === newStatus) return;
    try {
      await this.animalService.updateListing({
        id: item.id,
        title: item.title,
        category: item.category,
        species: this.animal?.species ?? 0,
        status: newStatus,
        price: item.price,
        currency: item.currency,
        quantity: item.quantity,
        unit: item.unit
      });
      item.status = newStatus;
      this.snackBar.open(`✅ Đã cập nhật trạng thái thành "${this.LISTING_STATUS[newStatus]}"`, '', { duration: 3000 });
    } catch (e: any) {
      this.snackBar.open('Lỗi cập nhật: ' + (e?.error?.error ?? e?.message ?? 'unknown'), 'OK', { duration: 5000 });
    }
  }

  viewOnMarketplace(): void {
    const userAppUrl = this.configService.userAppUrl;
    if (!userAppUrl) {
      this.snackBar.open('Chưa cấu hình URL của webuser. Liên hệ admin để bổ sung USER_APP_URL.', 'OK', { duration: 4000 });
      return;
    }
    window.open(`${userAppUrl.replace(/\/+$/, '')}/marketplace`, '_blank', 'noopener');
  }

  // ---- Display helpers ----
  speciesLabel(s: number | undefined): string {
    return ({ 0: 'Hươu', 1: 'Cừu', 2: 'Bò' } as Record<number, string>)[s ?? -1] ?? '—';
  }
  genderLabel(g: number | undefined): string {
    return ({ 0: 'Đực', 1: 'Cái' } as Record<number, string>)[g ?? -1] ?? '—';
  }
  healthLabel(h: number | undefined): string {
    return ({ 0: 'Khoẻ mạnh', 1: 'Cần theo dõi', 2: 'Bệnh' } as Record<number, string>)[h ?? -1] ?? '—';
  }
  categoryLabel(c: number | undefined): string {
    return ({ 0: 'Giống', 1: 'Nhung', 2: 'Thịt', 3: 'Trứng', 99: 'Khác' } as Record<number, string>)[c ?? -1] ?? '—';
  }
  categoryEmoji(c: number | undefined): string {
    return ({ 0: '🐄', 1: '🦌', 2: '🥩', 3: '🥚', 99: '📦' } as Record<number, string>)[c ?? -1] ?? '📦';
  }
}
