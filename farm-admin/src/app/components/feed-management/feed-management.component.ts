import { Component, OnInit } from '@angular/core';
import { MatDialogConfig } from '@angular/material/dialog';
import { FeedItem, FeedStockLevel, FeedTransaction } from '../../models/feed.model';
import { FeedTransactionType, FeedTransactionTypeLabels } from '../../models/enum/feed.enum';
import { FeedService } from '../../services/feed.service';
import { FarmService } from '../../services/farm.service';
import { Farm } from '../../models/farm.model';
import { DialogService } from '../../shared/services/dialog.service';
import { FeedItemPopupComponent } from './feed-item-popup/feed-item-popup.component';
import { FeedTransactionPopupComponent } from './feed-transaction-popup/feed-transaction-popup.component';

/**
 * Phase 1 - Feed inventory management.
 * Tabs: Items catalog, Per-farm summary, Recent transactions, Low-stock alerts.
 */
@Component({
  selector: 'app-feed-management',
  standalone: false,
  templateUrl: './feed-management.component.html'
})
export class FeedManagementComponent implements OnInit {
  items: FeedItem[] = [];
  transactions: FeedTransaction[] = [];
  lowStock: FeedStockLevel[] = [];
  farms: Farm[] = [];
  selectedFarmId: string | null = null;
  summaryItems: FeedStockLevel[] = [];

  txTypeLabels = FeedTransactionTypeLabels;
  FeedTransactionType = FeedTransactionType;

  configDialog = new MatDialogConfig();

  constructor(
    private feedService: FeedService,
    private farmService: FarmService,
    private dialogService: DialogService
  ) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '560px';
  }

  async ngOnInit() {
    await Promise.all([this.loadItems(), this.loadLowStock(), this.loadFarms()]);
    if (this.farms.length) {
      this.selectedFarmId = this.farms[0].id;
      await Promise.all([this.loadSummary(), this.loadTransactions()]);
    }
  }

  async loadItems() {
    const result = await this.feedService.getItems(0, 50);
    if (result) this.items = result.items;
  }

  async loadLowStock() {
    this.lowStock = (await this.feedService.getLowStock()) ?? [];
  }

  async loadFarms() {
    try {
      this.farms = (await this.farmService.getActiveFarms()) ?? [];
    } catch {
      this.farms = [];
    }
  }

  async loadSummary() {
    if (!this.selectedFarmId) return;
    const summary = await this.feedService.getSummary(this.selectedFarmId);
    this.summaryItems = summary?.items ?? [];
  }

  async loadTransactions() {
    this.transactions = (await this.feedService.getTransactions({ farmId: this.selectedFarmId ?? undefined })) ?? [];
  }

  createItem() {
    this.dialogService.openComponentDialog(FeedItemPopupComponent, {}, this.configDialog)
      .subscribe(async ok => { if (ok) await this.loadItems(); });
  }

  editItem(it: FeedItem) {
    this.dialogService.openComponentDialog(FeedItemPopupComponent, { item: it }, this.configDialog)
      .subscribe(async ok => { if (ok) await this.loadItems(); });
  }

  createTransaction() {
    const data = { farmId: this.selectedFarmId ?? undefined };
    this.dialogService.openComponentDialog(FeedTransactionPopupComponent, data, this.configDialog)
      .subscribe(async ok => {
        if (ok) await Promise.all([this.loadTransactions(), this.loadSummary(), this.loadLowStock()]);
      });
  }

  getTxTypeLabel(type: FeedTransactionType): string {
    return this.txTypeLabels[type];
  }
}
