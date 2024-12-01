import { Component } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { FooterComponent } from "../../../shared/components/footer/footer.component";
import { ProfileComponent } from "../../components/profile/profile.component";

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [NavBarComponent, FooterComponent, ProfileComponent],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.css'
})
export class ProfilePageComponent {

}
