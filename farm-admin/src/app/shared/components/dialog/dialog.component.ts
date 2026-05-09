import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export const DialogText = {
  successTitle: 'Confirmation Message',
  errorTitle: 'Error Message',
  yesButton: 'Yes',
  noButton: 'No',
  cancelButton: 'Cancel',
  closeButton: 'Close',
  saveButton: 'Save',
  createMessage: 'The content has been created successfully',
  updateMessage: 'The content has been update successfully',
  confirmMessage: 'Are you sure you want to remove {0}?'
}

@Component({
  selector: 'app-dialog',
  templateUrl: './dialog.component.html',
  styleUrls: ['./dialog.component.css'],
  standalone: false,
})
export class DialogComponent implements OnInit {
  public title: string = DialogText.successTitle;
  public message: string = "";
  public primaryButtons = DialogText.closeButton;
  public secondaryButtons = DialogText.cancelButton;
  public titleType = [DialogText.successTitle, DialogText.errorTitle, DialogText.successTitle]; //0: success, 1:error
  public yesOption = false;
  get dependenciesFailed() {
    return this.data["dependenciesFailed"] === true;
  }
  get htmlContent() {
    const html = this.data["htmlContent"];
    return html ? html : '';
  }
  constructor(public dialogRef: MatDialogRef<DialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }

  ngOnInit() {
    let dialogType = this.data["type"] || null;
    this.generateDialog(dialogType);
  }

  generateDialog(type: number) {
    this.title = this.data["title"];
    this.message = this.data["message"] || this.message;
    switch (type - 1) {
      case (0): //0: success dialog
        this.primaryButtons = this.data["primaryButtons"] || DialogText.closeButton;
        this.secondaryButtons = this.data["secondaryButtons"] || null;
        break;
      case (1): //error dialog
        this.primaryButtons = this.data["primaryButtons"] || DialogText.closeButton;
        this.secondaryButtons = this.data["secondaryButtons"] || null;
        break;
      case (2):  //confirm dialog
        this.primaryButtons = this.data["primaryButtons"] || DialogText.yesButton;
        this.secondaryButtons = this.data["secondaryButtons"] || DialogText.noButton;
        break;
      default:
        break;
    }
  }

  isConfirmDialog() {
    return this.data["type"] === 3;
  }

  onPrimaryButton() {
    this.yesOption = true;
    this.dialogRef.close(this.yesOption);
  }

  onSecondaryButton() {
    this.yesOption = false;
    this.dialogRef.close(this.yesOption);
  }

  onCloseDialog() {
    this.dialogRef.close();
  }
}
