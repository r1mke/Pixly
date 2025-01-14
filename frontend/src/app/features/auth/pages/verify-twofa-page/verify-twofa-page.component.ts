import { Component, OnInit, ViewChildren, QueryList, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-verify-twofa-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule],
  templateUrl: './verify-twofa-page.component.html',
  styleUrl: './verify-twofa-page.component.css'
})
export class VerifyTwofaPageComponent {
  public frmVerify2FA: FormGroup;
  public verificationCodeControls: FormControl[] = [];
  public userEmail: string | null = null;
  public errorMessage: string = ''; 
  public successMessage: string = '';
  public resendErrorMessage: string = '';
  public resendSuccessMessage: string = '';
  public isLoading: boolean = false;
  public user: any = {};

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private loginService = inject(LoginService);

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

    this.frmVerify2FA = this.fb.group(formControls);
  }

  ngOnInit(): void {
    this.userEmail = sessionStorage.getItem('email');
  }
  
  
  submitVerificationCode(): void {
    this.resetMessages();
    this.isLoading = true;
    
    if (!this.frmVerify2FA.valid) {
      this.errorMessage = !this.frmVerify2FA.valid ? 'Kod nije validan.' : 'Email nije pronađen.';
      this.isLoading = false;
      return;
    }
  
    const code = Object.values(this.frmVerify2FA.value).join('');
    this.loginService.verify2FA(code).subscribe({
      next: (response) => {
        console.log(response);
        sessionStorage.removeItem('statusCode');
        this.router.navigate(['/home']);
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Greška pri verifikaciji:', error);
        this.errorMessage = error.message || 'Došlo je do greške pri verifikaciji.';
      },
      complete: () => {
        this.isLoading = false;
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
    this.isLoading = true;
    
      this.loginService.resend2FA().subscribe({
        next: (response) => {
          //console.log('Ponovno slanje koda uspješno:', response);
          this.resendSuccessMessage = 'New code has been sent.';
          this.resendErrorMessage = '';
          this.errorMessage = '';
        },
        error: (error) => {
          //console.error('Greška pri ponovnom slanju koda:', error);
          this.resendErrorMessage = error.message || 'Došlo je do greške pri ponovnom slanju koda.';
          this.resendSuccessMessage = '';
        },
        complete: () => {
          this.isLoading = false;
          //console.log('Ponovno slanje koda završeno.');
        }
      });
  }

  resetMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.resendErrorMessage = '';
    this.resendSuccessMessage = '';
  }
}
