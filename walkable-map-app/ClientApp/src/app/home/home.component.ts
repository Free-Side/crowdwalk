import { Component } from '@angular/core';
import { latLng, tileLayer, Map, polyline, Layer } from "leaflet";
import { VideoData, VideoDataLoader } from 'src/app/home/video-data';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  private _mapInstance: Map;
  private _selectedVideo: VideoData = undefined;

  protected location: string = '';

  public mapOptions: any = {
    layers: [
      tileLayer(
        'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
        {
          maxZoom: 18,
          attribution: '© <a href="https://www.openstreetmap.org/copyright" target="_blank">OpenStreetMap</a> contributors'
        }
      )
    ],
    zoom: 14,
    center: latLng(21.295372244528252, -157.86169052124026)
  };

  public mapLayers: Layer[] = [];

  constructor() {
  }

  public onMapReady(map: Map) {
    this._mapInstance = map;
    this.onMapMoving();
    this.onMapMoved();
  }

  public onMapMoving() {
    if (this._mapInstance) {
      const cntr = this._mapInstance.getCenter();
      this.location = `${cntr.lat}, ${cntr.lng}`;
    }
  }

  public async onMapMoved() {
    // Check for videos
    let bounds = this._mapInstance.getBounds();
    let filter = new URLSearchParams(<any> {
      top: bounds.getNorth(),
      left: bounds.getWest(),
      bottom: bounds.getSouth(),
      right: bounds.getEast()
    });
    let videosResponse = await fetch(`/video/find?${filter}`);
    let videos = (<any[]> await videosResponse.json()).map(VideoDataLoader.fromObject);
    if (videos.length) {
      if (!this._selectedVideo || this._selectedVideo.title !== videos[0].title) {
        if (this.mapLayers.length) {
          // this is a terrible way to organize things. ngx-leaflet supports separate child elements for each layer
          // TODO: refactor this so we can add other kinds of layers
          this.mapLayers.splice(0, 1);
        }
        this._selectedVideo = videos[0];

        this.mapLayers.push(
          polyline(this._selectedVideo.points.map(p => latLng(p.latitude, p.longitude)))
        )
      }
    } else if (this._selectedVideo) {
      this._selectedVideo = undefined;
      this.mapLayers.splice(0, 1);
    }
  }
}
