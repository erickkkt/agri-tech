import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Farm } from '../../../models/farm.model';
import { Status } from '../../../models/enum/status.enum';
import { HttpBaseService } from '../../../shared/services/http-base.service';
import { ApiEndPoints } from '../../../shared/config/api-end-points';
import { DialogService } from '../../../shared/services/dialog.service';
import { Utility } from '../../../shared/utils/utility';

@Component({
  selector: 'app-farm-popup',
  standalone: false,
  templateUrl: './farm-popup.component.html',
  styleUrl: './farm-popup.component.css'
})

export class FarmPopupComponent implements OnInit {

  updateFarm: Farm;
  updateMode = false;

  form!: FormGroup;

  Status = Status;

  constructor(
    private readonly _httpService: HttpBaseService,
    private readonly _api: ApiEndPoints,
    public dialogService: DialogService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<FarmPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.updateFarm = data.farm;
    this.createForm();
    if (this.updateFarm) {
      this.updateMode = true;
      this.initDataForUpdate(this.updateFarm);
    }
  }

  async ngOnInit() {
  }

  createForm() {
    this.form = this.formBuilder.group({
      name: ['', [Validators.required]],
      description: [''],
      ownerName: ['',[Validators.required]],
      status: [Status.Active, [Validators.required]],
      location: ['']
    });
  }

  get name() {
    return this.form.get('name') as AbstractControl;
  }

  get description() {
    return this.form.get('description') as AbstractControl;
  }

  get ownerName() {
    return this.form.get('ownerName') as AbstractControl;
  }

  get status() {
    return this.form.get('status') as AbstractControl;
  }

  get location() {
    return this.form.get('location') as AbstractControl;
  }

  initDataForUpdate(farm: Farm) {
    if (farm) {
      this.name.setValue(farm.name);
      this.description.setValue(farm.description);
      this.ownerName.setValue(farm.ownerName);
      this.status.setValue(farm.isActive ? Status.Active : Status.Inactive);
      this.location.setValue(farm.location);
    }
  }

  async onSave() {
    await this.save(true);
  }

  async save(closeDialog: boolean) {
    if (this.form.valid) {

      let farm: Farm = {
        id: this.updateMode ? this.updateFarm.id : '',
        name: this.name.value,
        isActive: this.status.value == 'Active' ? true : false,
        description: this.description.value,
        ownerName: this.ownerName.value,
        location: this.location.value
      };

      if (!this.updateMode) {
        await this._httpService.postDataAsync<string>(this._api.createFarm(), farm);
      }
      else {
        await this._httpService.putDataAsync<Farm>(this._api.updateFarm(), farm);
      }

      this.dialogService.openSuccessDialogConfirm({ title: "Farm Management", message: 'Farm saved successful!' }).subscribe((result: any) => {
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
      this.dialogService.openConfirmationDialog({ title: "Farm Management", message: 'Do you want to cancel?' }).subscribe((result: any) => {
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


