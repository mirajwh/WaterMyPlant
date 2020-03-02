using System;

namespace WaterMyPlant.Models.Entity
{
    public class PlantModel : BaseEntity
    {
        private string _name;
        private string _imageUrl;
        private string _location;
        private int _wateringTimeInSec = 10; //default 10 Seconds
        private int _restingTimeInSec = 30; //default 30 Seconds
        private int _canStayWithoutWaterInMin = 360; //default 360 Minutes(6 Hours)
        private int _wateringStatus = 4; // Watered

        public string Name
        {
            get { return _name; }
            set { _name = value?.Trim() ?? string.Empty; }
        }
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value?.Trim() ?? string.Empty; }
        }
        public string Location
        {
            get { return _location; }
            set { _location = value?.Trim() ?? string.Empty; }
        }
        public int WateringTimeInSec
        {
            get { return _wateringTimeInSec; }
            set { _wateringTimeInSec = value <= 0 ? _wateringTimeInSec : value; }
        }
        public int RestingTimeInSec
        {
            get { return _restingTimeInSec; }
            set { _restingTimeInSec = value <= 0 ? RestingTimeInSec : value; }
        }
        public int CanStayWithoutWaterInMin
        {
            get { return _canStayWithoutWaterInMin; }
            set { _canStayWithoutWaterInMin = value <= 0 ? CanStayWithoutWaterInMin : value; }
        }
        public int WateringStatus { 
            get { return _wateringStatus; }
            set { _wateringStatus = value == 0 ? (int)Enums.WateringStatus.Watered : value; }
        }
        public DateTime LastWateredDateTime { get; set; } = DateTime.Now;
    }
}