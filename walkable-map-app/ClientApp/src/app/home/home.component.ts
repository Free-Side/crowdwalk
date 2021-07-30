import { Component, ElementRef, ViewChild } from '@angular/core';
import { latLng, tileLayer, Map, polyline, Layer, marker, Marker, icon, LatLng, latLngBounds } from "leaflet";
import { Point, VideoData, VideoDataLoader } from 'src/app/home/video-data';
import * as p5 from 'p5';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  private static MarkerIcon = icon({
    iconSize: [25, 41],
    iconAnchor: [13, 41],
    // specify the path here
    iconUrl: 'assets/marker-icon.png',
    shadowUrl: 'assets/marker-shadow.png'
  });

  private _mapInstance: Map;
  private _selectedVideo: VideoData = undefined;
  private _videoMarker: Marker;
  private _videoElement: p5.MediaElement;
  private _videoPlaying: boolean = false;

  @ViewChild('videoContainer')
  public videoContainer: ElementRef<HTMLDivElement>;

  public mapCenter: string = '';
  public timeOffset: number = 0;
  public percentComplete: number = 0;

  public videoContainerClasses: string[] = ['no-display'];

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
  private _videoDisplay: p5;

  constructor() {
  }

  ngOnInit() {
    const sketch = (p: p5) => {
      p.setup = () => {
        p.noCanvas();
      };

      p.draw = () => {
      };
    }

    this._videoDisplay = new p5(sketch);
  }


  public onMapReady(map: Map) {
    this._mapInstance = map;
    this.onMapMoving();
    this.onMapMoved();
  }

  public onMapMoving() {
    if (this._mapInstance) {
      const pos = this._mapInstance.getCenter();
      this.mapCenter = `${pos.lat}, ${pos.lng}`;
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
          this.mapLayers.splice(0, 2);
        }
        this._selectedVideo = videos[0];
        console.log(`Track Duration: ${(this._selectedVideo.points[this._selectedVideo.points.length - 1].time.getTime() - this._selectedVideo.points[0].time.getTime()) / 1000}`)

        this.mapLayers.push(
          polyline(this._selectedVideo.points.map(p => latLng(p.latitude, p.longitude))),
          this._videoMarker =
            marker(
              latLng(this._selectedVideo.points[0].latitude, this._selectedVideo.points[0].longitude),
              { draggable: true, icon: HomeComponent.MarkerIcon }
            )
        );

        this._videoElement =
          this._videoDisplay.createVideo(
            this._selectedVideo.url,
            loaded => {
              this._videoElement.volume(0);
              console.log(`Video Duration: ${this._videoElement.duration()}`);
            }
          );
        (<p5.Element> <unknown> this._videoElement).parent(this.videoContainer.nativeElement);

        this._videoMarker.on('click', e => {
          // toggle video display
          let ix = this.videoContainerClasses.indexOf('no-display');
          if (ix >= 0) {
            this.videoContainerClasses = this.videoContainerClasses.filter(c => c !== 'no-display');
          } else {
            this.videoContainerClasses = [...this.videoContainerClasses, 'no-display'];
          }
        });

        this._videoMarker.on('drag', e => {
          // Scrub video
          const pos = this._videoMarker.getLatLng();
          let closest: { dist: number, point: Point, ix: number };
          let ix = 0;
          for (const p of this._selectedVideo.points) {
            let dist = sqdist(pos, p);
            if (!closest || dist < closest.dist) {
              closest = { dist, point: p, ix };
            }
            ix++;
          }

          this._videoMarker.setLatLng(latLng(closest.point.latitude, closest.point.longitude));

          let dest = (closest.point.time.getTime() - this._selectedVideo.points[0].time.getTime()) / 1000;
          this.timeOffset = dest;
          this.percentComplete = closest.ix / this._selectedVideo.points.length;
          let delta = dest - this._videoElement.time();
          try {
            if (delta > 0.5 && delta < 10) {
              (<any> this._videoElement).clearCues();
              this._videoElement.speed(Math.min(16, delta * 2)); // attempt to advance to the current slider position in 1/2 second regardless of how far
              this._videoElement.addCue(
                dest,
                () => {
                  this._videoElement.pause();
                  this._videoPlaying = false;
                }
              );
              if (!this._videoPlaying) {
                this._videoElement.play();
                this._videoPlaying = true;
              }
            } else if (Math.abs(delta) > 0.5) {
              (<any> this._videoElement).clearCues();
              if (this._videoPlaying) {
                this._videoElement.pause();
                this._videoPlaying = false;
              }
              this._videoElement.time(dest);
            }
          } catch (e) {
            console.error(e);
          }
        });

        this._videoMarker.on('dragend', e => {
          // Snap to path

        });
      }
    } else if (this._selectedVideo) {
      this._selectedVideo = undefined;
      this.mapLayers.splice(0, 2);
      (<p5.Element> <unknown> this._videoElement).remove();
      this.videoContainerClasses = ['no-display'];
    }
  }
}

function sqdist(start: LatLng, end: Point): number {
  return (end.latitude - start.lat) ** 2 + (end.longitude - start.lng) ** 2;
}
