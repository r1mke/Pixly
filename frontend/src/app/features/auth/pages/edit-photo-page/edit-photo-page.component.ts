import { Component, OnInit, ViewChild } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PhotoService } from '../../../public/services/photo.service';
import { NgbdToast } from '../../../shared/components/toast/toast.component';

@Component({
  selector: 'app-edit-photo-page',
  standalone: true,
  imports: [NavBarComponent, ReactiveFormsModule, CommonModule, NgbdToast],
  templateUrl: './edit-photo-page.component.html',
  styleUrl: './edit-photo-page.component.css'
})
export class EditPhotoPageComponent implements OnInit {
  editPhotoForm!: FormGroup;
  photo: any;
  isLoading: boolean = false;
  @ViewChild(NgbdToast)
  ngbdToast!: NgbdToast;

  constructor( private fb: FormBuilder, private route: ActivatedRoute, private photoService: PhotoService) { }

  ngOnInit(): void {
    this.initForm();
    this.getPhotoById();
  }

  private initForm(): void {
    this.editPhotoForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.maxLength(200), Validators.minLength(5)]],
      location: ['', [Validators.required, Validators.maxLength(50), Validators.minLength(3)]],
      tags: ['', Validators.required]
    });
  }

  getPhotoById(): void {
    const photoId = Number(this.route.snapshot.paramMap.get('id'));
    if (photoId) {
      this.isLoading = true;
      this.photoService.getPhotoById(photoId).subscribe({
        next: (data) => {
          this.photo = data;
          console.log(this.photo)
          this.updateForm(data);
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching photo:', error);
          this.isLoading = false;
        }
      });
    } else {
      console.error('No photo ID provided');
    }
  }
  
  updateForm(photo: any) {
    this.editPhotoForm.patchValue({
      title: photo.title || '',
      description: photo.description || '',
      location: photo.location || '',
      tags: photo.tags || ''
    });
  }


  public onSubmit():void{
    const { title, description, location } = this.editPhotoForm.getRawValue();
    this.isLoading = true;

    this.photoService.updatePhoto(this.photo.id, {title, description, location})
    .subscribe({
      next: () => {
        this.isLoading = false;
        this.ngbdToast.showMessage('Successfully updated photo!', 'success');
        this.getPhotoById();
      },
      error: (error) => {
        console.error('Error updating photo:', error);
        this.isLoading = false;
      },
      complete:() =>{
        this.isLoading = false;
      }
    })
  }
  
}
