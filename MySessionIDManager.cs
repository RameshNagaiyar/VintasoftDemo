using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace AspNetMvcDocumentViewerDemo
{
    /// <summary>
    /// Class that allows to configure the session state for a Web application.
    /// </summary>
    public class MySessionIDManager : IHttpModule, ISessionIDManager
    {

        #region Fields

        SessionStateSection _sessionStateSection = null;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MySessionIDManager"/> class. 
        /// </summary>
        public MySessionIDManager()
        {
        }

        #endregion



        #region Methods

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="app">An System.Web.HttpApplication that provides access to the methods, properties,
        /// and events common to all application objects within an ASP.NET application.</param>
        public void Init(HttpApplication app)
        {
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements System.Web.IHttpModule.
        /// </summary>
        public void Dispose()
        {
        }


        /// <summary>
        /// Initializes the SessionIDManager object.
        /// </summary>
        public void Initialize()
        {
            if (_sessionStateSection == null)
            {
                Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                _sessionStateSection = (SessionStateSection)cfg.GetSection("system.web/sessionState");
            }
        }

        /// <summary>
        /// Performs per-request initialization of the SessionIDManager object.
        /// </summary>
        /// <param name="context">The HttpContext object that contains information about the current request.</param>
        /// <param name="suppressAutoDetectRedirect">true if the session-ID manager should redirect to determine
        /// cookie support; otherwise, false to suppress automatic redirection to determine cookie support.</param>
        /// <param name="supportSessionIDReissue">When this method returns, contains a Boolean that indicates
        /// whether the ISessionIDManager object supports issuing new session IDs when the original ID is out of date.
        /// This parameter is passed uninitialized.</param>
        /// <returns>true to indicate that the initialization performed a redirect; otherwise, false.</returns>
        public bool InitializeRequest(
            HttpContext context,
            bool suppressAutoDetectRedirect,
            out bool supportSessionIDReissue)
        {
            if (_sessionStateSection.Cookieless == HttpCookieMode.UseCookies)
            {
                supportSessionIDReissue = false;
                return false;
            }
            else
            {
                supportSessionIDReissue = true;
                return context.Response.IsRequestBeingRedirected;
            }
        }

        /// <summary>
        /// Gets the session identifier from the context of the current HTTP request.
        /// </summary>
        /// <param name="context">The current HttpContext object that references server objects
        /// used to process HTTP requests.</param>
        /// <returns>The current session identifier sent with the HTTP request.</returns>
        public string GetSessionID(HttpContext context)
        {
            string id = null;
            if (_sessionStateSection.Cookieless == HttpCookieMode.UseUri)
            {
                // do something
            }
            else
            {
                // if cookies are not empty
                if (context.Request.Cookies.Count > 0)
                    // get session ID from cookie
                    if (context.Request.Cookies[_sessionStateSection.CookieName] != null)
                        id = context.Request.Cookies[_sessionStateSection.CookieName].Value;
            }
            if (!Validate(id))
                id = null;

            return id;
        }

        /// <summary>
        /// Creates a unique session identifier.
        /// </summary>
        /// <param name="context">The current HttpContext object that references server objects
        /// used to process HTTP requests.</param>
        /// <returns>A unique session identifier.</returns>
        public string CreateSessionID(HttpContext context)
        {
            // use GUID as session ID
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Deletes the session identifier from the cookie or from the URL.
        /// </summary>
        /// <param name="context">The current HttpContext object that references server objects
        /// used to process HTTP requests.</param>
        public void RemoveSessionID(HttpContext context)
        {
            context.Response.Cookies.Remove(_sessionStateSection.CookieName);
        }

        /// <summary>
        /// Saves a newly created session identifier to the HTTP response.
        /// </summary>
        /// <param name="context">The current HttpContext object that references server objects
        /// used to process HTTP requests.</param>
        /// <param name="id">The session identifier.</param>
        /// <param name="redirected">When this method returns, contains a Boolean value that is true
        /// if the response is redirected to the current URL with the session identifier added to the URL;
        /// otherwise, false.</param>
        /// <param name="cookieAdded">When this method returns, contains a Boolean value that is true
        /// if a cookie has been added to the HTTP response; otherwise, false.</param>
        public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
        {
            redirected = false;
            cookieAdded = false;

            if (_sessionStateSection.Cookieless == HttpCookieMode.UseUri)
            {
                redirected = true;
                return;
            }
            else
            {
                // create cookie
                HttpCookie cookie = new HttpCookie(_sessionStateSection.CookieName, id);

                // update the expiration time of cookie
                cookie.Expires = DateTime.Now + _sessionStateSection.Timeout;

                // add cookie to the HTTP response
                context.Response.Cookies.Add(cookie);

                // indicate that we have added cookie to the HTTP response
                cookieAdded = true;
            }
        }

        /// <summary>
        /// Confirms that the supplied session identifier is valid.
        /// </summary>
        /// <param name="id">The session identifier to validate.</param>
        /// <returns>true if the session identifier is valid; otherwise, false.</returns>
        public bool Validate(string id)
        {
            if (id == null)
                return false;

            try
            {
                Guid testGuid = new Guid(id);
                if (id == testGuid.ToString())
                    return true;
            }
            catch
            {
            }

            return false;
        }

        #endregion

    }

}