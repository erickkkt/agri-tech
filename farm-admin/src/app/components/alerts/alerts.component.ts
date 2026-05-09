import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Alert } from '../../models/alert.model';
import { AlertSeverity, AlertSeverityLabels, AlertType, AlertTypeLabels } from '../../models/enum/alert.enum';
import { AlertService } from '../../services/alert.service';
import { RealtimeService } from '../../services/realtime.service';

/**
 * Phase 1 - Alerts inbox.
 * Listens to SignalR `AlertCreated` and re-fetches when new ones arrive.
 */
@Component({
  selector: 'app-alerts',
  standalone: false,
  templateUrl: './alerts.component.html'
})
export class AlertsComponent implements OnInit, OnDestroy {
  alerts: Alert[] = [];
  total = 0;
  unreadOnly = true;
  pageIndex = 0;
  pageSize = 20;

  typeLabels = AlertTypeLabels;
  severityLabels = AlertSeverityLabels;
  AlertSeverity = AlertSeverity;
  AlertType = AlertType;

  private sub?: Subscription;

  constructor(private alertService: AlertService, private realtime: RealtimeService) { }

  async ngOnInit() {
    await this.load();
    this.sub = this.realtime.alert$.subscribe(() => this.load());
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  async load() {
    const result = await this.alertService.getAlerts(this.pageIndex, this.pageSize, this.unreadOnly);
    if (result) {
      this.alerts = result.items;
      this.total = result.total;
    }
  }

  async markRead(alert: Alert) {
    await this.alertService.markRead(alert.id);
    await this.load();
  }

  async markAllRead() {
    await this.alertService.markAllRead();
    await this.load();
  }

  severityColor(s: AlertSeverity): string {
    switch (s) {
      case AlertSeverity.Critical: return '#d33';
      case AlertSeverity.Warning: return '#e80';
      default: return '#0a7';
    }
  }

  getTypeLabel(type: AlertType): string {
    return this.typeLabels[type];
  }

  getSeverityLabel(s: AlertSeverity): string {
    return this.severityLabels[s];
  }
}
