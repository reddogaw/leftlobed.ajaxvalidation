using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers.Filters
{
    public class AjaxValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Allows specific property's validation rules to be ignored so that they can be handled manually by the controller action
        /// </summary>
        /// <remarks>
        /// Should be a comma separated list of property names.
        /// </remarks>
        public string Ignore
        {
            get;
            set;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // For AJAX requests only...
            if (!filterContext.Controller.ViewData.ModelState.IsValid && filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var ignore = new HashSet<string>();
                if (Ignore != null)
                {
                    // Split by comma and trim out the whitespace
                    var properties = Ignore.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    ignore.UnionWith(properties);
                }

                // Convert model state errors to error objects and filter those that we're meant to ignore
                var errors = FindErrors(filterContext.Controller.ViewData.ModelState).Where(e => !ignore.Contains(e.Key));
                if (errors.Any())
                {
                    // Short circuit with a JSON response
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    filterContext.Result = new JsonResult
                    {
                        Data = new AjaxResponse<ValidationError[]>(errors.ToArray()) { TypeName = "ValidationErrors" },
                        ContentType = null,
                        ContentEncoding = null,
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
                }
            }
            base.OnActionExecuting(filterContext);
        }

        internal static IEnumerable<ValidationError> FindErrors(ModelStateDictionary modelState)
        {
            var result = new List<ValidationError>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors.Select(error => new ValidationError(fieldKey, error.ErrorMessage));
                result.AddRange(fieldErrors);
            }
            return result;
        }
    }

    public static class ModelStateExtensions
    {
    }
}