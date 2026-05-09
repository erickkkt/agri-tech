import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { CageDataSource } from '../../models/cage.datasource';
import { NO_RESULTS_FOUND, PagingConstant } from '../../shared/constants/constants';
import { CageService } from '../../services/cage.service';
import { DialogService } from '../../shared/services/dialog.service';
import { Cage } from '../../models/cage.model';
import { CagePopupComponent } from './cage-popup/cage-popup.component';

@Component({
  selector: 'app-cage-management',
  standalone: false,
  templateUrl: './cage-management.component.html',
  styleUrl: './cage-management.component.css'
})
export class CageManagementComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['name', 'farmName', 'capacity', 'currentAnimalCount', 'isActive', 'actions'];

  dataSource!: CageDataSource;
  totalRecords: number = 0;

  configDialog = new MatDialogConfig();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  pagingConstant = PagingConstant;

  notfound: string = '';

  get notFoundMessage() {
    return this.notfound;
  }

  constructor(
    private readonly _CageService: CageService,
    private readonly _dialogService: DialogService) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '1200px';
  }

  async ngOnInit() {
    this.dataSource = new CageDataSource(this._CageService);
    await this.loadData();
    this.dataSource.cagesSubject$.subscribe(() => {
      this.checkNotFoundResults();
    });
  }

  checkNotFoundResults() {
    if (this.dataSource && this.dataSource.totalRecords === 0) {
      this.notfound = NO_RESULTS_FOUND;
    } else {
      this.notfound = '';
    }
  }

  async loadData(sortField: string = 'Name', sortDirection: string = 'asc') {
    let index = this.paginator ? this.paginator.pageIndex : 0;
    let pageSize = this.paginator ? this.paginator.pageSize : PagingConstant.pageSize;

    this.dataSource.getCages(sortField, sortDirection, index, pageSize);
  }

  ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
    });

    this.paginator.page
      .pipe(
        tap(() => this.getCagePage())
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.getCagePage())
      )
      .subscribe();
  }

  getCagePage() {
    const fieldSort = this.sort && this.sort.active
      ? this.sort.active.charAt(0).toUpperCase() + this.sort.active.slice(1)
      : 'Name';
    const direction = this.sort && this.sort.direction ? this.sort.direction : 'asc';
    this.loadData(fieldSort, direction);
  }

  editCage(cage: Cage) {
    this._dialogService.openComponentDialog(CagePopupComponent, { cage: cage }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }

  createCage() {
    this._dialogService.openComponentDialog(CagePopupComponent, {
    }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }
}
