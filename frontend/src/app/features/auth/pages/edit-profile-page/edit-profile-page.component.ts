import { Component } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { EditProfileComponent } from "../../components/edit-profile/edit-profile.component";

@Component({
  selector: 'app-edit-profile-page',
  standalone: true,
  imports: [NavBarComponent, EditProfileComponent],
  templateUrl: './edit-profile-page.component.html',
  styleUrl: './edit-profile-page.component.css'
})
export class EditProfilePageComponent {

}
