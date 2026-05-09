export interface DialogData {
  title?: string;
  message?: string;
  type?: number;
  disableClose?: boolean;
  yesOption?: boolean;
  [key: string]: any; 
}