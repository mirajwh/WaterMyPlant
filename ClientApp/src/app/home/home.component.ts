import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { PlantserviceService } from '../services/plantservice.service';
import { Plant } from '../interfaces/plant';
import * as signalR from '@aspnet/signalr';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  plants: Plant[];

  constructor(private plantService: PlantserviceService,
    private cdr: ChangeDetectorRef) { }

  ngOnInit() {

    this.loadAllPlants();

    this.setupSignalR();

  }

  loadAllPlants() {

    this.plantService.GetListOfPlants().subscribe(result => {
      this.plants = (<any>result).data as Plant[];
    },
      error => {
        console.log(error);
      });
  }

  startwatering(id: number) {
    let index: number = this.getIndex(id);
    this.plants[index].wateringStatus = 0;
    this.plants[index].msg = "Staring"
    this.cdr.detectChanges();
    this.plantService.StartWateringPlant(id).subscribe(result => {      
    },
      error => {
        console.log(error);
        this.plants[index].wateringStatus = -1;
        this.plants[index].msg = error.error.message;
        this.cdr.detectChanges();
      });
  }

  stopwatering(id: number) {
    let index: number = this.getIndex(id);
    this.plants[index].wateringStatus = 0;
    this.plants[index].msg = "Stopping";
    this.cdr.detectChanges();
    this.plantService.StopWateringPlant(id).subscribe(result => {
    },
      error => {
        console.log(error);
        this.plants[index].wateringStatus = -1;
        this.plants[index].msg = error.error.message;
        this.cdr.detectChanges();
      });
  }


  setupSignalR() {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/plantHub")
      .build();

    connection.start().then(function () {
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("SendThristyAlert", (msg, msg2) => {
      let index: number = this.getIndex(msg2);
      this.plants[index].wateringStatus = -1;
      this.plants[index].msg = "Please Water me.";
      this.cdr.detectChanges();
    });
    connection.on("SendWateredStatus", (msg, msg2) => {
      let index: number = this.getIndex(msg2.id);
      this.plants[index].wateringStatus = 0;
      this.plants[index].msg = "Watered Successfully. Please wait to re-water.";
      this.plants[index].lastWateredDateTime = msg2.lastWateredDateTime;
      this.cdr.detectChanges();
    });
    connection.on("SendWateringStartedStatus", (msg, msg2) => {
      let index: number = this.getIndex(msg2.id);
      this.plants[index].wateringStatus = 3;
      this.plants[index].msg = ""
      this.plants[index].lastWateredDateTime = msg2.lastWateredDateTime;
      this.cdr.detectChanges();
    });
    connection.on("SendWateringStopStatus", (msg, msg2) => {
      let index: number = this.getIndex(msg2.id);
      this.plants[index].wateringStatus = 0;
      this.plants[index].msg = "Stopped. Please wait to re-water.";
      this.plants[index].lastWateredDateTime = msg2.lastWateredDateTime;
      this.cdr.detectChanges();
    });
    connection.on("SendWateringFailedStatus", (msg, msg2) => {
      let index: number = this.getIndex(msg2.id);
      this.plants[index].wateringStatus = msg2.wateringStatus;
      this.plants[index].msg = "Watering Failed. Please wait to re-water.";
      this.plants[index].lastWateredDateTime = msg2.lastWateredDateTime;
      this.cdr.detectChanges();
    });
    connection.on("RestingFinish", (msg, msg2) => {
      let index: number = this.getIndex(msg2.id);
      this.plants[index].wateringStatus = msg2.wateringStatus;
      this.plants[index].msg = "";
      this.cdr.detectChanges();
    });
  }

  getIndex(id: number) {
    return this.plants.findIndex(p => p.id == id);
  }
}
