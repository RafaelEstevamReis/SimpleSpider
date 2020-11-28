using System;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers.HTML;

namespace RafaelEstevam.Simple.Spider.Test.Sample
{
    /// <summary>
    /// Simple example to demonstrate authentication
    /// </summary>
    public class QuotesToScrape_Login
    {
        public static void run()
        {
            // Sets our page address and auxialiary variables
            Uri login = new Uri("http://quotes.toscrape.com/login");
            Tag page = null;
            // create the request instance and sets is to use cookies
            RequestHelper req = new RequestHelper(UseCookies: true);
            // Request the login url to get the cookie and the form
            var success = req.SendGetRequest(login, (s, e) => page = new Tag(e.GetDocument()));
            if (!success) throw new InvalidOperationException(" Oh, no =( ");
            // Get all form fields
            Form form = page.SelectTag<Form>("//form");
            var formData = form.GetFormData();
            // Fill credentials
            formData["username"] = "ABC";
            formData["password"] = "secret";
            // submit our form
            success = req.SendFormData(login, formData, null);

            if (!success) throw new InvalidOperationException(" We did't make it =( ");
            Console.WriteLine("We make it !");
        }
    }
}