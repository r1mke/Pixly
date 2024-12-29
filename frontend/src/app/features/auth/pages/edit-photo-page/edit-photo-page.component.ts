import { Component, OnInit, ViewChild } from '@angular/core';
import { NavBarComponent } from "../../../shared/components/nav-bar/nav-bar.component";
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PhotoService } from '../../../public/services/photo.service';
import { NgbdToast } from '../../../shared/components/toast/toast.component';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { NavigationComponent } from "../../../admin/components/navigation/navigation.component";
interface Approved {
  text: string;
  status: boolean;
}
@Component({
  selector: 'app-edit-photo-page',
  standalone: true,
  imports: [NavBarComponent, ReactiveFormsModule, CommonModule, NgbdToast, FormsModule, NavigationComponent],
  templateUrl: './edit-photo-page.component.html',
  styleUrl: './edit-photo-page.component.css'
})
export class EditPhotoPageComponent implements OnInit {
  editPhotoForm!: FormGroup;
  photo: any;
  isLoading: boolean = false;
  @ViewChild(NgbdToast)
  ngbdToast!: NgbdToast;
  tags: string[] = [];
  price: number = 0;
  currentUrl : string = ''
  isAdminPage : boolean = false
  approved : Approved = {text: '', status: false}
  constructor( private fb: FormBuilder, private route: ActivatedRoute,
     private photoService: PhotoService, private router: Router,
     private location: Location) { }

  ngOnInit(): void {
    this.checkUrl();
    this.initForm();
    this.getPhotoById();
  }

  checkUrl(): void {
    this.route.url.subscribe(url => {
      this.isAdminPage = this.router.url.includes('admin');
      this.currentUrl = url.join('/');
    })
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
          this.tags = this.photo.tags;
          this.price = this.photo.price;
          this.approved.text = this.photo.approved === true ? 'Approved' : 'Approve';
          console.log(this.approved);
          console.log(this.photo)
          this.updateForm(data);
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error fetching photo:', error);
          this.isLoading = false;
          this.location.back();
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

  approvePhoto() {
    this.isLoading = true;
    console.log(this.approved.status, this.photo.id);
    this.approved.status = !this.approved.status
    this.photoService.approvedPhoto(this.photo.id, this.approved.status )
    .subscribe({
      next: () => {
        this.isLoading = false;
        this.ngbdToast.showMessage('Successfully approved photo!', 'success');
        this.getPhotoById();
      },
      error: (error) => {
        console.error('Error updating photo:', error);
        this.isLoading = false;
      },
      complete:() =>{
        this.isLoading = false;
        this.approved.status = !this.approved.status

      }
    })
  }

  deletePhotoById(): void {
    this.isLoading = true;
    this.photoService.deletePhotoById(this.photo.id).subscribe({
      next: () => {
        this.isLoading = false;
        this.ngbdToast.showMessage('Successfully deleted photo!', 'success');
      },
      error: (error) => {
        console.error('Error deleting photo:', error);
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
        this.location.back();
      }
    });
  }
  
}
