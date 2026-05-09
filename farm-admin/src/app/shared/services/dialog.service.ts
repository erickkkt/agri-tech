import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DialogComponent } from '../components/dialog/dialog.component';
import { AccessDeniedComponent } from '../components/access-denied/access-denied.component';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { DialogData } from '../components/dialog/dialog-data.model';


@Injectable({
    providedIn: 'root'
})
export class DialogService {

    constructor(private dialog: MatDialog) {
    }

    public openSuccessDialog(data: DialogData) {
        data['type'] = 1; // success
        this.openDialog(data);
    }

    public openSuccessDialogConfirm(data: DialogData) : Observable<any> {
        data['type'] = 1; // success
        return this.openDialog(data).afterClosed();
    }

    public openErrorDialog(data: DialogData) {
        data['type'] = 2; // 2:error
        const dialogRef = this.openDialog(data);
        dialogRef.keydownEvents().subscribe(e => {
            if (e.keyCode === 13) {
                e.preventDefault();
                e.stopPropagation();
                setTimeout(() => {
                    dialogRef.close();
                }, 1000);
            }
        });
    }

    public openErrorDialogConfirm(data: DialogData) : Observable<any> {
        data['type'] = 2; // 2:error
        const dialogRef = this.openDialog(data);
        dialogRef.keydownEvents().subscribe(e => {
            if (e.keyCode === 13) {
                e.preventDefault();
                e.stopPropagation();
                setTimeout(() => {
                    dialogRef.close();
                }, 1000);
            }
        });
        return dialogRef.afterClosed();
    }

    public openConfirmationDialog(data: DialogData): Observable<any> {
        data['type'] = 3; // confirm
        data['yesOption'] = false;
        let dialogConfig = this.getDialogConfig();
        dialogConfig.data = data;
        let dialogRef = this.dialog.open(DialogComponent, dialogConfig);
        return dialogRef.afterClosed();
    }

    public getDialogConfig() {
        let config = new MatDialogConfig();
        config.disableClose = true;
        config.width = '600px';
        return config;
    }

    public openComponentDialog(componentOrTemplateRef: any, data?: Object, config?: MatDialogConfig): Observable<any> {
        let dialogConfig = Object.assign({}, this.getDialogConfig(), config);
        if (data) {
            dialogConfig.data = data;
        }
        let dialogRef = this.dialog.open(componentOrTemplateRef, dialogConfig);
        return dialogRef.afterClosed();
    }

    openDialog(data: DialogData) {
        let dialogConfig = this.getDialogConfig();
        dialogConfig.disableClose = data.disableClose || true;
        dialogConfig.autoFocus = true;
        dialogConfig.data = data;
        let dialogRef = this.dialog.open(DialogComponent, dialogConfig);
        return dialogRef;
    }

    openAccessDeniedDialog(){
        let config = new MatDialogConfig();
        config.disableClose = true;
        config.width = '100%';
        config.height='100%';
        config.autoFocus= true;
        let dialogRef = this.dialog.open(AccessDeniedComponent, config);
        return dialogRef;
    }
}

