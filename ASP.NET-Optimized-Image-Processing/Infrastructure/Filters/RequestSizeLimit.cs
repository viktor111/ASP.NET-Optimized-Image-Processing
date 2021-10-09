﻿using ASP.NET_Optimized_Image_Processing.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Optimized_Image_Processing.Infrastructure.Filters
{
    public class RequestSizeLimitChecker : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestSize = context.HttpContext.Request.ContentLength;

            if(requestSize.HasValue)
            {
                if(requestSize.Value > FileSizeCalculator.MegaBytes(100)){
                    var result = new ViewResult();
                    result.StatusCode = 413;
                    result.ViewName = "~/Views/Images/Failed.cshtml";

                    context.Result = result;
                    return;
                }
            }

            await next();
        }
    }
}
