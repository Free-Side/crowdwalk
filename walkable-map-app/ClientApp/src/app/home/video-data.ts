export type VideoData = {
  title: string;
  url: string;
  points: Point[];
  center: Point;
  bounds: Bounds;
};

export type Point = {
  latitude: number;
  longitude: number;
  time?: Date;
}

export type Bounds = {
  top: number;
  left: number;
  bottom: number;
  right: number;
}

export const VideoDataLoader = {
  fromObject: function(value: any): VideoData {
    return {
      title: value.title,
      url: value.url,
      points: (value.points || []).map(pointFromObject),
      center: pointFromObject(value.center ?? {}),
      bounds: value.bounds
    };
  }
}

function pointFromObject(value: any): Point {
  let result = {
    latitude: value.latitude,
    longitude: value.longitude
  };
  if (value.time) {
    return {
      ...result,
      time: value.time instanceof Date ? value.time : new Date(value.time)
    }
  } else {
    return result;
  }
}
