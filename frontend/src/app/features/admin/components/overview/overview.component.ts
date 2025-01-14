import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import {PhotoEndpointsService} from '../../services/Endpoints/Photo/photo-endpoints.service';
@Component({
  selector: 'app-overview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './overview.component.html',
  styleUrl: './overview.component.css'
})
export class OverviewComponent implements OnInit, OnDestroy {
  activeMenuItem: string = 'Overview'; // Podrazumevano aktivna stavka
  data:any;
  private ngOnDestroy$ = new Subject<void>();

  constructor(private PhotoService: PhotoEndpointsService){
  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.PhotoService.getOverviewData().pipe(takeUntil(this.ngOnDestroy$))
    .subscribe(data=> {
      console.log(data);
      this.data = data
    })
  }

  ngOnDestroy(): void {
    this.ngOnDestroy$.next();
    this.ngOnDestroy$.complete();
  }



  setActive(item: string) {
    this.activeMenuItem = item;
  }
}
