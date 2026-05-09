import { Component } from '@angular/core';
import { NavService } from '../side-nav/nav.service';

@Component({
    selector: 'app-navigation',
    templateUrl: './navigation.component.html',
    styleUrls: ['./navigation.component.scss'],
    standalone: false,
})

export class NavigationComponent  {
    constructor(private navService: NavService) { }

    public closeNav() {
        this.navService.closeNav();
      }
}
