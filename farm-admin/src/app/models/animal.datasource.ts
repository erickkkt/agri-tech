import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CollectionViewer } from '@angular/cdk/collections';
import { Animal } from './animal.model';
import { AnimalService } from '../services/animal.service';
import { PaginationResponse } from './pagination-response.model';

export class AnimalDataSource implements DataSource<Animal> {

    private animalsSubject = new BehaviorSubject<Animal[]>([]);
    public totalRecords: number | undefined;

    public animalsSubject$ = this.animalsSubject.asObservable();

    constructor(private _animalService: AnimalService) { }

    connect(collectionViewer: CollectionViewer): Observable<Animal[]> {
        return this.animalsSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.animalsSubject.complete();
    }

    getAnimals(sortField = '', sortDirection = 'asc', pageIndex = 0, pageSize = 10) {
        this._animalService.getAnimals(sortField, sortDirection, pageIndex, pageSize)
            .then((result: PaginationResponse<Animal> | undefined) => {
                if (result) {
                    this.totalRecords = result.total;
                    this.animalsSubject.next(result.items);
                }
            });
    }
}
