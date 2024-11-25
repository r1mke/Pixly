import { Component, OnInit, ViewChildren, QueryList, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterLink } from '@angular/router';
import { VerifyEmailService } from '../../services/verify-email.service';
import { ResendVerificationCodeService } from '../../services/resend-verififcation-code.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgbModule],
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
  public isLoading: boolean = false;
  public user: any = {};

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private verifyEmailService = inject(VerifyEmailService);
  private resendVerificationCodeService = inject(ResendVerificationCodeService);

  @ViewChildren('inputField') inputFields!: QueryList<ElementRef>;
  
  constructor(private authService: AuthService) {
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
    this.isLoading = true; // Postavite indikator na početku.
    this.authService.getCurrentUser().subscribe({
      next: (response) => {
        if (response) {
          this.user = response.user; // Dohvatite podatke o korisniku.
          this.userEmail = response.user?.email;
        } else {
          this.errorMessage = 'Greška prilikom dohvaćanja podataka korisnika.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Greška:', err);
        this.errorMessage = 'Greška prilikom dohvaćanja podataka korisnika.';
        this.isLoading = false;
      },
    });
  }
  
  
  submitVerificationCode() {
    this.resetMessages();
    this.isLoading = true;
  
    if (!this.frmVerifyEmail.valid || !this.userEmail) {
      this.errorMessage = !this.frmVerifyEmail.valid ? 'Kod nije validan.' : 'Email nije pronađen.';
      this.isLoading = false;
      return;
    }
    const code = Object.values(this.frmVerifyEmail.value).join('');
    this.verifyEmailService.verifyEmail(code, this.userEmail).subscribe({
      next: (response) => {
        this.router.navigate(['/login']);
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
    
    if (this.userEmail) {
      this.resendVerificationCodeService.resendVerificationCode().subscribe({
        next: (response) => {
          console.log('Ponovno slanje koda uspješno:', response);
          this.resendSuccessMessage = 'New code has been sent.';
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
