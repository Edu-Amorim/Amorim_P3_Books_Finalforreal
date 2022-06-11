using Amorim_P3_Books_Finalforreal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class InvoicesController : Controller
    {
        // GET: Invoices
        /// <summary>
        ///     This views shows product codes, the unit price, quantity we have in stock, item total, and the invoice id
        ///     All columns are clickable and, when you click at them it sorts the table by the clicked column
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortBy">
        ///     case 0 and default: InvoiceID; 
        ///     case 1: Customer Name;  
        ///     case 2: Invoice Date;  
        ///     case 3: Product Total;  
        ///     case 4: Sales Tax;  
        ///     case 5: Shipping Cost
        ///     case 6: Invoice Total
        /// </param>
        /// <returns>
        ///     Return AllInvoice view
        /// </returns>
        public ActionResult AllInvoices(string id = "", int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;

            switch (sortBy)
            {
                case 1:
                    invoices = context.Invoices.OrderBy(invoice => invoice.Customer.Name).ToList();
                    break;
                case 2:
                    invoices = context.Invoices.OrderBy(invoice => invoice.InvoiceDate).ToList();
                    break;
                case 3:
                    invoices = context.Invoices.OrderBy(invoice => invoice.ProductTotal).ToList();
                    break;
                case 4:
                    invoices = context.Invoices.OrderBy(invoice => invoice.SalesTax).ToList();
                    break;
                case 5:
                    invoices = context.Invoices.OrderBy(invoice => invoice.Shipping).ToList();
                    break;
                case 6:
                    invoices = context.Invoices.OrderBy(invoice => invoice.InvoiceTotal).ToList();
                    break;
                default:
                    invoices = context.Invoices.OrderBy(invoice => invoice.InvoiceID).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                invoices = invoices.Where(invoice =>
                                     invoice.InvoiceID.ToString().Contains(id) ||
                                     invoice.Customer.Name.ToLower().Contains(id) ||
                                     invoice.InvoiceDate.ToString().Contains(id) ||
                                     invoice.ProductTotal.ToString().Contains(id) ||
                                     invoice.SalesTax.ToString().Contains(id) ||
                                     invoice.Shipping.ToString().Contains(id) ||
                                     invoice.InvoiceTotal.ToString().Contains(id)
                                     ).ToList();
            }

            invoices = invoices.Where(invoice => invoice.IsDeleted == false).ToList();
            return View(invoices);
        }

        //GET: AddInvoice
        /// <summary>
        ///     This view shows the form to add, edit, and delete a invoice
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     Returns AddInvoice view
        /// </returns>
        [HttpGet]
        public ActionResult AddInvoice(int id = 0)
        {
            BooksEntities context = new BooksEntities();
            Invoice invoice = context.Invoices.FirstOrDefault(invoice1 => invoice1.InvoiceID == id);
            List<Customer> customers = context.Customers.ToList();

            if (invoice == null) invoice = new Invoice();

            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
            {
                Invoice = invoice,
                Customers = customers
            };

            return View(viewModel);
        }

        //POST: Invoice
        /// <summary>
        ///     This method does an upsert in the database with the info from the form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="CustomerId"></param>
        /// <returns>
        ///     Redirects to AllInvoices
        /// </returns>
        [HttpPost]
        public ActionResult AddInvoice(UpsertInvoiceModel model, int CustomerId)
        {
            Invoice newInvoice = model.Invoice;
            newInvoice.CustomerID = CustomerId;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Invoices.Count(invoice => invoice.InvoiceID == newInvoice.InvoiceID) > 0)
                {
                    Invoice invoiceToSave = context.Invoices.FirstOrDefault(invoice => invoice.InvoiceID == newInvoice.InvoiceID);

                    if (invoiceToSave != null)
                    {
                        invoiceToSave.CustomerID = newInvoice.CustomerID;
                        invoiceToSave.InvoiceDate = newInvoice.InvoiceDate;
                        invoiceToSave.ProductTotal = newInvoice.ProductTotal;
                        invoiceToSave.SalesTax = newInvoice.SalesTax;
                        invoiceToSave.Shipping = newInvoice.Shipping;
                    }
                }
                else context.Invoices.Add(newInvoice);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllInvoices");
        }

        //DELETE: Invoice
        /// <summary>
        ///     Does a logical deletion, i.e. turns the isDeleted attribute to true, which hides it in the AllInvoices view
        /// </summary>
        /// <param name="id">The queried id</param>
        /// <returns>
        ///     Returns a JSON containing the success status (true or false), id, and a return url (AllInvoices)
        /// </returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {

            BooksEntities context = new BooksEntities();
            if (!int.TryParse(id, out var invoiceId))
                return Json(new
                {
                    Success = true,
                    Id = id,
                    returnUrl = "/Invoices/AllInvoices"
                }, JsonRequestBehavior.AllowGet);
            try
            {
                Invoice invoice = context.Invoices.FirstOrDefault(invoice1 => invoice1.InvoiceID == invoiceId);
                if (invoice != null) invoice.IsDeleted = true;
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
                returnUrl = "/Invoices/AllInvoices"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}