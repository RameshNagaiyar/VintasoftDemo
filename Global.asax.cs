using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace AspNetMvcDocumentViewerDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // specify that VintaSoft Imaging .NET SDK should use System.Drawing (GDI+) library for drawing of 2D graphics
            Vintasoft.Imaging.Drawing.Gdi.GdiGraphicsFactory.SetAsDefault();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["init"] = 0;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.AbsolutePath;
            if (string.IsNullOrEmpty(url) || url.Length == 1)
                return;
            bool isUrlChanged = false;

            if (url.EndsWith("AspNetMvcDocumentViewerDemo"))
            {
                url = url + "/";
                isUrlChanged = true;
            }
            if (isUrlChanged)
            {
                Response.Clear();
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", url);
                Response.End();
            }
        }

    }
}