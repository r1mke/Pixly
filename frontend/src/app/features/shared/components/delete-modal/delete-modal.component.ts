import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-delete-modal',
  templateUrl: './delete-modal.component.html',
  styleUrls: ['./delete-modal.component.css'],
  standalone: true,
})
export class DeleteModalComponent {
  @Output() deleteConfirmed = new EventEmitter<void>();

  onDelete(): void {
    this.deleteConfirmed.emit();
  }
}
