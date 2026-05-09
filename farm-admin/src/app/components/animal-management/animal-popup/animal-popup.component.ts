import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Animal } from '../../../models/animal.model';
import { Status } from '../../../models/enum/status.enum';
import { HttpBaseService } from '../../../shared/services/http-base.service';
import { ApiEndPoints } from '../../../shared/config/api-end-points';
import { DialogService } from '../../../shared/services/dialog.service';
import { Utility } from '../../../shared/utils/utility';
import { HealthStatus } from '../../../models/enum/health-status.enum';
import { Species } from '../../../models/enum/species.enum';
import { Gender } from '../../../models/enum/gender.enum';
import { Farm } from '../../../models/farm.model';
import { Cage } from '../../../models/cage.model';

@Component({
  selector: 'app-animal-popup',
  standalone: false,
  templateUrl: './animal-popup.component.html',
  styleUrl: './animal-popup.component.css'
})

export class AnimalPopupComponent implements OnInit {

  updateAnimal: Animal;
  updateMode = false;

  farms: Farm[] = [];
  cages: Cage[] = [];

  form!: FormGroup;

  Status = Status;
  Species = Species;
  HealthStatus = HealthStatus;
  Gender = Gender;

  constructor(
    private readonly _httpService: HttpBaseService,
    private readonly _api: ApiEndPoints,
    public dialogService: DialogService,
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<AnimalPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.updateAnimal = data.animal;
    this.createForm();
    if (this.updateAnimal) {
      this.updateMode = true;
      this.initDataForUpdate(this.updateAnimal);
    }
  }

  async ngOnInit() {
    await this.loadFarms();
    if (this.updateAnimal && this.updateAnimal.farmId) {
      await this.loadCages(this.updateAnimal.farmId);
    }

    if (this.updateMode && this.updateAnimal) {
      this.initDataForUpdate(this.updateAnimal);
    }
    
    this.farmId.valueChanges.subscribe((farmId: string) => {
      if (farmId) {
        this.loadCages(farmId);
        this.cageId.setValue('');
      } else {
        this.cages = [];
        this.cageId.setValue('');
      }
    });
  }

  async loadCages(farmId: string) {
    const cages = await this._httpService.getDataAsync<Cage[]>(this._api.getActiveCages(farmId));
    this.cages = cages ?? [];
  }

  async loadFarms() {
      const farms = await this._httpService.getDataAsync<Farm[]>(this._api.getActiveFarms());
      this.farms = farms ?? [];
  }

  createForm() {
    this.form = this.formBuilder.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
      description: [''],
      species: [Species.Cow],
      height: [0, [Validators.min(0)]],
      weight: [0, [Validators.min(0)]],
      gender: [Gender.Male],
      cageId: ['', [Validators.required]],
      farmId: ['', [Validators.required]],
      healthStatus: [HealthStatus.Healthy],
      birthDate: [new Date()],
      arrivalDate: [new Date()],
      status: [Status.Active]
    });
  }

  get code() {
    return this.form.get('code') as AbstractControl;
  }

  get name() {
    return this.form.get('name') as AbstractControl;
  }

  get description() {
    return this.form.get('description') as AbstractControl;
  }

  get species() {
    return this.form.get('species') as AbstractControl;
  }

  get height() {
    return this.form.get('height') as AbstractControl;
  }

  get weight() {
    return this.form.get('weight') as AbstractControl;
  }

  get gender() {
    return this.form.get('gender') as AbstractControl;
  }

  get cageId() {
    return this.form.get('cageId') as AbstractControl;
  }

  get farmId() {
    return this.form.get('farmId') as AbstractControl;
  }

  get healthStatus() {
    return this.form.get('healthStatus') as AbstractControl;
  }

  get birthDate() {
    return this.form.get('birthDate') as AbstractControl;
  }

  get arrivalDate() {
    return this.form.get('arrivalDate') as AbstractControl;
  }

  get status() {
    return this.form.get('status') as AbstractControl;
  }

  initDataForUpdate(animal: Animal) {
    if (animal) {
      // Add the missing code field
      this.code.setValue(animal.code);
      this.name.setValue(animal.name);
      this.description.setValue(animal.description);
      
      if (typeof animal.species === 'string') {
        this.species.setValue(Species[animal.species as keyof typeof Species] || animal.species);
      } else {
        this.species.setValue(animal.species);
      }
      
      this.height.setValue(animal.height);      
      this.weight.setValue(animal.weight);
      
      if (typeof animal.gender === 'string') {
        this.gender.setValue(Gender[animal.gender as keyof typeof Gender] || animal.gender);
      } else {
        this.gender.setValue(animal.gender);
      }
      
      this.cageId.setValue(animal.cageId);
      this.farmId.setValue(animal.farmId);
      
      if (typeof animal.healthStatus === 'string') {
        this.healthStatus.setValue(HealthStatus[animal.healthStatus as keyof typeof HealthStatus] || animal.healthStatus);
      } else {
        this.healthStatus.setValue(animal.healthStatus);
      }
      
      // Improved date handling
      this.birthDate.setValue(animal.dateOfBirth ? new Date(animal.dateOfBirth) : new Date());
      this.arrivalDate.setValue(animal.dateOfArrival ? new Date(animal.dateOfArrival) : new Date());
      
      this.status.setValue(animal.isActive ? Status.Active : Status.Inactive);
    }
  }

  async onSave() {
    await this.save(true);
  }

  async save(closeDialog: boolean) {
    if (this.form.valid) {

      let animal: Animal = {
        id: this.updateMode ? this.updateAnimal.id : '',
        code: this.code.value,
        name: this.name.value,
        isActive: this.status.value == 'Active' ? true : false,
        description: this.description.value,
        species: typeof this.species.value === 'string' ? Species[this.species.value as keyof typeof Species] : this.species.value,
        height: this.height.value,
        weight: this.weight.value,
        gender: typeof this.gender.value === 'string' ? Gender[this.gender.value as keyof typeof Gender] : this.gender.value,
        cageId: this.cageId.value,
        farmId: this.farmId.value,
        healthStatus: typeof this.healthStatus.value === 'string' ? HealthStatus[this.healthStatus.value as keyof typeof HealthStatus] : this.healthStatus.value,
        dateOfBirth: this.birthDate.value,
        dateOfArrival: this.arrivalDate.value,
        cageName: '', 
        farmName: '' 
      };

      if (!this.updateMode) {
        await this._httpService.postDataAsync<string>(this._api.createAnimal(), animal);
      }
      else {
        await this._httpService.putDataAsync<Animal>(this._api.updateAnimal(), animal);
      }

      this.dialogService.openSuccessDialogConfirm({ title: "Animal Management", message: 'Animal saved successful!' }).subscribe((result: any) => {
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
      this.dialogService.openConfirmationDialog({ title: "Animal Management", message: 'Do you want to cancel?' }).subscribe((result: any) => {
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
