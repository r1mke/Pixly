import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { loadStripe } from '@stripe/stripe-js';
import { Observable } from 'rxjs';
import { MYCONFIG } from '../../../my-config';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  stripePromise = loadStripe('pk_test_51Qaeu5Im4IWmKNZDZ3PfYcX7Zu8HVETfO1zQI1PKuMrUqxcpmCqDJPQv172fVVR9oOkKqoKO757amcPxLikfyhgH00RBTtt4Hb');
  private apiUrl = `${MYCONFIG.apiUrl}`;

  constructor(private http: HttpClient) {}

  checkout(amount: number, currency: string, photoImage: string, photoDescription: string, photoId: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/stripe/create-checkout-session`, {
      amount: amount,
      currency: currency,
      photoImage: photoImage,
      photoDescription: photoDescription,
      successUrl: window.location.origin + `/public/success/photoId/${photoId}`,
      cancelUrl: window.location.origin + '/cancel'
    });
  }

  redirectToCheckout(sessionId: string) {
    this.stripePromise.then((stripe) => {
      stripe!.redirectToCheckout({ sessionId });
    });
  }

  downloadPhoto(photoId: any): Observable<any> {
    return this.http.get(`${this.apiUrl}/api/stripe/download-photo/${photoId}`, { responseType: 'blob' });
  }

}
