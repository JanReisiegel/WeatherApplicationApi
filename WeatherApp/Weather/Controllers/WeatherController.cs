﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Weather.Models;
using Weather.Services;
using Weather.ViewModels;

namespace Weather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherServices _weatherServices;

        public WeatherController()
        {
            _weatherServices = new WeatherServices();
        }

        [HttpGet("actual")]
        public async Task<ActionResult<MyWeatherInfo>> GetActualWeather([FromQuery] string cityName)
        {
            MyWeatherInfo weather;
            try { weather = await _weatherServices.GetActualWeather(cityName); }
            catch (LocationException e) { return NotFound(e.Message); }
            catch (Exception e) { return StatusCode(StatusCodes.Status500InternalServerError, e.Message); }
            if (weather == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Cannot store data in database, please contact admin");
            }
            return Ok(weather);
        }
        [HttpGet("forecast")]
        public async Task<ActionResult<MyWeatherForecast>> GetForecast([FromQuery] string cityName)
        {
            MyWeatherForecast weather;
            try
            {
                weather = await _weatherServices.GetWeatherForecast5Days(cityName);
            }
            catch (LocationException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            
            if (weather == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Cannot store data in database, please contact admin");
            }
            return Ok(weather);
        }
    }
}
