import { Component, OnInit, ViewChild } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImageFile } from '../../model/image-file';
import { ImageUploaderDirective } from '../../directives/image-uploader.directive';
import { PhotoPostService } from '../../services/Photos/photo-post.service';
import { PostPhoto } from '../../model/PostPhoto';
import { AuthService } from '../../../auth/services/auth.service';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { inject } from '@angular/core';
import {RouterModule} from  '@angular/router';
import { NgbdToast } from "../../../shared/components/toast/toast.component";



@Component({
  selector: 'app-upload-page',
  standalone: true,
  imports: [NavBarComponent, CommonModule, RouterModule, FormsModule, ImageUploaderDirective, ReactiveFormsModule, NgbdToast],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.css'
})
export class UploadPageComponent implements OnInit {

  //--PostPhotoRequest--//
  title: string = '';
  description: string = '';
  location: string = '';
  userId: number | null = null;
  file: ImageFile | null = null;  
  tags: string[] = [];
  //-----------------------//

  //--atributes--//
  user:any;
  image: boolean = false;
  tagInput: string = ''; 
  //-----------------------//
  isLoading: boolean = false;
  @ViewChild(NgbdToast)
  ngbdToast!: NgbdToast;
  
  uploadForm: FormGroup;

  constructor(
    private photoPostService: PhotoPostService,
    private authService: AuthService, private formBuilder: FormBuilder) {


      this.uploadForm = this.formBuilder.group({
        title: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(5)]],
        description: ['', [Validators.required, Validators.maxLength(200), Validators.minLength(5)]],
        location: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(3)]],
        tags: ['', Validators.required],
        price: [null, [Validators.required, Validators.min(0), Validators.max(50)]]
      });

      this.uploadForm.get('tags')?.valueChanges.subscribe(value => {
        this.tagInput = value;
        console.log(this.tagInput);
      });

    }
     
    

    ngOnInit(): void {
      this.authService.currentUser$.subscribe({
        next:(res) =>{
          this.user = res;
        },
        error:(error) => {
          console.error('Error fetching user:', error);
        }
      });


    }

    capitalizeFirstLetter(text: string): string {
      if (!text) return ""; 
      return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase();
  }
    

  //-----------------------//


    //--tag selection--//
    addTag() {

      if (this.tagInput.trim() === '') {
        return;
      }
    
      if (!this.tags.includes(this.tagInput.trim())) {
        this.tags.push(this.capitalizeFirstLetter(this.tagInput.trim()));
        this.uploadForm.get('tags')?.setValue(''); 
      }

    }
  
    removeTag(tag: string) {
      this.tags = this.tags.filter(t => t !== tag);
    }
  //-----------------------//



    onDropFiles(file: ImageFile): void {
      this.file = file; 
      this.image = true; 
    }

    onFileSelected(event: any): void {
      const file = event.target.files[0];
      if (file) {
        this.file = { file: file, url: URL.createObjectURL(file) };
        this.image = true;
      }
      else{
        this.image = false;
      }
    }

    removeImage(): void {
      this.file = null;
      this.tags = [];
      this.uploadForm.reset();
      this.uploadForm.get('categories')?.setValue('');
      this.image = false;
    }

    onSubmit():void {
      this.isLoading = true;

      if (!this.file || !this.file.file) {
        alert('Niste odabrali fajl!');
        return;
      }

      const title = this.uploadForm.get('title')?.value;
      const description = this.uploadForm.get('description')?.value;
      const location = this.uploadForm.get('location')?.value;
      const price = this.uploadForm.get('price')?.value;

      const formData = new FormData();
      const tagsString = this.tags.join(",");
     formData.append('Title', this.capitalizeFirstLetter(title));
     formData.append('Description', this.capitalizeFirstLetter(description));
     formData.append('Location', this.capitalizeFirstLetter(location));
     formData.append('UserId', this.user.userId.toString());
     formData.append('File', this.file.file);
     formData.append('Tags', tagsString);
     formData.append('Price', price)
     console.log(this.tags);
     console.log(JSON.stringify(this.tags));
    this.photoPostService.postPhoto(formData).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.ngbdToast.showMessage('Successfully uploaded!', 'success');
        this.removeImage();
      },
      error: (err) => {
        this.isLoading = false;
      }
    }); 
    }

  }


    

