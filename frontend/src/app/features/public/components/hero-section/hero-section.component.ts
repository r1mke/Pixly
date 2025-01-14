import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {  RouterLink } from '@angular/router';
import { Router } from '@angular/router';
@Component({
  selector: 'app-hero-section',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './hero-section.component.html',
  styleUrl: './hero-section.component.css'
})
export class HeroSectionComponent  {
  constructor(private router: Router){}
  
}
