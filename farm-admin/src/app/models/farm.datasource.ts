import { DataSource } from '@angular/cdk/table';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { CollectionViewer } from '@angular/cdk/collections';
import { Farm } from './farm.model';
import { FarmService } from '../services/farm.service';
import { PaginationResponse } from './pagination-response.model';

export class FarmDataSource implements DataSource<Farm> {

    private farmsSubject = new BehaviorSubject<Farm[]>([]);
    public totalRecords: number | undefined;

    public farmsSubject$ = this.farmsSubject.asObservable();

    constructor(private _farmService: FarmService) { }

    connect(collectionViewer: CollectionViewer): Observable<Farm[]> {
        return this.farmsSubject.asObservable();
    }

    disconnect(collectionViewer: CollectionViewer): void {
        this.farmsSubject.complete();
    }

    getFarms(sortField = '', sortDirection = 'asc', pageIndex = 0, pageSize = 10) {
        this._farmService.getFarms(sortField, sortDirection, pageIndex, pageSize)
            .then((result: PaginationResponse<Farm> | undefined) => {
                if (result) {
                    this.totalRecords = result.total;
                    this.farmsSubject.next(result.items);
                }
            });
    }
}
