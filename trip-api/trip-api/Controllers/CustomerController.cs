using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trip_api.DataAccess;
using trip_api.Models;
using trip_api.Services;

namespace trip_api.Controllers
{
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:scopes")]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        ICosmosConnection _connection;
        CustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICosmosConnection con, CustomerService cs, ILogger<CustomerController> logger)
        {
            _logger = logger;
            _connection = con;
            _customerService = cs;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<Customer>> Get()
        {
            try
            {
                return Ok(_customerService.GetAllCustomers());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("/api/customer/{id}")]
        public ActionResult<Customer> GetCustomerById(string id)
        {
            try
            {
                return Ok(_customerService.GetCustomerById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("/api/customer/add")]
        public ActionResult<Customer> AddNewCustomer([FromBody] Customer customer)
        {
            try
            {
                bool ifExist = _customerService.CheckIfExist(customer);
                if (!ifExist)
                {
                    Customer result = _customerService.CreateNewCustomer(customer);
                    return Ok(result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, "Already exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
