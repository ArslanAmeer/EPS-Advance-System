using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EPS_Advance_Classes_Library.UserMgmt;

namespace EPS_Advance_Classes_Library.ProductMgmt
{
    public class ProductHandler : IDisposable
    {
        private readonly DBContextClass db = new DBContextClass();

        public Product GetProduct(int? id)
        {
            using (db)
            {
                return (from c in db.Products
                            .Include("Category")
                            .Include("Images")
                            .Include(x => x.User.CityId.Country)
                        where c.Id == id
                        select c).FirstOrDefault();
            }
        }

        public List<Product> GetAllProducts()
        {
            using (db)
            {
                return (from c in db.Products
                            .Include(x => x.Category)
                            .Include("Images")
                            .Include(x => x.User.CityId.Country)
                        select c).ToList();
            }
        }

        public List<Category> GetCategories()
        {
            using (db)
            {
                return (from c in db.Categories select c).ToList();
            }
        }

        public List<Product> GetAllProductsByCategory(int id)
        {
            using (db)
            {
                return (from c in db.Products
                        .Include("Category")
                        .Include("Images")
                        .Include(x => x.User.CityId.Country)
                        where c.Category.Id == id
                        select c).ToList();
            }
        }

        public void AddProduct(Product product)
        {
            using (db)
            {
                db.Entry(product.Category).State = EntityState.Unchanged;
                db.Entry(product.User).State = EntityState.Unchanged;
                db.Products.Add(product);
                db.SaveChanges();
            }
        }

        public void DeleteProduct(int id)
        {
            using (db)
            {
                // If Any Error/Exception Occured. UnComment Below Code. 
                //
                //List<ProductImages> Images = db.ProductImages.Where(p => p.Id == id).ToList();
                //db.ProductImages.RemoveRange(Images);

                Product prod = db.Products.Find(id);
                db.Products.Remove(prod);
                db.SaveChanges();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (db)
            {
                //db.Entry(product.Brand).State = EntityState.Unchanged;
                //db.Entry(product.Series).State = EntityState.Unchanged;
                //db.Entry(product.Category).State = EntityState.Unchanged;
                //db.Entry(product.SubCategory).State = EntityState.Unchanged;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public int GetProductCount()
        {
            using (db)
            {
                return (from c in db.Products select c).Count();
            }

        }

        public List<Product> RecommendedProductsForUser(User user)
        {
            using (db)
            {


                List<Product> userProducts = (from c in db.Products
                        .Include(x => x.Category)
                        .Include(x => x.User.CityId.Country)
                        .Include("Images")
                        .OrderBy(x => x.MinPrice)
                                              where c.User.Id == user.Id
                                              where c.User.CityId.Id == user.CityId.Id
                                              select c).ToList();

                List<Product> recomProducts = new List<Product>();
                foreach (var product in userProducts)
                {
                    List<Product> products = (from c in db.Products
                            .Include(x => x.Category)
                            .Include(x => x.User.CityId.Country)
                            .Include("Images")
                                              where (c.MinPrice >= product.MinPrice) && (c.MinPrice <= product.MaxPrice)
                                              where (c.MaxPrice <= product.MaxPrice) && (c.MaxPrice > product.MinPrice)
                                              where (c.User.CityId.Id == user.CityId.Id)
                                              where (c.User.Id != user.Id)
                                              select c).ToList();
                    recomProducts.AddRange(products);
                }
                recomProducts.TrimExcess();
                var filteredList = recomProducts.GroupBy(i => i.Id)
                    .Select(g => g.First()).ToList();
                return filteredList;

                //Product product2 = (from c in db.Products
                //        .Include(x => x.Category)
                //        .Include(x => x.User.CityId.Country)
                //        .Include("Images")
                //        .OrderBy(x => x.MinPrice)
                //                    where c.User.Id == user.Id
                //                    select c).Skip(1).FirstOrDefault();
                //List<Product> products2 = (from c in db.Products
                //        .Include(x => x.Category)
                //        .Include(x => x.User.CityId.Country)
                //        .Include("Images")
                //                           where (c.MinPrice >= product2.MinPrice) && (c.MinPrice <= product2.MaxPrice)
                //                           where (c.MaxPrice <= product2.MaxPrice)
                //                           where (c.User.Id != user.Id)
                //                           select c).ToList();
                //products.AddRange(products2);
                //products2.TrimExcess();
                //return products;
            }

        }

        public List<Product> RecommendedProductsForHomepage(User user)
        {
            using (db)
            {


                List<Product> userProducts = (from c in db.Products
                        .Include(x => x.Category)
                        .Include(x => x.User.CityId.Country)
                        .Include("Images")
                        .OrderBy(x => x.MinPrice)
                                              where c.User.Id == user.Id
                                              select c).ToList();
                List<Product> recomProducts = new List<Product>();
                foreach (var product in userProducts)
                {
                    List<Product> products = (from c in db.Products
                            .Include(x => x.Category)
                            .Include(x => x.User.CityId.Country)
                            .Include("Images")
                                              where (c.MinPrice >= product.MinPrice) && (c.MinPrice <= product.MaxPrice)
                                              where (c.MaxPrice <= product.MaxPrice)
                                              where (c.User.CityId.Id == product.User.CityId.Id)
                                              where (c.User.Id != user.Id)
                                              select c).Take(6).ToList();
                    recomProducts.AddRange(products);
                }
                recomProducts.TrimExcess();
                return recomProducts;

                //Product product2 = (from c in db.Products
                //        .Include(x => x.Category)
                //        .Include(x => x.User.CityId.Country)
                //        .Include("Images")
                //        .OrderBy(x => x.MinPrice)
                //                    where c.User.Id == user.Id
                //                    select c).Skip(1).FirstOrDefault();
                //List<Product> products2 = (from c in db.Products
                //        .Include(x => x.Category)
                //        .Include(x => x.User.CityId.Country)
                //        .Include("Images")
                //                           where (c.MinPrice >= product2.MinPrice) && (c.MinPrice <= product2.MaxPrice)
                //                           where (c.MaxPrice <= product2.MaxPrice)
                //                           where (c.User.Id != user.Id)
                //                           select c).ToList();
                //products.AddRange(products2);
                //products2.TrimExcess();
                //return products;
            }

        }

        public List<Product> AllProductsByCity(User user)
        {
            using (db)
            {
                return (from p in db.Products
                    .Include(x => x.Category)
                    .Include(x => x.User.CityId.Country)
                    .Include("Images")
                    .OrderBy(x => x.MinPrice)
                    .Where(x => x.User.CityId.Id == user.CityId.Id)
                        select p).ToList();
            }
        }

        public List<Product> ProductsByUser(User user)
        {
            using (db)
            {
                return (from c in db.Products
                        .Include(x => x.Category)
                        .Include("Images")
                        .Include(x => x.User.CityId.Country)
                        where c.User.Id == user.Id
                        select c).ToList();
            }
        }

        public List<Product> SearchProduct(string search)
        {
            List<Product> products;
            using (db)
            {
                products = (from p in db.Products.Where(x =>
                        x.Name.Contains(search) || x.MinPrice.ToString().Contains(search)).Include(x => x.Images).Include(x => x.User.CityId.Country).Include(x => x.Category)
                            select p).ToList();
            }

            return products;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}
