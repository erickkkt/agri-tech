import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-error-handler',
  templateUrl: './error-handler.component.html',
  styleUrls: ['./error-handler.component.scss'],
  standalone: false,
})
export class ErrorHandlerComponent  {
  errorCode: string = '500';
  subMessage1='Sorry….. It’s not you. It’s us.';
  subMessage2='Something went wrong.';
  message: string = 'This has been automatically notified to our Application Support Team.';

  constructor(private readonly _router: Router) {
  }

  goBack(){
    this._router.navigate(['/app'])
  }
}
