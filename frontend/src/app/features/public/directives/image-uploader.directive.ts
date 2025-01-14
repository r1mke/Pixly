import {
  Directive,
  HostBinding,
  HostListener,
  Output,
  EventEmitter
} from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { ImageFile } from "../model/image-file";

enum DropColor {
  Default = "#fff",
  Over = "#80d1b21e"
}

@Directive({
  selector: "[corpImgUpload]",
  standalone: true,
})
export class ImageUploaderDirective {
  @Output() dropFiles: EventEmitter<ImageFile> = new EventEmitter();  
  @HostBinding("style.background") backgroundColor = DropColor.Default;

  constructor(private sanitizer: DomSanitizer) {}

  @HostListener("dragover", ["$event"]) public dragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.backgroundColor = DropColor.Over;
  }

  @HostListener("dragleave", ["$event"]) public dragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.backgroundColor = DropColor.Default;
  }

  @HostListener("drop", ["$event"]) public drop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();

    const dataTransfer = event.dataTransfer;
    if (!dataTransfer) return;

    const fileList = event.dataTransfer.files;
    if (fileList.length > 1) {
      console.warn("Only one file is allowed.");
      return;
    }

    const file = fileList[0];
    const url = this.sanitizer.bypassSecurityTrustUrl(window.URL.createObjectURL(file));
    const imageFile: ImageFile = { file, url };

    this.dropFiles.emit(imageFile); 
  }
}
