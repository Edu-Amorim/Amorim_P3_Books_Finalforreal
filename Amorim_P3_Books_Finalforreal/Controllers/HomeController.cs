using Square;
using Square.Exceptions;
using Square.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Square Payments";
            SquareClient client = new SquareClient.Builder()
                                    .Environment(Square.Environment.Sandbox)
                                    .AccessToken("EAAAEG7IlIlmw725UfwlHXX1jHgL2FmHfSzn4RLlg-ZPFAVncs6bHUH5TzFO9tDz")
                                    .Build();

            // create a payment from Square
            var amountMoney = new Money.Builder()
                              .Amount(100L)
                              .Currency("CAD")
                              .Build();

            var body = new CreatePaymentRequest.Builder(
                sourceId: "cnon:card-nonce-ok",
                idempotencyKey: Guid.NewGuid().ToString(),
                amountMoney: amountMoney)
                .Build();

            try
            {

                var result = client.PaymentsApi.CreatePayment(body: body);
            }
            catch (ApiException e)
            {
                Console.WriteLine("Failed to make the request");
                Console.WriteLine($"Response Code: {e.ResponseCode}");
                Console.WriteLine($"Exception: {e.Message}");
            }

            return View();
        }
    }
}