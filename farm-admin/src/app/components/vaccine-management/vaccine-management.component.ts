import { Component, OnInit } from '@angular/core';
import { MatDialogConfig } from '@angular/material/dialog';
import { Vaccine, VaccineSchedule } from '../../models/vaccine.model';
import { VaccineStatus, VaccineStatusLabels } from '../../models/enum/vaccine-status.enum';
import { VaccineService } from '../../services/vaccine.service';
import { DialogService } from '../../shared/services/dialog.service';
import { VaccinePopupComponent } from './vaccine-popup/vaccine-popup.component';
import { VaccineSchedulePopupComponent } from './vaccine-schedule-popup/vaccine-schedule-popup.component';

/**
 * Phase 1 - Vaccine catalog & schedule overview.
 * Two tabs: Catalog (vaccines) and Upcoming schedules (next 7 days + overdue).
 */
@Component({
  selector: 'app-vaccine-management',
  standalone: false,
  templateUrl: './vaccine-management.component.html'
})
export class VaccineManagementComponent implements OnInit {
  selectedTab: 'catalog' | 'schedule' = 'catalog';

  vaccines: Vaccine[] = [];
  vaccinesTotal = 0;
  pageIndex = 0;
  pageSize = 10;

  upcoming: VaccineSchedule[] = [];

  statusLabels = VaccineStatusLabels;
  VaccineStatus = VaccineStatus;

  configDialog = new MatDialogConfig();

  constructor(
    private vaccineService: VaccineService,
    private dialogService: DialogService
  ) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '560px';
  }

  async ngOnInit() {
    await Promise.all([this.loadVaccines(), this.loadUpcoming()]);
  }

  async loadVaccines() {
    const result = await this.vaccineService.getVaccines('Name', 'asc', this.pageIndex, this.pageSize);
    if (result) {
      this.vaccines = result.items;
      this.vaccinesTotal = result.total;
    }
  }

  async loadUpcoming() {
    this.upcoming = (await this.vaccineService.getUpcoming(7)) ?? [];
  }

  async administer(s: VaccineSchedule) {
    const by = window.prompt('Người tiêm:');
    if (!by) return;
    await this.vaccineService.administer({
      scheduleId: s.id,
      administeredDate: new Date(),
      administeredBy: by
    });
    await this.loadUpcoming();
  }

  getStatusLabel(status: VaccineStatus): string {
    return this.statusLabels[status];
  }

  createVaccine() {
    this.dialogService.openComponentDialog(VaccinePopupComponent, {}, this.configDialog)
      .subscribe(async ok => { if (ok) await this.loadVaccines(); });
  }

  editVaccine(v: Vaccine) {
    this.dialogService.openComponentDialog(VaccinePopupComponent, { vaccine: v }, this.configDialog)
      .subscribe(async ok => { if (ok) await this.loadVaccines(); });
  }

  createSchedule() {
    this.dialogService.openComponentDialog(VaccineSchedulePopupComponent, {}, this.configDialog)
      .subscribe(async ok => { if (ok) await this.loadUpcoming(); });
  }
}
