using Pjax.Mvc5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MStack.MainSite.Controllers
{
    [Pjax.Mvc5.Pjax]
    public class BaseController : Controller, IPjax
    {
        public bool IsPjaxRequest { get; set; }

        public string PjaxVersion { get; set; }
    }
}
