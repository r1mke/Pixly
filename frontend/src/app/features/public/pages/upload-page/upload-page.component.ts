import { Component, OnInit } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImageFile } from '../../model/image-file';
import { ImageUploaderDirective } from '../../directives/image-uploader.directive';
import { GetAllCategoriesService } from '../../services/get-all-categories.service';
import {Category} from '../../model/category'
import { PhotoPostService } from '../../services/photo-post.service';
import { PostPhoto } from '../../model/PostPhoto';
import { AuthService } from '../../../auth/services/auth.service';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { inject } from '@angular/core';
import {RouterModule} from  '@angular/router';



@Component({
  selector: 'app-upload-page',
  standalone: true,
  imports: [NavBarComponent, CommonModule,RouterModule, FormsModule, ImageUploaderDirective, ReactiveFormsModule],
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
  categories: Category[] = [];
  //-----------------------//

  //--atributes--//
  user:any;
  image: boolean = false;
  selectArray: Category[] = [];
  tagInput: string = ''; 
  //-----------------------//
  isLoading: boolean = false;
  
  uploadForm: FormGroup;

  constructor(private getAllCategoriesService: GetAllCategoriesService,
    private photoPostService: PhotoPostService,
    private authService: AuthService, private formBuilder: FormBuilder) {


      this.getAllCategoriesService.getAllCategories().subscribe((data: Category[]) => {
        for (let i = 0; i < data.length; i++) {
          this.selectArray.push(data[i]);
        }
      })

      this.uploadForm = this.formBuilder.group({
        title: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(10)]],
        description: ['', [Validators.required, Validators.maxLength(200), Validators.minLength(15)]],
        location: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(3)]],
        tags: ['', Validators.required],
        categories: ['', Validators.required],
      });

      this.uploadForm.get('tags')?.valueChanges.subscribe(value => {
        this.tagInput = value;
        console.log(this.tagInput);
      });

    }
     
    

    ngOnInit(): void {
      this.getAllCategoriesService.getAllCategories().subscribe((data: Category[]) => {
        for (let i = 0; i < data.length; i++) {
          this.selectArray.push(data[i]);
        }
      })

      this.authService.getCurrentUser().subscribe((data: any) => {
        this.user = data.user;
      });


    }
    

    //--category selection--//
    selectCategory(event: any): void {
      const selectedCategoryId = event.target.value;
      
      if (selectedCategoryId !== 'select') {
        const selectedCategory = this.selectArray.find(c => c.id === +selectedCategoryId);
        
       
        if (selectedCategory && !this.categories.some(c => c.id === selectedCategory.id)) {
          this.categories.push(selectedCategory);
          this.uploadForm.patchValue({ categories: this.categories }); // Ažurirajte formu
        }
      }
      event.target.value = '';
    }
  
    removeCategory(categoryToRemove: Category): void {
    this.categories = this.categories.filter(category => category.id !== categoryToRemove.id);
    }
  //-----------------------//


    //--tag selection--//
    addTag() {

      if (this.tagInput.trim() === '') {
        return;
      }
    
      if (!this.tags.includes(this.tagInput.trim())) {
        this.tags.push(this.tagInput);
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
      this.categories = [];
      this.uploadForm.reset();
      this.uploadForm.get('categories')?.setValue('');
      this.image = false;
    }

    onSubmit():void {
      this.isLoading = true;
      console.log("cliiick");
      console.log(this.uploadForm.value);
      console.log(this.user);

      if (!this.file || !this.file.file) {
        alert('Niste odabrali fajl!');
        return;
      }


      console.log("podaci: "+this.uploadForm.value);

      const formData = new FormData();

     formData.append('Title', this.uploadForm.get('title')?.value);
     formData.append('Description', this.uploadForm.get('description')?.value);
     formData.append('Location', this.uploadForm.get('location')?.value);
     formData.append('UserId', this.user.userId.toString());
     formData.append('File', this.file.file);
     formData.append('Tags', JSON.stringify(this.tags));
     this.categories.forEach(category => {
      formData.append('Categories', JSON.stringify(category));
    });
     
    this.photoPostService.postPhoto(formData).subscribe({
      next: (response) => {
        alert("Uspjesno: ");
        this.isLoading = false;
        this.removeImage();
      },
      error: (err) => {
        alert("Uspjesno al u k: ");
        this.isLoading = false;
      }
    }); 
    }

  }


    

