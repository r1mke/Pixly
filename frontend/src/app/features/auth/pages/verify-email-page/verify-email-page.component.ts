import { Component } from '@angular/core';
import { VerifyEmailComponent } from "../../components/verify-email/verify-email.component";

@Component({
  selector: 'app-verify-email-page',
  standalone: true,
  imports: [VerifyEmailComponent],
  templateUrl: './verify-email-page.component.html',
  styleUrl: './verify-email-page.component.css'
})
export class VerifyEmailPageComponent {

}
