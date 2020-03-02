export interface Plant {
  id?: number;
  name: string;
  imageUrl: string;
  location: string;
  wateringTimeInSec: number;
  restingTimeInSec: number;
  canStayWithoutWaterInMin: number;
  wateringStatus: number;
  lastWateredDateTime: string;
  msg: string;
}
