using EPS_Advance_Classes_Library.ProductMgmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPS_Advance.Models
{
    public class ModelHelper
    {

        // taking Dyanimc values and adding them into SelectListItem's List

        public static List<SelectListItem> ToSelectItemList(dynamic values)
        {
            List<SelectListItem> templist = null;

            if (values != null)
            {
                templist = new List<SelectListItem>();

                foreach (var v in values)
                {
                    templist.Add(new SelectListItem { Text = v.Name, Value = Convert.ToString(v.Id) });
                }
                templist.TrimExcess();
            }

            return templist;
        }

        // Getting few data from mobile entity class to Product Summary Class

        public static ProductSummaryModel ToProductSummary(Product product)
        {
            return new ProductSummaryModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category.Name,
                Condition = product.Condition,
                MinPrice = product.MinPrice,
                MaxPrice = product.MaxPrice,
                Description = product.Description,
                ImageUrl = (product.Images.Count > 0) ? product.Images.First().Image_Url : null
            };
        }

        // Convert Product List Items to Product Summary List Items

        public static List<ProductSummaryModel> ToProdSummaryList(IEnumerable<Product> product)
        {
            List<ProductSummaryModel> prodList = new List<ProductSummaryModel>();
            if (product != null)
            {
                foreach (var c in product)
                {
                    prodList.Add(ToProductSummary(c));
                }
                prodList.TrimExcess();
            }

            return prodList;
        }

    }
}