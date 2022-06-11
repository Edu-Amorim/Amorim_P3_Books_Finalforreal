using Amorim_P3_Books_Finalforreal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class StatesController : Controller
    {
        // GET: AllStates
        /// <summary>
        ///     This views shows state code and name
        ///     All columns are clickable and, when you click at them it sorts the table by the clicked column
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortBy">
        ///     case 0 and default: State Code; 
        ///     case 1: State Name;  
        /// </param>
        /// <returns></returns>
        public ActionResult AllStates(string id = "", int sortBy = 0)
        {
            BooksEntities context = new BooksEntities();
            List<State> states;

            switch (sortBy)
            {
                case 1:
                    states = context.States.OrderBy(state => state.StateName).ToList();
                    break;
                default:
                    states = context.States.OrderBy(state => state.StateCode).ToList();
                    break;
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();

                states = states.Where(state =>
                                     state.StateCode.ToLower().Contains(id) ||
                                     state.StateName.ToLower().Contains(id)
                                     ).ToList();
            }

            states = states.Where(state => state.IsDeleted == false).ToList();

            return View(states);
        }


        //Get: AddState
        /// <summary>
        ///     This view shows the form to add, edit, and delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///     Returns AddProduct view
        /// </returns>
        [HttpGet]
        public ActionResult AddState(string id)
        {
            BooksEntities context = new BooksEntities();
            State state = context.States.FirstOrDefault(state1 => state1.StateCode == id) ?? new State();

            return View(state);
        }


        //POST: State
        /// <summary>
        ///     This method does an upsert in the database with the info from the form
        /// </summary>
        /// <param name="newState">new state to be created</param>
        /// <returns>
        ///     Redirects to AllStates
        /// </returns>
        [HttpPost]
        public ActionResult AddState(State newState)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                if (context.States.Count(state => state.StateCode == newState.StateCode) > 0)
                {
                    State stateToSave = context.States.FirstOrDefault(state => state.StateCode == newState.StateCode);

                    if (stateToSave != null) stateToSave.StateName = newState.StateName;
                }
                else context.States.Add(newState);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("AllStates");
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
                State state = context.States.FirstOrDefault(s => s.StateCode == id);
                if (state != null) state.IsDeleted = true;
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
                returnUrl = "/States/AllStates"
            }, JsonRequestBehavior.AllowGet);

        }
    }
}