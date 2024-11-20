import { AfterViewInit, Component, OnInit} from '@angular/core';
import { CommonModule} from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WindowSizeService } from '../../../../services/window-size.service';
import { Renderer2,ElementRef } from '@angular/core';
@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent  implements OnInit, AfterViewInit{
  isScrolled: boolean = false;
  windowWidth: number = 0;
  user: boolean = true;
  menuOpen: boolean = false;  

  constructor(private windowSizeService: WindowSizeService,private router: Router, private renderer: Renderer2) {
    this.windowSizeService.data$.subscribe((w)=> {
      this.windowWidth = w;

      this.menuOpen = this.windowWidth >= 900;
      console.log(this.menuOpen, this.windowWidth);

    })
  }

  ngOnInit(): void {
    this.windowSizeService.data$.subscribe((w)=> {
      this.windowWidth = w;

      this.menuOpen = this.windowWidth >= 900;
      console.log(this.menuOpen, this.windowWidth);

    })
    }

  ngAfterViewInit() {

  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  goToHome() {
    this.router.navigate(['/']);
  }
 

}