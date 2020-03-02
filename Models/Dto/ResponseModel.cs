using System;

namespace WaterMyPlant.Models.Dto
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public dynamic Data { get; set; }
        public string Type { get; set; }
        public DateTime ResponseDateTime { get; private set; } = DateTime.Now;
    }
}