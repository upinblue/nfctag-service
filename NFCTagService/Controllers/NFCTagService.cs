using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Net.Http;


namespace NFCTagService.Controllers
{
    [ApiController]
    [Route("v1/api/NFC")]
    [Produces("application/json")]
    public class NFCTagService : ControllerBase
    {

        DatabaseController db;
        public NFCTagService() { 
            db = new DatabaseController();
        }

        [HttpPost]
        [Route("saveTag")]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<string>>> saveTag()
        {

            string tagID = Request.Headers["TagID"];
            string serial = Request.Headers["Serial"];

            string savedURL = Request.Headers["Value"];
            string applicationKey = Request.Headers["Issuer"];
            string token = Request.Headers["Token"];

            if (string.IsNullOrEmpty(tagID) || string.IsNullOrEmpty(applicationKey))
            {
#if DEBUG
                Response.Headers.Add("error", "empty input");
#endif
                return StatusCode(400);
            }

            
            if (!await isApplicationEntitledAsync(applicationKey))
            {
#if DEBUG
                Response.Headers.Add("error", "permission denied");
#endif
                return StatusCode(401);
            }

            if (!await isUserAuthenticated(token))
            {
#if DEBUG
                Response.Headers.Add("error", "permission denied");
#endif
                return StatusCode(401);
            }


            if (!db.saveTag(tagID, serial))
            {
                return StatusCode(500);
            }

            int userID = db.getUserIDBySessionToken(token);
            if (userID == -1)
            {
#if DEBUG
                Response.Headers.Add("sessionerror", "non active session");
#endif
                return StatusCode(500);
            }

            if (!db.addFlashHistory(savedURL, serial, userID))
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }



        private async System.Threading.Tasks.Task<bool> isApplicationEntitledAsync(string applicationKey)
        {

            System.Uri uri = new Uri("https://api.upinblue.com/v1/api/Entitlement/checkEntitlement");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Create a HttpWebrequest object to the desired URL.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Headers["token"] = applicationKey;
            req.Headers["entitlement"] = "2";
            try
            {
                WebResponse x = await req.GetResponseAsync();

                x.Close();
                return true;
            }
            catch (WebException e)
            {
#if DEBUG
                Response.Headers.Add("entitlementerror", "checking entitlement: " + e.Message);
#endif
                return false;
            }
        }

        private async System.Threading.Tasks.Task<bool> isUserAuthenticated(string token)
        {

            System.Uri uri = new Uri("https://api.upinblue.com/v1/api/Authentication/authenticate");
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // Create a HttpWebrequest object to the desired URL.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Headers["token"] = token;
            try
            {
                WebResponse x = await req.GetResponseAsync();

                x.Close();
                return true;
            }
            catch (WebException e)
            {
#if DEBUG
                Response.Headers.Add("sessionerror", "checking session: " + e.Message);
#endif
                return false;
            }
        }

    }
}
