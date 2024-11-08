import { Component } from '@angular/core';
import { RegisterComponent } from "../../components/register/register.component";
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [NavBarComponent, RegisterComponent],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.css'
})
export class RegisterPageComponent {

}
