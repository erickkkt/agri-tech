import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { DiseaseRecord, Treatment } from '../models/disease.model';

@Injectable({ providedIn: 'root' })
export class DiseaseService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getByAnimal(animalId: string) {
    return this.http.getDataAsync<DiseaseRecord[]>(this.api.getDiseasesByAnimal(animalId));
  }

  create(record: DiseaseRecord) {
    return this.http.postDataAsync<string>(this.api.createDisease(), record);
  }

  update(record: DiseaseRecord) {
    return this.http.putDataAsync<DiseaseRecord>(this.api.updateDisease(), record);
  }

  createTreatment(t: Treatment) {
    return this.http.postDataAsync<string>(this.api.createTreatment(), t);
  }

  getTreatments(animalId: string) {
    return this.http.getDataAsync<Treatment[]>(this.api.getTreatmentsByAnimal(animalId));
  }
}
