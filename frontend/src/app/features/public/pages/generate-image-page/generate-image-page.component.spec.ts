import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateImagePageComponent } from './generate-image-page.component';

describe('GenerateImagePageComponent', () => {
  let component: GenerateImagePageComponent;
  let fixture: ComponentFixture<GenerateImagePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenerateImagePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GenerateImagePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
