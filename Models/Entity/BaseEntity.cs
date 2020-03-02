using System.ComponentModel.DataAnnotations;

namespace WaterMyPlant.Models.Entity
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}