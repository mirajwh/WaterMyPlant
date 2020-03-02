import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Plant } from '../interfaces/plant';
import { HttpClient } from '@angular/common/http';
import { shareReplay, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PlantserviceService {

  private BASE_URL_GET_PLANTS = "/api/Plant/GetListOfPlants";
  private BASE_URL_START_WATERING = "/api/WaterMyPlant/StartWateringPlant"
  private BASE_URL_STOP_WATERING = "/api/WaterMyPlant/StopWateringPlant"
  private BASE_URL_GET_WATERING_STATUS = "/api/WaterMyPlant/GetPlantWateringStatus"
  private BASE_URL_WATERING_HISTORY = "/api/WaterMyPlant/GetPlantWateringHistory"

  private plant$: Observable<Plant[]>;

  constructor(private http: HttpClient) { }

  GetListOfPlants(): Observable<Plant[]>
  {
    if (!this.plant$)
    {
      this.plant$ = this.http.get<Plant[]>(this.BASE_URL_GET_PLANTS).pipe();
    }

    return this.plant$;
  }

  StartWateringPlant(id: number): Observable<any> {
    
    const result = this.http.get<any>(this.BASE_URL_START_WATERING + "/" + id);

    return result;
  }

  StopWateringPlant(id: number): Observable<any> {

    const result = this.http.get<any>(this.BASE_URL_STOP_WATERING + "/" + id);

    return result;
  }

  GetPlantWateringStatus(id: number): Observable<any> {

    const result = this.http.get<any>(this.BASE_URL_GET_WATERING_STATUS + "/" + id);

    return result;
  }

  GetPlantWateringHistory(id: number): Observable<any> {

    const result = this.http.get<any>(this.BASE_URL_WATERING_HISTORY + "/" + id);

    return result;
  }
}
