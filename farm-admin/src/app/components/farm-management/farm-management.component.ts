import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FarmDataSource } from '../../models/farm.datasource';
import { NO_RESULTS_FOUND, PagingConstant } from '../../shared/constants/constants';
import { FarmService } from '../../services/farm.service';
import { DialogService } from '../../shared/services/dialog.service';
import { Farm } from '../../models/farm.model';
import { FarmPopupComponent } from './farm-popup/farm-popup.component';


@Component({
  selector: 'app-farm-management',
  standalone: false,
  templateUrl: './farm-management.component.html',
  styleUrl: './farm-management.component.css'
})

export class FarmManagementComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['name', 'ownerName', 'description', 'location', 'isActive', 'actions'];

  dataSource!: FarmDataSource;
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
    private readonly _FarmService: FarmService,
    private readonly _dialogService: DialogService) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '800px';
  }

  async ngOnInit() {
    this.dataSource = new FarmDataSource(this._FarmService);
    await this.loadData();
    this.dataSource.farmsSubject$.subscribe(() => {
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

    this.dataSource.getFarms(sortField, sortDirection, index, pageSize);
  }

  ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
    });

    this.paginator.page
      .pipe(
        tap(() => this.getFarmPage())
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.getFarmPage())
      )
      .subscribe();
  }

  getFarmPage() {
    const fieldSort = this.sort && this.sort.active
      ? this.sort.active.charAt(0).toUpperCase() + this.sort.active.slice(1)
      : 'Name';
    const direction = this.sort && this.sort.direction ? this.sort.direction : 'asc';
    this.loadData(fieldSort, direction);
  }

  editFarm(farm: Farm) {
    this._dialogService.openComponentDialog(FarmPopupComponent, { farm: farm }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }

  createFarm() {
    this._dialogService.openComponentDialog(FarmPopupComponent, {
    }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }
}

