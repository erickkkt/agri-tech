import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Animal } from '../../../../models/animal.model';
import { AnimalService, ListForSaleDto, PriceSuggestion } from '../../../../services/animal.service';

/** UI-friendly category presets — drives icons, default unit, and price-suggest hints. */
interface CategoryPreset {
  value: number;
  label: string;
  emoji: string;
  defaultUnit: string;
  hint: string;
}

const CATEGORIES: CategoryPreset[] = [
  { value: 0,  label: 'Vật nuôi giống', emoji: '🐄', defaultUnit: 'con', hint: 'Bán cả con để nhân giống' },
  { value: 1,  label: 'Nhung hươu',     emoji: '🦌', defaultUnit: 'kg',  hint: 'Nhung tươi đã thu hoạch' },
  { value: 2,  label: 'Thịt',           emoji: '🥩', defaultUnit: 'kg',  hint: 'Thịt thương phẩm' },
  { value: 3,  label: 'Trứng đà điểu',  emoji: '🥚', defaultUnit: 'quả', hint: 'Trứng tươi từ trang trại' }
];

@Component({
  selector: 'app-sell-animal-dialog',
  templateUrl: './sell-animal-dialog.component.html',
  styleUrls: ['./sell-animal-dialog.component.css'],
  standalone: false
})
export class SellAnimalDialogComponent implements OnInit {

  animal: Animal;
  suggestion: PriceSuggestion | null = null;
  readonly categories = CATEGORIES;

  // form fields
  title = '';
  description = '';
  category: CategoryPreset = CATEGORIES[0];
  price = 0;
  currency = 'VND';
  quantity = 1;
  unit = 'con';
  province = '';
  photoUrls: string[] = [''];   // start with one empty input

  submitting = false;
  error: string | null = null;
  createdListingId: string | null = null;

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

  selectCategory(cat: CategoryPreset): void {
    this.category = cat;
    this.unit = cat.defaultUnit;
    // For non-breeding categories, reset quantity to a more sensible default
    if (cat.value !== 0 && this.quantity === 1) {
      this.quantity = cat.value === 3 ? 10 : 5;   // 10 quả trứng / 5 kg
    }
  }

  addPhoto(): void { this.photoUrls.push(''); }
  removePhoto(i: number): void { this.photoUrls.splice(i, 1); }
  trackByIndex(i: number): number { return i; }

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
        category: this.category.value,
        price: this.price,
        currency: this.currency,
        quantity: this.quantity,
        unit: this.unit,
        province: this.province?.trim() || undefined,
        photoUrls: this.photoUrls
          .map(u => u?.trim())
          .filter((u): u is string => !!u && u.length > 0)
      };
      const listingId = await this.animalService.listForSale(this.animal.id, dto);
      this.createdListingId = listingId ?? null;

      // Close with payload so caller can show snackbar with marketplace link.
      this.dialogRef.close({
        success: true,
        listingId,
        categoryLabel: this.category.label
      });
    } catch (e: any) {
      this.error = e?.error?.error ?? e?.message ?? 'Lỗi tạo tin đăng';
    } finally {
      this.submitting = false;
    }
  }
}
