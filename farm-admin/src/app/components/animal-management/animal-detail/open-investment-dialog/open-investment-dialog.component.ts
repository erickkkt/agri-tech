import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Animal } from '../../../../models/animal.model';
import { AnimalService, OpenInvestmentDto } from '../../../../services/animal.service';

@Component({
  selector: 'app-open-investment-dialog',
  templateUrl: './open-investment-dialog.component.html',
  styleUrls: ['./open-investment-dialog.component.css'],
  standalone: false
})
export class OpenInvestmentDialogComponent {

  animal: Animal;

  title = '';
  description = '';
  totalShares = 100;
  pricePerShare = 100_000;
  profitRatioPct = 70;          // shown as % to admin, sent as 0..1
  expectedHarvestDate: string | null = null;

  submitting = false;
  error: string | null = null;

  constructor(
    private readonly dialogRef: MatDialogRef<OpenInvestmentDialogComponent>,
    private readonly animalService: AnimalService,
    @Inject(MAT_DIALOG_DATA) private readonly data: { animal: Animal }
  ) {
    this.animal = data.animal;
    this.title = `Đầu tư vào ${this.animal.name} (${this.animal.code})`;
    this.description = this.animal.description ?? '';
  }

  get totalRaise(): number {
    return Math.max(0, this.totalShares) * Math.max(0, this.pricePerShare);
  }

  cancel(): void { this.dialogRef.close(false); }

  async submit(): Promise<void> {
    this.error = null;
    if (this.totalShares <= 0) { this.error = 'Tổng số share phải > 0'; return; }
    if (this.pricePerShare <= 0) { this.error = 'Giá / share phải > 0'; return; }
    if (this.profitRatioPct < 0 || this.profitRatioPct > 100) {
      this.error = '% lợi nhuận phải trong khoảng 0-100'; return;
    }

    this.submitting = true;
    try {
      const dto: OpenInvestmentDto = {
        title: this.title.trim() || undefined,
        description: this.description?.trim() || undefined,
        totalShares: this.totalShares,
        pricePerShare: this.pricePerShare,
        profitRatio: this.profitRatioPct / 100,
        expectedHarvestDate: this.expectedHarvestDate || undefined
      };
      await this.animalService.openInvestment(this.animal.id, dto);
      this.dialogRef.close(true);
    } catch (e: any) {
      this.error = e?.error?.error ?? e?.message ?? 'Lỗi tạo offer';
    } finally {
      this.submitting = false;
    }
  }
}
