import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifyTwofaPageComponent } from './verify-twofa-page.component';

describe('VerifyTwofaPageComponent', () => {
  let component: VerifyTwofaPageComponent;
  let fixture: ComponentFixture<VerifyTwofaPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VerifyTwofaPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VerifyTwofaPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
