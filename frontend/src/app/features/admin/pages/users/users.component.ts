import { Component } from '@angular/core';
import { DisplayUsersComponent } from "../../../shared/components/display-users/display-users.component";

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [DisplayUsersComponent],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent {

}
