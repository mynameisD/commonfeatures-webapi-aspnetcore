using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebApiDemo.Middlewares;
using WebApiDemo.Models;
using Xunit;

namespace UnitTestsDemo
{
    public class CustomExceptionMiddlewareTests
    {
        [Fact]
        public async Task WhenAGenericExceptionIsRaised_CustomExceptionMiddlewareShouldHandleItToProperErrorResponseAndLoggerCalled()
        {
            // Arrange
            var loggerMock = Substitute.For<ILogger<CustomExceptionMiddleware>>();
            RequestDelegate requestDelegate = (innerHttpContext) =>
            {
                throw new Exception("Oooops error!");
            };
            var middleware = new CustomExceptionMiddleware(requestDelegate, loggerMock);

            // Use DefaultHttpContext insteadof mocking HttpContext
            var context = new DefaultHttpContext();

            // Initialize response body
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.Invoke(context);

            //Assert
            // set the position to beginning before reading
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            // Read the response
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<Error>(streamText);

            
            objResponse
            .Should()
            .BeEquivalentTo(new { Message = "Unhandled error", Errors = new List<string>() , Code = "00009" });

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.InternalServerError);

            context.Response.ContentType
            .Should()
            .Be("application/json");

            loggerMock.Received(1);
        }
    }
}
