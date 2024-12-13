import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  standalone: true,
  name: 'customDate'
})
export class CustomDatePipe implements PipeTransform {

  transform(value: string): string {
    if (!value) return '';

    const date = new Date(value);
    const day = this.padZero(date.getDate());
    const year = date.getFullYear();
    const monthName = this.getMonthName(date.getMonth());

    return `${monthName} ${day}, ${year}`;
  }

  private padZero(num: number): string {
    return num < 10 ? `0${num}` : `${num}`;
  }

  private getMonthName(monthIndex: number): string {
    const months = [
      'January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'
    ];
    return months[monthIndex];
  }
}
