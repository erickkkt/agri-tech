import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FeedItem } from '../../../models/feed.model';
import { FeedService } from '../../../services/feed.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-feed-item-popup',
  standalone: false,
  templateUrl: './feed-item-popup.component.html'
})
export class FeedItemPopupComponent {
  form: FormGroup;
  updateMode = false;

  constructor(
    private fb: FormBuilder,
    private feedService: FeedService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<FeedItemPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { item?: FeedItem }
  ) {
    const it = data?.item;
    this.updateMode = !!it;
    this.form = this.fb.group({
      code: [it?.code ?? '', [Validators.required, Validators.maxLength(50)]],
      name: [it?.name ?? '', [Validators.required, Validators.maxLength(250)]],
      unit: [it?.unit ?? 'kg', [Validators.required, Validators.maxLength(50)]],
      manufacturer: [it?.manufacturer ?? '', [Validators.maxLength(250)]],
      nutritionInfo: [it?.nutritionInfo ?? '', [Validators.maxLength(2000)]],
      lowStockThreshold: [it?.lowStockThreshold ?? 10, [Validators.required, Validators.min(0)]],
      isActive: [it?.isActive ?? true]
    });
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload: FeedItem = {
      id: this.updateMode ? this.data.item!.id : '',
      ...this.form.value
    };
    try {
      if (this.updateMode) await this.feedService.updateItem(payload);
      else await this.feedService.createItem(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Thức ăn', message: 'Đã lưu!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Thức ăn', message: e?.error?.error ?? 'Lưu thất bại.' });
    }
  }

  onCancel() { this.dialogRef.close(false); }
}
