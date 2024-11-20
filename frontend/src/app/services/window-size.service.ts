import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent, Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { debounceTime } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class WindowSizeService {
  private windowWidth = new BehaviorSubject<number>(typeof window !== 'undefined' ? window.innerWidth : 0);
  data$: Observable<number> = this.windowWidth.asObservable();

  constructor() {
    // Provjeravamo da li smo na klijentskoj strani
    if (typeof window !== 'undefined') {
      this.windowWidth = new BehaviorSubject<number>(window.innerWidth);
      this.data$ = this.windowWidth.asObservable();

      fromEvent(window, 'resize')
        .pipe(
          map(() => window.innerWidth),
          startWith(window.innerWidth),
          debounceTime(200)
        )
        .subscribe((width) => this.windowWidth?.next(width));
    } else {
      // Ako smo na serverskoj strani, postavljamo početnu vrijednost kao Observable sa defaultnom vrijednošću
      this.data$ = new BehaviorSubject<number>(0).asObservable();
    }
  }
}
