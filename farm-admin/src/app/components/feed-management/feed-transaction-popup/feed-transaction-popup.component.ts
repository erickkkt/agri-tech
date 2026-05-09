import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Farm } from '../../../models/farm.model';
import { CreateFeedTransaction, FeedItem } from '../../../models/feed.model';
import { FeedTransactionType, FeedTransactionTypeLabels } from '../../../models/enum/feed.enum';
import { FarmService } from '../../../services/farm.service';
import { FeedService } from '../../../services/feed.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-feed-transaction-popup',
  standalone: false,
  templateUrl: './feed-transaction-popup.component.html'
})
export class FeedTransactionPopupComponent implements OnInit {
  form: FormGroup;
  farms: Farm[] = [];
  items: FeedItem[] = [];
  txTypes = Object.entries(FeedTransactionTypeLabels)
    .map(([k, v]) => ({ value: Number(k) as FeedTransactionType, label: v }));

  constructor(
    private fb: FormBuilder,
    private farmService: FarmService,
    private feedService: FeedService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<FeedTransactionPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { farmId?: string }
  ) {
    this.form = this.fb.group({
      feedItemId: ['', Validators.required],
      farmId: [data?.farmId ?? '', Validators.required],
      type: [FeedTransactionType.In, Validators.required],
      quantity: [0, [Validators.required, Validators.min(0.0001)]],
      unitPrice: [null],
      transactionDate: [new Date(), Validators.required],
      notes: ['']
    });
  }

  async ngOnInit() {
    try {
      this.farms = (await this.farmService.getActiveFarms()) ?? [];
    } catch { this.farms = []; }
    const items = await this.feedService.getItems(0, 200);
    this.items = (items?.items ?? []).filter(i => i.isActive);
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload: CreateFeedTransaction = {
      feedItemId: this.form.value.feedItemId,
      farmId: this.form.value.farmId,
      type: this.form.value.type,
      quantity: this.form.value.quantity,
      unitPrice: this.form.value.unitPrice ?? undefined,
      transactionDate: this.form.value.transactionDate,
      notes: this.form.value.notes
    };
    try {
      await this.feedService.createTransaction(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Kho', message: 'Đã ghi nhận giao dịch!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Kho', message: e?.error?.error ?? 'Không thể ghi nhận.' });
    }
  }

  onCancel() { this.dialogRef.close(false); }
}
