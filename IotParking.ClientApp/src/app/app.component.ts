import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface ParkingSpot {
  naziv: string;
  latitude: number;
  longitude: number;
  slobodan: boolean;
}
@Component({
  selector: 'app-root',
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  parkingSpots: ParkingSpot[] = [];
  isLoading = false;
  private mockData: ParkingSpot[] = [
    {
      naziv: 'Trg Partizana',
      latitude: 43.8563,
      longitude: 19.8435,
      slobodan: true,
    },
    {
      naziv: 'Gradski Park',
      latitude: 43.8571,
      longitude: 19.8421,
      slobodan: false,
    },
  ];
  totalSpots: number = 2;
  constructor(private http: HttpClient) { }
  ngOnInit() {
    this.refreshParkingSpots();
  }

  refreshParkingSpots() {
    this.isLoading = true;
    this.http
      .get('http://192.168.0.12:5050/api/ParkingSpot/get-free')
      .subscribe({
        next: (res: any) => {
          this.parkingSpots = res;
        },
        error: (err: any) => {
          console.error('Failed to get free parking spots:', err);
        },
      });
    this.isLoading = false;
  }

  trackByName(index: number, spot: ParkingSpot): string {
    return spot.naziv;
  }
  openInGoogleMaps(spot: ParkingSpot): void {
    const url = `https://www.google.com/maps?q=${spot.latitude},${spot.longitude}&z=17`;
    window.open(url, '_blank');
  }

  navigateToSpot(spot: ParkingSpot): void {
    const url = `https://www.google.com/maps/dir/?api=1&destination=${spot.latitude},${spot.longitude}`;
    window.open(url, '_blank');
  }
}
