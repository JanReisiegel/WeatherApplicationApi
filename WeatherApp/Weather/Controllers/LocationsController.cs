﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Weather.Models;
using Weather.MyExceptions;
using Weather.Services;

namespace Weather.Controllers
{
    [Route("api/Locations")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly LocationServices _locationServices = new LocationServices();
        private readonly LocationTransformation _locationTransformation = new LocationTransformation();

        [HttpGet]
        public async Task<IActionResult> GetSavedLocation([FromHeader]string? userToken, [FromQuery]string customName)
        {
            if (userToken == null || !(await UserServices.GetAuthenticate(UserServices.GetClaims(userToken)["email"])))
            {
                return Unauthorized("You must be logged in");
            }
            var user = await JsonFileService.GetUserAsync(UserServices.GetClaims(userToken)["email"]);
            Location location; 
            try
            {
                location = await _locationServices.GetLocation(user, customName);
            } 
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            return location == null ? StatusCode(StatusCodes.Status204NoContent, "Cannot load data from database, please contact admin") : Ok(location);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetSavedLocations([FromHeader] string? userToken)
        {
            if (userToken == null || !(await UserServices.GetAuthenticate(UserServices.GetClaims(userToken)["email"])))
            {
                return Unauthorized("You must be logged in");
            }
            var user = await JsonFileService.GetUserAsync(UserServices.GetClaims(userToken)["email"]);
            var locations = await _locationServices.GetAllLocations(user);
            if (locations == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Cannot load data from database, please contact admin");
            }
            return Ok(locations);
        }
        [HttpPost]

        public async Task<IActionResult> SaveLocation([FromHeader]string? userToken, [FromQuery]string cityName, [FromQuery]string country, [FromQuery] string customName)
        {
            if (userToken == null || !(await UserServices.GetAuthenticate(UserServices.GetClaims(userToken)["email"])))
            {
                return Unauthorized("You must be logged in");
            }
            var user = await JsonFileService.GetUserAsync(UserServices.GetClaims(userToken)["email"]);
            Location location = _locationServices.StoreLocation(cityName, country, customName, user).Result;
            return Ok(location);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteLocation([FromHeader]string? userToken, [FromQuery]string customName)
        {
            if (userToken == null || !(await UserServices.GetAuthenticate(UserServices.GetClaims(userToken)["email"])))
            {
                return Unauthorized("You must be logged in");
            }
            var user = await JsonFileService.GetUserAsync(UserServices.GetClaims(userToken)["email"]);
            var location = await _locationServices.GetLocation(user, customName);
            if (location == null)
            {
                return NotFound("Location not found");
            }
            var index = user.SavedLocations.FindIndex(l => l.CustomName == customName);
            user.SavedLocations.RemoveAt(index);
            var result = await JsonFileService.UpdateUserAsync(user);
            if (result.Succeeded)
            {
                return Ok("Location deleted");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Cannot store data in database, please contact admin");
        }

        [HttpGet("coords")]
        public async Task<IActionResult> GetLocation([FromQuery]double latitude, [FromQuery]double longitude)
        {
            Location location;
            try
            {
                location = await _locationTransformation.GetLocationFromCoords(latitude, longitude);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            return location == null ? StatusCode(StatusCodes.Status204NoContent, "Cannot load data from database, please contact admin") : Ok(location);
        }
    }
}
