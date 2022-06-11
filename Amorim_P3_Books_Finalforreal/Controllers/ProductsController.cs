using Amorim_P3_Books_Finalforreal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        /// <summary>
        ///     This views shows product codes, the description, quantity we have in stock, and product code
        ///     All columns are clickable and, when you click at them it sorts the table by the clicked column
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortBy">
        ///     case 0 and default: Product Code; 
        ///     case 1: Description;  
        ///     case 2: Unit Price;  
        ///     case 3: On Hand Quantity;  
        /// </param>
        /// <returns>
        ///     Return AllProducts view
        /// </returns>
        public ActionResult AllProducts(string id = "", int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;

            switch (sortBy)
            {
                case 1:
                    products = context.Products.OrderBy(product => product.Description).ToList();
                    break;
                case 2:
                    products = context.Products.OrderBy(product => product.UnitPrice).ToList();
                    break;
                case 3:
                    products = context.Products.OrderBy(product => product.OnHandQuantity).ToList();
                    break;
                default:
                    products = context.Products.OrderBy(product => product.ProductCode).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                products = products.Where(product =>
                                       product.ProductCode.ToLower().Contains(id) ||
                                       product.Description.ToLower().Contains(id) ||
                                       product.UnitPrice.ToString().Contains(id) ||
                                       product.OnHandQuantity.ToString().Contains(id)
                                       ).ToList();
            }

            products = products.Where(product => product.IsDeleted == false).ToList();
            return View(products);
        }

        //GET: ADDProducts
        /// <summary>
        ///     This view shows the form to add, edit, and delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     Returns AddProduct view
        /// </returns>
        [HttpGet]
        public ActionResult AddProduct(string id)
        {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.FirstOrDefault(product1 => product1.ProductCode == id) ?? new Product();

            return View(product);
        }


        //POST: Product
        /// <summary>
        ///     This method does an upsert in the database with the info from the form
        /// </summary>
        /// <param name="newProduct">the new product to be created</param>
        /// <returns>
        ///     Redirects to AllProducts
        /// </returns>
        [HttpPost]
        public ActionResult AddProduct(Product newProduct)
        {

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Products.Count(product => product.ProductCode == newProduct.ProductCode) > 0)
                {
                    Product productToSave = context.Products.FirstOrDefault(product => product.ProductCode == newProduct.ProductCode);

                    if (productToSave != null)
                    {
                        productToSave.Description = newProduct.Description;
                        productToSave.UnitPrice = newProduct.UnitPrice;
                        productToSave.OnHandQuantity = newProduct.OnHandQuantity;
                    }
                }
                else context.Products.Add(newProduct);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllProducts");
        }

        //DELETE: Product
        /// <summary>
        ///     Does a logical deletion, i.e. turns the isDeleted attribute to true, which hides it in the AllProducts view
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     Returns a JSON containing the success status (true or false), id, and a return url (AllProducts)
        /// </returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                Product product = context.Products.FirstOrDefault(product1 => product1.ProductCode == id);
                if (product != null) product.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Id = id,
                    Message = ex.Message
                });
            }
            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/Products/AllProducts"
            }, JsonRequestBehavior.AllowGet);

        }
    }
}