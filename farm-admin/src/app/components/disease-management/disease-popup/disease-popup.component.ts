import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DiseaseRecord, DiseaseSeverity, DiseaseStatus } from '../../../models/disease.model';
import { DiseaseService } from '../../../services/disease.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-disease-popup',
  standalone: false,
  templateUrl: './disease-popup.component.html'
})
export class DiseasePopupComponent {
  form: FormGroup;
  DiseaseSeverity = DiseaseSeverity;
  DiseaseStatus = DiseaseStatus;

  constructor(
    private fb: FormBuilder,
    private diseaseService: DiseaseService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<DiseasePopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { animalId: string }
  ) {
    this.form = this.fb.group({
      diseaseName: ['', [Validators.required, Validators.maxLength(250)]],
      diagnosedAt: [new Date(), Validators.required],
      diagnosedBy: ['', Validators.maxLength(250)],
      severity: [DiseaseSeverity.Mild, Validators.required],
      status: [DiseaseStatus.Active, Validators.required],
      notes: ['', Validators.maxLength(2000)]
    });
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload: DiseaseRecord = {
      id: '',
      animalId: this.data.animalId,
      animalCode: '', animalName: '',
      diseaseName: this.form.value.diseaseName,
      diagnosedAt: this.form.value.diagnosedAt,
      diagnosedBy: this.form.value.diagnosedBy,
      severity: this.form.value.severity,
      status: this.form.value.status,
      notes: this.form.value.notes
    };
    try {
      await this.diseaseService.create(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Bệnh', message: 'Đã ghi nhận!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Bệnh', message: e?.error?.error ?? 'Không thể lưu.' });
    }
  }

  onCancel() { this.dialogRef.close(false); }
}
