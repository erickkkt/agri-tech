import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Animal } from '../../../models/animal.model';
import { Vaccine } from '../../../models/vaccine.model';
import { AnimalService } from '../../../services/animal.service';
import { VaccineService } from '../../../services/vaccine.service';
import { DialogService } from '../../../shared/services/dialog.service';

@Component({
  selector: 'app-vaccine-schedule-popup',
  standalone: false,
  templateUrl: './vaccine-schedule-popup.component.html'
})
export class VaccineSchedulePopupComponent implements OnInit {
  form: FormGroup;
  animals: Animal[] = [];
  vaccines: Vaccine[] = [];

  constructor(
    private fb: FormBuilder,
    private animalService: AnimalService,
    private vaccineService: VaccineService,
    private dialogService: DialogService,
    public dialogRef: MatDialogRef<VaccineSchedulePopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { animalId?: string }
  ) {
    this.form = this.fb.group({
      animalId: [data?.animalId ?? '', Validators.required],
      vaccineId: ['', Validators.required],
      scheduledDate: [new Date(), Validators.required],
      notes: ['']
    });
  }

  async ngOnInit() {
    const [a, v] = await Promise.all([
      this.animalService.getAnimals('Name', 'asc', 0, 200),
      this.vaccineService.getVaccines('Name', 'asc', 0, 200)
    ]);
    this.animals = a?.items ?? [];
    this.vaccines = (v?.items ?? []).filter(x => x.isActive);
  }

  async onSave() {
    if (this.form.invalid) return;
    const payload = {
      animalId: this.form.value.animalId,
      vaccineId: this.form.value.vaccineId,
      scheduledDate: this.form.value.scheduledDate,
      notes: this.form.value.notes
    };
    try {
      await this.vaccineService.createSchedule(payload);
      this.dialogService.openSuccessDialogConfirm({ title: 'Lịch tiêm', message: 'Đã tạo lịch!' })
        .subscribe(() => this.dialogRef.close(true));
    } catch (e: any) {
      this.dialogService.openErrorDialog({ title: 'Lịch tiêm', message: e?.error?.error ?? 'Không tạo được lịch.' });
    }
  }

  onCancel() { this.dialogRef.close(false); }
}
