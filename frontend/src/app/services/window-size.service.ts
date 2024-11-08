import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent, Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WindowSizeService {
  private windowWidth: BehaviorSubject<number> | null = null;

  // Observable za pristup podacima o širini prozora
  data$: Observable<number>;

  constructor() {
    // Provjeravamo da li smo na klijentskoj strani
    if (typeof window !== 'undefined') {
      this.windowWidth = new BehaviorSubject<number>(window.innerWidth);
      this.data$ = this.windowWidth.asObservable();

      fromEvent(window, 'resize')
        .pipe(
          map(() => window.innerWidth),
          startWith(window.innerWidth)
        )
        .subscribe((width) => this.windowWidth?.next(width));
    } else {
      // Ako smo na serverskoj strani, postavljamo početnu vrijednost kao Observable sa defaultnom vrijednošću
      this.data$ = new BehaviorSubject<number>(0).asObservable();
    }
  }
}
