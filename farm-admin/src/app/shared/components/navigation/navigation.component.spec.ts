// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { NavigationComponent } from './navigation.component';

let component: NavigationComponent;
let fixture: ComponentFixture<NavigationComponent>;

describe('navigation component', () => {
    beforeEach(async() => {
        TestBed.configureTestingModule({
            declarations: [ NavigationComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(NavigationComponent);
        component = fixture.componentInstance;
    });

    it('should do something', async() => {
        expect(true).toEqual(true);
    });
});