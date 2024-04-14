﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Data;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly AppDbContext _context;
        public WeatherController(AppDbContext context)
        {
            _context = context;
            _weatherService = new WeatherService(context);
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }

        [HttpGet("weather")]
        public IActionResult GetWeather([FromQuery]double latitude, [FromQuery]double longitude)
        {
            var rweather = _weatherService.GetWeather(latitude, longitude);
            return Ok(rweather);
        }

        [HttpGet("weather")]
        public IActionResult GetWeatherForcast([FromQuery]double latitude, [FromQuery]double longitude)
        {
            var rweather = _weatherService.GetWeatherForcast5Days(latitude, longitude);
            return Ok(rweather);
        }
    }
}
