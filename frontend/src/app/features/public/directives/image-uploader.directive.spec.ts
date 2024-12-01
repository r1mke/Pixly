import { ImageUploaderDirective } from './image-uploader.directive';
import { DomSanitizer } from '@angular/platform-browser';
import { TestBed } from '@angular/core/testing';

describe('ImageUploaderDirective', () => {
  let directive: ImageUploaderDirective;
  let sanitizer: DomSanitizer;

  beforeEach(() => {
    // Kreiranje TestBed okruženja i injektovanje zavisnosti
    TestBed.configureTestingModule({
      providers: [DomSanitizer]  // Osiguravamo da je sanitizer dostupan
    });

    // Injektovanje zavisnosti
    sanitizer = TestBed.inject(DomSanitizer);
    directive = new ImageUploaderDirective(sanitizer);
  });

  it('should create an instance', () => {
    expect(directive).toBeTruthy();
  });
});
