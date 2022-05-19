using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using trip_api.DataAccess;
using trip_api.Models;
using trip_api.Services;

namespace trip_api.Controllers
{
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        ICosmosConnection _connection;
        TripService _tripService;
        CustomerService _customerService;

        private readonly ILogger<TripController> _logger;

        public TripController(ICosmosConnection con, TripService ts, CustomerService cs, ILogger<TripController> logger)
        {
            _logger = logger;
            _connection = con;
            _tripService = ts;
            _customerService = cs;
        }
        
        [HttpGet]
        public ActionResult<List<Trip>> Get()
        {
            try
            {
                var response = _tripService.GetAllTrips();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpGet]
        [Authorize]
        [Route("/api/trip/{id}")]
        public ActionResult<Trip> TripById(string id)
        {
            try
            {
                var trip = _tripService.GetTripById(id);
                return Ok(trip);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("/api/trip/add")]
        public ActionResult<Trip> AddNewTrip([FromBody] Trip trip)
        {
            try
            {
                bool isCustomerExist = _customerService.CheckIfExist(new Customer() {LoginId = User.FindFirst(ClaimTypes.Name).Value });
                if (!isCustomerExist)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Customer Not found");
                }
                bool canAllow = _tripService.CheckOverlapping(trip);
                if (canAllow)
                {
                    Trip result = _tripService.CreateNewTrip(trip);
                    return Ok(result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Overlapping trip");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        [HttpPost]
        [Authorize]
        [Route("/api/trip/cancel")]
        public ActionResult<Trip> CancelTrip([FromBody] string id)
        {
            try
            {
                bool canAllow = _tripService.CanCancel(id);
                if (canAllow)
                {
                    Trip result = _tripService.CancelTrip(id);
                    return Ok(result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, "Can't cancel trip");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
