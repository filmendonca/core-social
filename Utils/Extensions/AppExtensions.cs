using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Models;

namespace Utils.Extensions
{
    public static class AppExtensions
    {
        private static readonly ILogger _logger;

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context => {

                    //Get uncaught exception
                    var contextFeature = context.Features?.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var exception = contextFeature.Error;

                        context.Response.ContentType = "text/html";

                        //filler code; improve later
                        if (exception is ArithmeticException)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            //log error

                            await context.Response.WriteAsync(new Error
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Bad Request."
                            }.ToString());
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            await context.Response.WriteAsync(new Error
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal Server Error. Please Try Again Later."
                            }.ToString());
                        }
                    }
                });
            });
        }
    }
}
