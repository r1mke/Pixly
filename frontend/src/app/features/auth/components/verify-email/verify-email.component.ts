import { Component, OnInit, ViewChildren, QueryList, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';
import { VerifyEmailService } from '../../services/verify-email.service';
import { ResendVerificationCodeService } from '../../services/resend-verififcation-code.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule, RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent implements OnInit {
  public frmVerifyEmail: FormGroup;
  public verificationCodeControls: FormControl[] = [];
  public userEmail: string | null = null;
  public errorMessage: string = ''; 
  public successMessage: string = '';
  public resendErrorMessage: string = '';
  public resendSuccessMessage: string = '';

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private verifyEmailService = inject(VerifyEmailService);
  private resendVerificationCodeService = inject(ResendVerificationCodeService);

  @ViewChildren('inputField') inputFields!: QueryList<ElementRef>;
  
  constructor() {
    this.verificationCodeControls = Array(6).fill(null).map(() => new FormControl('', [
      Validators.required,
      Validators.pattern(/^\d$/)
    ]));

    const formControls = this.verificationCodeControls.reduce((acc, control, i) => {
      acc[`code${i}`] = control;
      return acc;
    }, {} as Record<string, FormControl>);

    this.frmVerifyEmail = this.fb.group(formControls);
  }

  ngOnInit(): void {
    const token = localStorage.getItem('jwtToken');
    console.log('JWT Token:', token);
    if (token) {
      const decodedToken: any = jwtDecode(token);
      this.userEmail = decodedToken.email;
      console.log('Decoded email:', this.userEmail);
    }
  }
  
  submitVerificationCode() {
    this.resetMessages();

    if (!this.frmVerifyEmail.valid || !this.userEmail) {
      this.errorMessage = !this.frmVerifyEmail.valid ? 'Kod nije validan.' : 'Email nije pronađen.';
      return;
    }

    const code = Object.values(this.frmVerifyEmail.value).join('');
    console.log('Uneseni kod:', code, 'Poslani email:', this.userEmail);
  
    this.verifyEmailService.verifyEmail(code, this.userEmail).subscribe({
      next: (response) => {
        console.log('Verifikacija uspješna:', response);
        localStorage.removeItem('jwtToken');
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Greška pri verifikaciji:', error);
        this.errorMessage = error.message || 'Došlo je do greške pri verifikaciji.';
      },
      complete: () => {
        console.log('Verifikacija završena.');
      }
    });
  }
  
  moveToNextInput(event: any, index: number) {
    if (event.target.value && index < 5) {
      const nextInput = this.inputFields.toArray()[index + 1]?.nativeElement;
      nextInput?.focus();
    }
  }

  resendCode() {
    this.resetMessages();
    
    if (this.userEmail) {
      this.resendVerificationCodeService.resendVerificationCode(this.userEmail).subscribe({
        next: (response) => {
          console.log('Ponovno slanje koda uspješno:', response);
          this.resendSuccessMessage = 'New code has beent sent.';
          this.resendErrorMessage = '';
          this.errorMessage = '';
        },
        error: (error) => {
          console.error('Greška pri ponovnom slanju koda:', error);
          this.resendErrorMessage = error.message || 'Došlo je do greške pri ponovnom slanju koda.';
          this.resendSuccessMessage = '';
        },
        complete: () => {
          console.log('Ponovno slanje koda završeno.');
        }
      });
    } else {
      this.resendErrorMessage = 'Email nije pronađen.';
    }
  }

  resetMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.resendErrorMessage = '';
    this.resendSuccessMessage = '';
  }
  
}
