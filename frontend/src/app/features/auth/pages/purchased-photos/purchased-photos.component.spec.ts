import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchasedPhotosComponent } from './purchased-photos.component';

describe('PurchasedPhotosComponent', () => {
  let component: PurchasedPhotosComponent;
  let fixture: ComponentFixture<PurchasedPhotosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchasedPhotosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurchasedPhotosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
