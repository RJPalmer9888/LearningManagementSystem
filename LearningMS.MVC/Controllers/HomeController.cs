using LearningMS.MVC.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace LearningMS.MVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel cvm)
        {
            // It is best practice to confirm the "state" of the model
            if (!ModelState.IsValid)
            {
                //  Send them back to the form, by passing the object to the view, 
                //the form returns with the original populated information.
                return View(cvm);
            }

            // Below code only executes if the form (object) passes model validation
            string returnMessage = $"You have received an email from {cvm.Name} with a subject " +
                $"{cvm.Subject}.  Please respond to {cvm.Email} with your response to " +
                $"the following message: <br />{cvm.Message}";

            Boolean isMailSetUp = true;

            if (isMailSetUp)
            {
                //Add using statements for the System Mail
                //Mailmessage Package is what sends the email (System.Net.Mail)
                MailMessage mm = new MailMessage(
                     // From
                     ConfigurationManager.AppSettings["EmailUser"].ToString(),

                    ConfigurationManager.AppSettings["EmailTo"].ToString(),
                    cvm.Subject, returnMessage
                    )
                {

                    //Mailmessage properties
                    //Allow HTML formatting
                    IsBodyHtml = true,

                    //Set Mail priority
                    Priority = MailPriority.High //Default is normal priority
                };

                //Set up the reply list
                mm.ReplyToList.Add(cvm.Email);

                //  SmtpClient - This is the information from the HOST (smarterAsp.net) 
                // that allows the email to actually be sent
                SmtpClient client = new SmtpClient(
                    ConfigurationManager.AppSettings["EmailClient"].ToString());

                //  Client credentials (smarterASP requires your user name and password)
                client.Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailUser"].ToString(),
                    ConfigurationManager.AppSettings["EmailPassword"].ToString());

                //  It is possible that the mailserver is down or we may have configuration 
                // issues, so we want to encapsulate our code in a try/catch
                try
                {
                    //Attempt to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    ViewBag.CustomerMessage = $"We're sorry your request could not be " +
                        $"completed at this time." +
                        $"  Please try again later.  Error Message: <br /> {ex.StackTrace}";
                    return View(cvm); //  Return the view with the entire message so that 
                                      //  users can copy/paste it for later


                }

            }

            return View("EmailConfirmation", cvm); //Upon successful email send users to
                                                   //a friendly confirmation page

        }
    }
}
