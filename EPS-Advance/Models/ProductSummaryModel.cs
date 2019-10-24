using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPS_Advance.Models
{
    public class ProductSummaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int Condition { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}