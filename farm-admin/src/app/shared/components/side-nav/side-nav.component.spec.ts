// / <reference path="../../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { SideNavComponent } from './side-nav.component';

let component: SideNavComponent;
let fixture: ComponentFixture<SideNavComponent>;

describe('side-nav component', () => {
    beforeEach(async () => {
        TestBed.configureTestingModule({
            declarations: [ SideNavComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(SideNavComponent);
        component = fixture.componentInstance;
    });

    it('should do something', async() => {
        expect(true).toEqual(true);
    });
});