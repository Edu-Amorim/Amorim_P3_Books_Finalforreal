using Amorim_P3_Books_Finalforreal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class CustomersController : Controller
    {
        // GET: All Customers
        /// <summary>
        ///     This views shows all customer names, address, and id
        ///     All columns are clickable and, when you click at them it sorts the table by the clicked column
        /// </summary>
        /// <param name="id">the search parameter</param>
        /// <param name="sortBy">
        ///     case 0 and default: CustomerID; 
        ///     case 1: Name;  
        ///     case 2: Address;  
        ///     case 3: City;  
        ///     case 4: State;  
        ///     case 5: Zipcode
        /// </param>
        /// <returns>
        ///     AllCustomers view
        /// </returns>
        public ActionResult AllCustomers(string id = "", int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers;

            
            switch (sortBy)
            {
                case 1:
                    customers = context.Customers.OrderBy(customer => customer.Name).ToList();
                    break;
                case 2:
                    customers = context.Customers.OrderBy(customer => customer.Address).ToList();
                    break;
                case 3:
                    customers = context.Customers.OrderBy(customer => customer.City).ToList();
                    break;
                case 4:
                    customers = context.Customers.OrderBy(customer => customer.State).ToList();
                    break;
                case 5:
                    customers = context.Customers.OrderBy(customer => customer.ZipCode).ToList();
                    break;
                default:
                    customers = context.Customers.OrderBy(customer => customer.CustomerID).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                customers = customers.Where(customer =>
                                      customer.CustomerID.ToString().Contains(id) ||
                                      customer.Name.ToLower().Contains(id) ||
                                      customer.Address.ToLower().Contains(id) ||
                                      customer.City.ToLower().Contains(id) ||
                                      customer.State.ToLower().Contains(id) ||
                                      customer.ZipCode.ToLower().Contains(id)
                                      ).ToList();
            }

            customers = customers.Where(customer => customer.IsDeleted == false).ToList();
            return View(customers);
        }

        //GET: Add Customer
        /// <summary>
        ///     This view shows the form to add, edit, and delete a customer
        /// </summary>
        /// <param name="id">the query parameter</param>
        /// <returns>
        ///     AddCustomer view
        /// </returns>
        [HttpGet]
        public ActionResult AddCustomer(int id = 0)
        {
            BooksEntities context = new BooksEntities();
            Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == id);
            List<State> states = context.States.ToList();

            if (customer == null) customer = new Customer();

            UpsertCustomerModel viewModel = new UpsertCustomerModel()
            {
                Customer = customer,
                States = states
            };

            return View(viewModel);
        }

        //POST: Customer
        /// <summary>
        ///     This method does an upsert in the database with the info from the form
        /// </summary>
        /// <param name="model">the upsert model</param>
        /// <param name="StateName"></param>
        /// <param name="CustomerAddress"></param>
        /// <param name="CustomerCity"></param>
        /// <param name="CustomerZipCode"></param>
        /// <returns>
        ///     redirects to all customers 
        /// </returns>
        [HttpPost]
        public ActionResult AddCustomer(UpsertCustomerModel model, string StateName, string CustomerAddress, string CustomerCity, string CustomerZipCode)
        {
            Customer newCustomer = model.Customer;

            newCustomer.Address = CustomerAddress;
            newCustomer.City = CustomerCity;
            newCustomer.ZipCode = CustomerZipCode;
            newCustomer.State = StateName;

            BooksEntities context = new BooksEntities();
            try
            {
                if (context.Customers.Count(customer => customer.CustomerID == newCustomer.CustomerID) > 0)
                {
                    Customer customerToSave = context.Customers.FirstOrDefault(customer => customer.CustomerID == newCustomer.CustomerID);

                    if (customerToSave != null)
                    {
                        customerToSave.Name = newCustomer.Name;
                        customerToSave.Address = newCustomer.Address;
                        customerToSave.City = newCustomer.City;
                        customerToSave.State = newCustomer.State;
                        customerToSave.ZipCode = newCustomer.ZipCode;
                    }
                }
                else context.Customers.Add(newCustomer);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("AllCustomers");
        }


        //DELETE: Customer
        /// <summary>
        ///     Does a logical deletion, i.e. turns the isDeleted attribute to true, which hides it in the AllCustomers view
        /// </summary>
        /// <param name="id">The query parameter is the ID </param>
        /// <returns>a JSON containing the succss status (true or false), the id and a return url (all customers)</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {

            BooksEntities context = new BooksEntities();
            if (!int.TryParse(id, out var customerId))
                return Json(new
                {
                    Success = true,
                    Id = id,
                    returnUrl = "/Customers/AllCustomers"
                }, JsonRequestBehavior.AllowGet);
            try
            {
                Customer customer = context.Customers.FirstOrDefault(c => c.CustomerID == customerId);
                if (customer != null) customer.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    Success = false,
                    Id = id,
                    ex.Message
                });
            }



            return Json(new
            {
                Success = true,
                Id = id,
                returnUrl = "/Customers/AllCustomers"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}