import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Cage } from '../../../models/cage.model';
import { Status } from '../../../models/enum/status.enum';
import { HttpBaseService } from '../../../shared/services/http-base.service';
import { ApiEndPoints } from '../../../shared/config/api-end-points';
import { DialogService } from '../../../shared/services/dialog.service';
import { Utility } from '../../../shared/utils/utility';
import { Farm } from '../../../models/farm.model';

@Component({
  selector: 'app-cage-popup',
  standalone: false,
  templateUrl: './cage-popup.component.html',
  styleUrl: './cage-popup.component.css'
})

export class CagePopupComponent implements OnInit {

  updateCage: Cage;
  updateMode = false;
  farms: Farm[] = [];
  form!: FormGroup;

  Status = Status;

  constructor(
    private readonly _httpService: HttpBaseService,
    private readonly _api: ApiEndPoints,
    public dialogService: DialogService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<CagePopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.updateCage = data.cage;
    this.createForm();
    if (this.updateCage) {
      this.updateMode = true;
      this.initDataForUpdate(this.updateCage);
    }
  }

  async ngOnInit() {
    await this.loadFarms();
  }

  async loadFarms() {
    const farms = await this._httpService.getDataAsync<Farm[]>(this._api.getActiveFarms());
    this.farms = farms ?? [];
  }

  createForm() {
    this.form = this.formBuilder.group({
      name: ['', [Validators.required]],
      description: [''],
      farmId: ['', [Validators.required]],
      capacity: [0, [Validators.required, Validators.min(1)]],
      currentAnimalCount: [0, [Validators.min(0)]],
      status: [Status.Active, [Validators.required]]
    });
  }

  get name() {
    return this.form.get('name') as AbstractControl;
  }

  get description() {
    return this.form.get('description') as AbstractControl;
  }

  get farmId() {
    return this.form.get('farmId') as AbstractControl;
  }

  get capacity() {
    return this.form.get('capacity') as AbstractControl;
  }

  get currentAnimalCount() {
    return this.form.get('currentAnimalCount') as AbstractControl;
  }

  get status() {
    return this.form.get('status') as AbstractControl;
  }


  initDataForUpdate(cage: Cage) {
    if (cage) {
      this.name.setValue(cage.name);
      this.description.setValue(cage.description);
      this.farmId.setValue(cage.farmId);
      this.capacity.setValue(cage.capacity);
      this.currentAnimalCount.setValue(cage.currentAnimalCount);
      this.status.setValue(cage.isActive ? Status.Active : Status.Inactive);
    }
  }

  async onSave() {
    await this.save(true);
  }

  async save(closeDialog: boolean) {
    if (this.form.valid) {

      let cage: Cage = {
        id: this.updateMode ? this.updateCage.id : '',
        name: this.name.value,
        isActive: this.status.value == 'Active' ? true : false,
        description: this.description.value,
        farmId: this.farmId.value,
        farmName: '',
        capacity: this.capacity.value,
        currentAnimalCount: this.currentAnimalCount.value,
      };

      if (!this.updateMode) {
        await this._httpService.postDataAsync<string>(this._api.createCage(), cage);
      }
      else {
        await this._httpService.putDataAsync<Cage>(this._api.updateCage(), cage);
      }

      this.dialogService.openSuccessDialogConfirm({ title: "Cage Management", message: 'Cage saved successful!' }).subscribe((result: any) => {
        if (result) {
          if (closeDialog) {
            this.dialogRef.close(true);
          }
        }
      })
    }
    else {
      return;
    }
  }

  onCloseDialog(isReload: boolean) {
    if (this.form.dirty) {
      this.dialogService.openConfirmationDialog({ title: "Cage Management", message: 'Do you want to cancel?' }).subscribe((result: any) => {
        if (result) {
          this.dialogRef.close(isReload);
        }
      });
    }
    else {
      this.dialogRef.close(isReload);
    }
  }

  convertName(name: string) {
    this.name.setValue(Utility.convertWords(name), { emitEvent: false });
  }
}
