import { Component, OnInit } from '@angular/core';
import { HostListener } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-error',
  standalone: true,
  imports: [],
  templateUrl: './error.component.html',
  styleUrl: './error.component.css'
})
export class ErrorComponent implements OnInit {
  isScreenTooSmall: any;
  windowWidth :any;
  constructor(private router : Router){}

  ngOnInit(): void {
    this.checkScreenSize();
  }

  @HostListener('window:resize', [])
    onResize() {
      this.checkScreenSize();
    }
  
    checkScreenSize() {
      this.isScreenTooSmall = window.innerWidth > 1000;
      this.windowWidth = window.innerWidth;
      if(this.isScreenTooSmall){
        this.router.navigate(['admin/dashboard']);
      }
    }
}
