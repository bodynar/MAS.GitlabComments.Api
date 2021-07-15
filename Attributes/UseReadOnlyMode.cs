namespace MAS.GitlabComments.Attributes
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Action filter to reject requests for endpoints not marked with <see cref="AllowInReadOnlyAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class UseReadOnlyModeAttribute : Attribute, IActionFilter
    {
        /// <inheritdoc cref="IActionFilter.OnActionExecuted(ActionExecutedContext)"/>
        public void OnActionExecuted(ActionExecutedContext context) { }

        /// <inheritdoc cref="IActionFilter.OnActionExecuting(ActionExecutingContext)"/>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerDescriptor)
            {
                var actionAttributes = controllerDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowInReadOnlyAttribute), false);

                if (actionAttributes == null || !actionAttributes.Any())
                {
                    context.Result = new EmptyResult();
                    return;
                }
            }
        }
    }
}
