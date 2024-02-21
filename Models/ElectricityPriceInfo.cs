using System;
using System.ComponentModel.DataAnnotations;

namespace Microservice_Teht.Models
{
    public class ElectricityPriceInfo
    {
        [Key]
        public int Id { get; set; }
        public decimal price { get; set; }
        public DateTime endDate { get; set; }
        public DateTime startDate { get; set; }
    }
}
