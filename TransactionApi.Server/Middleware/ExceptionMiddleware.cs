using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TransactionApi.Server.Exceptions;

namespace TransactionApi.Server.Middleware
{
    public class ErrorDetails
    {
        [JsonProperty("status")]
        public int StatusCode { get; set; }

        [JsonProperty("messages")]
        public string[] Messages { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
    
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        //should be refactored to stringresources
        private const string GenericExceptionMessage = "Something went wrong.Please check logs";
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = string.Empty;
            switch (exception)
            {
                case ValidationException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case FormatNotSupportedException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case InvalidFileDataException ex:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.ToJson();
                    break;
                case DbUpdateException ex:
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = GenericExceptionMessage;
                    break;
            }
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Messages = new string[] { string.IsNullOrEmpty(message) ? exception.Message : message }
            }.ToString());
        }
    }
}