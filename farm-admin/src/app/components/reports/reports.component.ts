import { Component, OnInit } from '@angular/core';
import { Dashboard } from '../../models/dashboard.model';
import { ReportService } from '../../services/report.service';

/**
 * Phase 1 - Reports / Dashboard.
 * Renders KPI cards + species/health distributions + 6-month average weight trend.
 */
@Component({
  selector: 'app-reports',
  standalone: false,
  templateUrl: './reports.component.html'
})
export class ReportsComponent implements OnInit {
  dashboard: Dashboard | null = null;
  loading = true;

  constructor(private reportService: ReportService) { }

  async ngOnInit() {
    await this.refresh();
  }

  async refresh() {
    this.loading = true;
    try {
      this.dashboard = (await this.reportService.getDashboard()) ?? null;
    } finally {
      this.loading = false;
    }
  }

  monthLabel(year: number, month: number): string {
    return `${year}-${month.toString().padStart(2, '0')}`;
  }
}
