using Amorim_P3_Books_Finalforreal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class InvoiceLineItemsController : Controller
    {
        // GET: All InvoiceLineItems
        /// <summary>
        ///     This views shows product codes, the unit price, quantity we have in stock, item total, and the invoice id
        ///     All columns are clickable and, when you click at them it sorts the table by the clicked column
        /// </summary>
        /// <param name="id">query parameter</param>
        /// <param name="sortBy">
        ///     case 0 and default: InvoiceID; 
        ///     case 1: ProductCode;  
        ///     case 2: UnitPrice;  
        ///     case 3: Quantity;  
        ///     case 4: ItemTotal;  
        /// </param>
        /// <returns>AllInvoiceLineItems view </returns>
        public ActionResult AllInvoiceLineItems(string id = "", int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> invoiceLineItems;

            switch (sortBy)
            {
                case 1:
                    invoiceLineItems = context.InvoiceLineItems.OrderBy(invoiceLineItem => invoiceLineItem.ProductCode).ToList();
                    break;
                case 2:
                    invoiceLineItems = context.InvoiceLineItems.OrderBy(invoiceLineItem => invoiceLineItem.UnitPrice).ToList();
                    break;
                case 3:
                    invoiceLineItems = context.InvoiceLineItems.OrderBy(invoiceLineItem => invoiceLineItem.Quantity).ToList();
                    break;
                case 4:
                    invoiceLineItems = context.InvoiceLineItems.OrderBy(invoiceLineItem => invoiceLineItem.ItemTotal).ToList();
                    break;
                default:
                    invoiceLineItems = context.InvoiceLineItems.OrderBy(invoiceLineItem => invoiceLineItem.InvoiceID).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                invoiceLineItems = invoiceLineItems.Where(li =>
                                         li.InvoiceID.ToString().Contains(id) ||
                                         li.ProductCode.ToLower().Contains(id) ||
                                         li.UnitPrice.ToString().Contains(id) ||
                                         li.Quantity.ToString().Contains(id) ||
                                         li.ItemTotal.ToString().Contains(id)
                                         ).ToList();
            }

            invoiceLineItems = invoiceLineItems.Where(li => li.IsDeleted == false).ToList();
            return View(invoiceLineItems);
        }


        //GET: Add InvoiceLineItem
        /// <summary>
        ///     This view shows the form to add, edit, and delete a invoicelineitem
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="productId"></param>
        /// <returns>
        ///     add customer view
        /// </returns>
        [HttpGet]
        public ActionResult AddInvoiceLineItem(int invoiceId, string productId)
        {
            BooksEntities context = new BooksEntities();
            InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.FirstOrDefault(li => li.InvoiceID == invoiceId && li.ProductCode == productId);
            List<Invoice> invoices = context.Invoices.ToList();
            List<Product> products = context.Products.ToList();

            if (invoiceLineItem == null) invoiceLineItem = new InvoiceLineItem();

            UpsertInvoiceLineItemModel viewModel = new UpsertInvoiceLineItemModel()
            {
                InvoiceLineItem = invoiceLineItem,
                Invoices = invoices,
                Products = products
            };

            return View(viewModel);
        }

        //POST: InvoiceLineItem
        /// <summary>
        ///     This method does an upsert in the database with the info from the form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="InvoiceID"></param>
        /// <param name="ProductName"></param>
        /// <returns>
        ///     Redirects to AllInvoiceLineItems
        /// </returns>
        [HttpPost]
        public ActionResult AddInvoiceLineItem(UpsertInvoiceLineItemModel model, int InvoiceID, string ProductName)
        {
            InvoiceLineItem newInvoiceLineItem = model.InvoiceLineItem;
            newInvoiceLineItem.InvoiceID = InvoiceID;
            newInvoiceLineItem.ProductCode = ProductName;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.InvoiceLineItems.Count(li => li.InvoiceID == newInvoiceLineItem.InvoiceID && li.ProductCode == newInvoiceLineItem.ProductCode) > 0)
                {
                    InvoiceLineItem invoiceLineItemToSave = context.InvoiceLineItems.FirstOrDefault(li => li.InvoiceID == newInvoiceLineItem.InvoiceID && li.ProductCode == newInvoiceLineItem.ProductCode);

                    if (invoiceLineItemToSave != null)
                    {
                        invoiceLineItemToSave.UnitPrice = newInvoiceLineItem.UnitPrice;
                        invoiceLineItemToSave.Quantity = newInvoiceLineItem.Quantity;
                        invoiceLineItemToSave.ItemTotal = newInvoiceLineItem.ItemTotal;
                    }
                }
                else context.InvoiceLineItems.Add(newInvoiceLineItem);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllInvoiceLineItems");
        }

        //DELETE: InvoiceLineItems
        /// <summary>
        ///     Does a logical deletion, i.e. turns the isDeleted attribute to true, which hides it in the AllInvoiceLineItems view
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="productId"></param>
        /// <returns>
        ///     Returns a JSON containing the success status (true or false), the invoiceLineItem id, product id and a return url (AllInvoiceLineItems)
        /// </returns>
        [HttpGet]
        public ActionResult Delete(int invoiceId, string productId)
        {

            BooksEntities context = new BooksEntities();

            try
            {
                InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.FirstOrDefault(li => li.InvoiceID == invoiceId && li.ProductCode == productId);
                if (invoiceLineItem != null) invoiceLineItem.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Id = invoiceId,
                    ProdId = productId,
                    Message = ex.Message
                });
            }
            return Json(new
            {
                Success = true,
                Id = invoiceId,
                ProdId = productId,
                returnUrl = "/InvoiceLineItems/AllInvoiceLineItems"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}