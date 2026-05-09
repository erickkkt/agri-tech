import { Component, OnInit } from '@angular/core';
import { MatDialogConfig } from '@angular/material/dialog';
import { Animal } from '../../models/animal.model';
import { DiseaseRecord, Treatment, DiseaseSeverity, DiseaseStatus } from '../../models/disease.model';
import { AnimalService } from '../../services/animal.service';
import { DiseaseService } from '../../services/disease.service';
import { DialogService } from '../../shared/services/dialog.service';
import { DiseasePopupComponent } from './disease-popup/disease-popup.component';
import { TreatmentPopupComponent } from './treatment-popup/treatment-popup.component';

/**
 * Phase 1 - Disease & treatment history per animal.
 * Pick an animal, view diseases + treatments, add new entries.
 */
@Component({
  selector: 'app-disease-management',
  standalone: false,
  templateUrl: './disease-management.component.html'
})
export class DiseaseManagementComponent implements OnInit {
  animals: Animal[] = [];
  selectedAnimalId: string | null = null;
  diseases: DiseaseRecord[] = [];
  treatments: Treatment[] = [];

  configDialog = new MatDialogConfig();

  severityLabels: Record<DiseaseSeverity, string> = {
    [DiseaseSeverity.Mild]: 'Nhẹ',
    [DiseaseSeverity.Moderate]: 'Vừa',
    [DiseaseSeverity.Severe]: 'Nặng',
    [DiseaseSeverity.Critical]: 'Nguy kịch'
  };
  statusLabels: Record<DiseaseStatus, string> = {
    [DiseaseStatus.Active]: 'Đang bệnh',
    [DiseaseStatus.UnderTreatment]: 'Đang điều trị',
    [DiseaseStatus.Recovered]: 'Đã khỏi',
    [DiseaseStatus.Fatal]: 'Tử vong'
  };

  constructor(
    private animalService: AnimalService,
    private diseaseService: DiseaseService,
    private dialogService: DialogService
  ) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '560px';
  }

  async ngOnInit() {
    const result = await this.animalService.getAnimals('Name', 'asc', 0, 200);
    this.animals = result?.items ?? [];
    if (this.animals.length) {
      this.selectedAnimalId = this.animals[0].id;
      await this.loadAll();
    }
  }

  async loadAll() {
    if (!this.selectedAnimalId) return;
    const [d, t] = await Promise.all([
      this.diseaseService.getByAnimal(this.selectedAnimalId),
      this.diseaseService.getTreatments(this.selectedAnimalId)
    ]);
    this.diseases = d ?? [];
    this.treatments = t ?? [];
  }

  createDisease() {
    if (!this.selectedAnimalId) return;
    this.dialogService.openComponentDialog(
      DiseasePopupComponent, { animalId: this.selectedAnimalId }, this.configDialog
    ).subscribe(async ok => { if (ok) await this.loadAll(); });
  }

  createTreatment() {
    if (!this.selectedAnimalId) return;
    this.dialogService.openComponentDialog(
      TreatmentPopupComponent,
      { animalId: this.selectedAnimalId, diseases: this.diseases },
      this.configDialog
    ).subscribe(async ok => { if (ok) await this.loadAll(); });
  }

  getSeverityLabel(s: DiseaseSeverity): string {
    return this.severityLabels[s];
  }

  getStatusLabel(s: DiseaseStatus): string {
    return this.statusLabels[s];
  }
}
