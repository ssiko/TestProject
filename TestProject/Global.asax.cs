﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace TestProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}