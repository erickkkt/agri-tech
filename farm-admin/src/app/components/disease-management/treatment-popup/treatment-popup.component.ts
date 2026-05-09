import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DiseaseRecord, Treatment } from '../../../models/disease.model';
import { DiseaseService } from '../../../services/disease.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-treatment-popup',
  standalone: false,
  templateUrl: './treatment-popup.component.html'
})
export class TreatmentPopupComponent {
  form: FormGroup;
  diseases: DiseaseRecord[];

  constructor(
    private fb: FormBuilder,
    private diseaseService: DiseaseService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<TreatmentPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { animalId: string; diseases: DiseaseRecord[] }
  ) {
    this.diseases = data?.diseases ?? [];
    this.form = this.fb.group({
      diseaseRecordId: [null],
      medication: ['', [Validators.required, Validators.maxLength(250)]],
      dosage: ['', Validators.maxLength(250)],
      startDate: [new Date(), Validators.required],
      endDate: [null],
      administeredBy: ['', Validators.maxLength(250)],
      outcome: ['', Validators.maxLength(2000)]
    });
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload: Treatment = {
      id: '',
      animalId: this.data.animalId,
      diseaseRecordId: this.form.value.diseaseRecordId ?? undefined,
      diseaseName: '',
      medication: this.form.value.medication,
      dosage: this.form.value.dosage,
      startDate: this.form.value.startDate,
      endDate: this.form.value.endDate ?? undefined,
      administeredBy: this.form.value.administeredBy,
      outcome: this.form.value.outcome
    };
    try {
      await this.diseaseService.createTreatment(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Điều trị', message: 'Đã ghi nhận!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Điều trị', message: e?.error?.error ?? 'Không thể lưu.' });
    }
  }

  onCancel() { this.dialogRef.close(false); }
}
