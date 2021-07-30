import { Component } from '@angular/core';
import { latLng, tileLayer, Map } from "leaflet";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  private _mapInstance: Map;

  protected location: string = '';

  public mapOptions: any = {
    layers: [
      tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 18, attribution: '...' })
    ],
    zoom: 14,
    center: latLng(21.295372244528252, -157.86169052124026)
  };

  constructor() {
  }

  public onMapReady(map: Map) {
    this._mapInstance = map;
    this.onMapMoved();
  }

  public onMapMoved() {
    if (this._mapInstance) {
      const cntr = this._mapInstance.getCenter();
      this.location = `${cntr.lat}, ${cntr.lng}`;
    }
  }
}
