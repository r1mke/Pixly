import { Component } from '@angular/core';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'ngbd-toast',
	standalone: true,
	imports: [NgbToastModule, CommonModule],
	templateUrl: './toast.component.html',
})
export class NgbdToast {
	showToast = false;
  toastMessage: string = '';
	toastType: string = ''; 

	showMessage(message: string, type: string) {
    this.toastMessage = message;
    this.toastType = type;
    this.showToast = true;

		setTimeout(() => {
      this.showToast = false;
    }, 2000);
  }

  closeToast(): void {
    this.showToast = false;
  }
}
