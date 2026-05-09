import { Component } from '@angular/core';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.scss'],
    standalone: false,
})
/** footer component*/
export class FooterComponent {
    /** footer ctor */
    public currentYear: number = new Date().getFullYear();

    constructor() {

    }
}