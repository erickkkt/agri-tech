import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { AnimalDataSource } from '../../models/animal.datasource';
import { NO_RESULTS_FOUND, PagingConstant } from '../../shared/constants/constants';
import { AnimalService } from '../../services/animal.service';
import { DialogService } from '../../shared/services/dialog.service';
import { Animal } from '../../models/animal.model';
import { AnimalPopupComponent } from './animal-popup/animal-popup.component';

@Component({
  selector: 'app-animal-management',
  standalone: false,
  templateUrl: './animal-management.component.html',
  styleUrl: './animal-management.component.css'
})
export class AnimalManagementComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = ['code', 'name', 'cageName', 'farmName', 'weight', 'height', 'healthStatus', 'isActive', 'actions'];

  dataSource!: AnimalDataSource;
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
    private readonly _AnimalService: AnimalService,
    private readonly _dialogService: DialogService) {
    this.configDialog.disableClose = true;
    this.configDialog.width = '1200px';
  }

  async ngOnInit() {
    this.dataSource = new AnimalDataSource(this._AnimalService);
    await this.loadData();
    this.dataSource.animalsSubject$.subscribe(() => {
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

    this.dataSource.getAnimals(sortField, sortDirection, index, pageSize);
  }

  ngAfterViewInit() {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
    });

    this.paginator.page
      .pipe(
        tap(() => this.getAnimalPage())
      )
      .subscribe();

    this.sort.sortChange
      .pipe(
        tap(() => this.getAnimalPage())
      )
      .subscribe();
  }

  getAnimalPage() {
    const fieldSort = this.sort && this.sort.active
      ? this.sort.active.charAt(0).toUpperCase() + this.sort.active.slice(1)
      : 'Name';
    const direction = this.sort && this.sort.direction ? this.sort.direction : 'asc';
    this.loadData(fieldSort, direction);
  }

  editAnimal(animal: Animal) {
    this._dialogService.openComponentDialog(AnimalPopupComponent, { animal: animal }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }

  createAnimal() {
    this._dialogService.openComponentDialog(AnimalPopupComponent, {
    }, this.configDialog).subscribe(async (result: any) => {
      if (result) {
        await this.loadData();
      }
    });
  }
}
