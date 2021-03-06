﻿using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Repositories;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoConfigController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromServices] IMyRepository repository)
        {
            return Ok(repository.GetConnectionString());
        }
    }
}