import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Animal } from '../../../../models/animal.model';
import { AnimalService, OpenInvestmentDto } from '../../../../services/animal.service';

/**
 * "1 animal = 1 investment" model. Admin enters the total amount they want
 * to raise and the profit share. We send totalShares=1, pricePerShare=amount
 * so the backend (which still has share semantics) treats it as a single-buyer
 * offer; farm-user UI also hides the share concept.
 */
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
  totalAmount = 10_000_000;
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

  cancel(): void { this.dialogRef.close(false); }

  async submit(): Promise<void> {
    this.error = null;
    if (this.totalAmount <= 0) { this.error = 'Mức huy động phải > 0'; return; }
    if (this.profitRatioPct < 0 || this.profitRatioPct > 100) {
      this.error = '% lợi nhuận phải trong khoảng 0-100'; return;
    }

    this.submitting = true;
    try {
      const dto: OpenInvestmentDto = {
        title: this.title.trim() || undefined,
        description: this.description?.trim() || undefined,
        totalShares: 1,                          // single-animal offer
        pricePerShare: this.totalAmount,
        profitRatio: this.profitRatioPct / 100,
        expectedHarvestDate: this.expectedHarvestDate || undefined
      };
      await this.animalService.openInvestment(this.animal.id, dto);
      this.dialogRef.close(true);
    } catch (e: any) {
      this.error = e?.error?.error ?? e?.message ?? 'Lỗi tạo cơ hội đầu tư';
    } finally {
      this.submitting = false;
    }
  }
}
