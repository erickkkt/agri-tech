import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CollectionViewer } from '@angular/cdk/collections';
import { Cage } from './cage.model';
import { CageService } from '../services/cage.service';
import { PaginationResponse } from './pagination-response.model';

export class CageDataSource implements DataSource<Cage> {

    private cagesSubject = new BehaviorSubject<Cage[]>([]);
    public totalRecords: number | undefined;

    public cagesSubject$ = this.cagesSubject.asObservable();

    constructor(private _cageService: CageService) { }

    connect(collectionViewer: CollectionViewer): Observable<Cage[]> {
        return this.cagesSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.cagesSubject.complete();
    }

    getCages(sortField = '', sortDirection = 'asc', pageIndex = 0, pageSize = 10) {
        this._cageService.getCages(sortField, sortDirection, pageIndex, pageSize)
            .then((result: PaginationResponse<Cage> | undefined) => {
                if (result) {
                    this.totalRecords = result.total;
                    this.cagesSubject.next(result.items);
                }
            });
    }
}
