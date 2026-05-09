import { Injectable } from '@angular/core';
import { ApiEndPoints } from '../shared/config/api-end-points';
import { HttpBaseService } from '../shared/services/http-base.service';
import { PaginationResponse } from '../models/pagination-response.model';
import { AdministerVaccine, CreateVaccineSchedule, Vaccine, VaccineSchedule } from '../models/vaccine.model';

@Injectable({ providedIn: 'root' })
export class VaccineService {
  constructor(private api: ApiEndPoints, private http: HttpBaseService) { }

  getVaccines(sortField = 'Name', sortDirection = 'asc', pageIndex = 0, pageSize = 10) {
    return this.http.getDataAsync<PaginationResponse<Vaccine>>(this.api.getVaccinesPaging(sortField, sortDirection, pageIndex, pageSize));
  }

  createVaccine(v: Vaccine) {
    return this.http.postDataAsync<string>(this.api.createVaccine(), v);
  }

  updateVaccine(v: Vaccine) {
    return this.http.putDataAsync<Vaccine>(this.api.updateVaccine(), v);
  }

  getSchedulesByAnimal(animalId: string) {
    return this.http.getDataAsync<VaccineSchedule[]>(this.api.getVaccineSchedulesByAnimal(animalId));
  }

  getUpcoming(daysAhead = 7) {
    return this.http.getDataAsync<VaccineSchedule[]>(this.api.getVaccineUpcoming(daysAhead));
  }

  createSchedule(payload: CreateVaccineSchedule) {
    return this.http.postDataAsync<string>(this.api.createVaccineSchedule(), payload);
  }

  administer(payload: AdministerVaccine) {
    return this.http.postDataAsync<VaccineSchedule>(this.api.administerVaccine(), payload);
  }
}
