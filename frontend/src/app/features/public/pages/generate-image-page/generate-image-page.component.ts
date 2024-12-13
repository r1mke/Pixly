import { Component,HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OpenaiService } from '../../services/openai.service';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
@Component({
  selector: 'app-generate-image-page',
  standalone: true,
  imports: [CommonModule, FormsModule, NavBarComponent],
  templateUrl: './generate-image-page.component.html',
  styleUrl: './generate-image-page.component.css'
})
export class GenerateImagePageComponent {
    input: string = '';
    loading:boolean = true;
    selectedQuantity: number = 1;
    numberArray: number[] = [];
    images: string[] = [];
    clicked: boolean = true;
    dropDown: boolean = false;
    constructor(private openaiService: OpenaiService) { }


    updateNumberArray() {
      this.numberArray = Array.from({ length: this.selectedQuantity }, (_, index) => index + 1);
      console.log('Number array updated:', this.numberArray);
    }

    toggleDropdown() {
      this.dropDown = !this.dropDown;
      console.log(this.dropDown);
    } 

    updatedSelectedQuantity(quantity: number) {
      this.selectedQuantity = quantity;
    }

  generateImage(){
    if(this.input==='') {
      alert('Please enter a prompt');
      return;
    }
    this.images = [];

    this.updateNumberArray();
    this.loading = true;
    this.openaiService.generateImage(this.input, this.selectedQuantity).subscribe((response) => {
      console.log(response);
      this.loading = false;
      this.images = response.data.map((image: any) => image.url);
      this.loading = false;
      this.input = '';
      this.selectedQuantity = 1;
    },
    (error) => {
      console.error('Error generating image:', error);
      this.loading = false;
    });

  }

  @HostListener('document:click', ['$event'])
    onDocumentClick(event: MouseEvent): void {
      const target = event.target as HTMLElement;
    
      // Proveri da li je klik izvan dropdowna
      if (!target.closest('.dropdown') && !target.closest('.dropdown-explore')) {
        this.dropDown = false;
      }
    }

  

  // openInNewTab() {
  //   if (this.imageUrl) {
  //     window.open(this.imageUrl, '_blank');
  //   }
  // }

  // downloadImage() {
  //   if (this.imageUrl) {
  //     const link = document.createElement('a');
  //     link.href = this.imageUrl;
  //     link.setAttribute('download', 'generated-image.png'); 
  //     link.target = '_self'; 
  //     link.click();
  //   }
  // }


}
