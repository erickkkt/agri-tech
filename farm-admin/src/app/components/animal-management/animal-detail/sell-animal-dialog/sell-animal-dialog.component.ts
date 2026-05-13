import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Animal } from '../../../../models/animal.model';
import { AnimalService, ListForSaleDto, PriceSuggestion } from '../../../../services/animal.service';

@Component({
  selector: 'app-sell-animal-dialog',
  templateUrl: './sell-animal-dialog.component.html',
  styleUrls: ['./sell-animal-dialog.component.css'],
  standalone: false
})
export class SellAnimalDialogComponent implements OnInit {

  animal: Animal;
  suggestion: PriceSuggestion | null = null;

  // form fields
  title = '';
  description = '';
  category = 0;     // ListingCategory.Breeding
  price = 0;
  currency = 'VND';
  quantity = 1;
  unit = 'con';
  province = '';

  submitting = false;
  error: string | null = null;

  constructor(
    private readonly dialogRef: MatDialogRef<SellAnimalDialogComponent>,
    private readonly animalService: AnimalService,
    @Inject(MAT_DIALOG_DATA) private readonly data: { animal: Animal }
  ) {
    this.animal = data.animal;
    this.title = `${this.animal.name} (${this.animal.code})`;
    this.description = this.animal.description ?? '';
  }

  async ngOnInit(): Promise<void> {
    try {
      this.suggestion = (await this.animalService.getPriceSuggestion(this.animal.id)) ?? null;
      if (this.suggestion) this.price = this.suggestion.suggestedPrice;
    } catch { /* non-fatal */ }
  }

  cancel(): void { this.dialogRef.close(false); }

  async submit(): Promise<void> {
    this.error = null;
    if (this.price <= 0) { this.error = 'Giá phải lớn hơn 0'; return; }
    if (this.quantity <= 0) { this.error = 'Số lượng phải > 0'; return; }

    this.submitting = true;
    try {
      const dto: ListForSaleDto = {
        title: this.title.trim() || undefined,
        description: this.description?.trim() || undefined,
        category: this.category,
        price: this.price,
        currency: this.currency,
        quantity: this.quantity,
        unit: this.unit,
        province: this.province?.trim() || undefined
      };
      await this.animalService.listForSale(this.animal.id, dto);
      this.dialogRef.close(true);
    } catch (e: any) {
      this.error = e?.error?.error ?? e?.message ?? 'Lỗi tạo tin đăng';
    } finally {
      this.submitting = false;
    }
  }
}
