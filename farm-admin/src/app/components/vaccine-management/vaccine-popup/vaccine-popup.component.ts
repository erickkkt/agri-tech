import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Vaccine } from '../../../models/vaccine.model';
import { Species } from '../../../models/enum/species.enum';
import { VaccineService } from '../../../services/vaccine.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-vaccine-popup',
  standalone: false,
  templateUrl: './vaccine-popup.component.html'
})
export class VaccinePopupComponent {
  form: FormGroup;
  updateMode = false;
  Species = Species;

  constructor(
    private fb: FormBuilder,
    private vaccineService: VaccineService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<VaccinePopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { vaccine?: Vaccine }
  ) {
    const v = data?.vaccine;
    this.updateMode = !!v;
    this.form = this.fb.group({
      name: [v?.name ?? '', [Validators.required, Validators.maxLength(250)]],
      manufacturer: [v?.manufacturer ?? '', [Validators.maxLength(250)]],
      recommendedSpecies: [v?.recommendedSpecies ?? null],
      intervalDays: [v?.intervalDays ?? 365, [Validators.required, Validators.min(0), Validators.max(3650)]],
      description: [v?.description ?? '', [Validators.maxLength(2000)]],
      isActive: [v?.isActive ?? true]
    });
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload: Vaccine = {
      id: this.updateMode ? (this.data.vaccine!.id) : '',
      name: this.form.value.name,
      manufacturer: this.form.value.manufacturer,
      recommendedSpecies: this.form.value.recommendedSpecies ?? undefined,
      intervalDays: this.form.value.intervalDays,
      description: this.form.value.description,
      isActive: this.form.value.isActive
    };
    try {
      if (this.updateMode) await this.vaccineService.updateVaccine(payload);
      else await this.vaccineService.createVaccine(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Vaccine', message: 'Vaccine saved successfully!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Vaccine', message: e?.error?.error ?? 'Failed to save vaccine.' });
    }
  }

  onCancel() {
    if (!this.form.dirty) { this.dialogRef.close(false); return; }
    this.dialogService.openConfirmationDialog({ title: 'Vaccine', message: 'Hủy thay đổi?' })
      .subscribe(ok => { if (ok) this.dialogRef.close(false); });
  }
}
