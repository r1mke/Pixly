import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule, RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent {
  public frmVerifyEmail: FormGroup;
  public verificationCodeControls: FormControl[];

  constructor(private fb: FormBuilder, private router: Router) {
    this.verificationCodeControls = Array(6).fill(null).map((_, i) =>
      new FormControl('', [Validators.required, Validators.pattern(/^\d$/)])
    );
    this.frmVerifyEmail = this.fb.group(
      Object.fromEntries(this.verificationCodeControls.map((control, i) => [`code${i}`, control]))
    );
  }

  submitVerificationCode() {
    if (this.frmVerifyEmail.valid) {
      const code = Object.values(this.frmVerifyEmail.value).join('');
      console.log('Uneseni kod:', code);
      this.router.navigate(['/login']);
    } else {
      console.log('Kod nije validan');
    }
  }

  moveToNextInput(event: any, index: number) {
    if (event.target.value && index < 5) {
      const nextInput = document.querySelector(`input[formControlName=code${index + 1}]`) as HTMLInputElement;
      nextInput?.focus();
    }
  }

  resendCode() {
    console.log('Ponovno slanje koda...');
  }
}
