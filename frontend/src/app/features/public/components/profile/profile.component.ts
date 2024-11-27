import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Importuje CommonModule za ngFor

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule], // Dodaj CommonModule ovde
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'] // Ispravi typo: "styleUrl" u "styleUrls"
})
export class ProfileComponent {
  navItems = [
    { label: 'Highlights', count: 0, active: true },
    { label: 'Gallery', count: 0, active: false },
    { label: 'Collections', count: null, active: false },
    { label: 'Statistics', count: null, active: false },
    { label: 'Followers', count: 0, active: false },
    { label: 'Following', count: 0, active: false },
  ];

  setActive(item: any, event: Event) {
    event.preventDefault(); // Sprečava prelazak na drugu stranicu
    this.navItems.forEach((nav) => (nav.active = false)); // Deaktivira sve stavke
    item.active = true; // Postavlja trenutni item kao aktivan
  }
  
}
