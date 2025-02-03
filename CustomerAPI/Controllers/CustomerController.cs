using System.Net;
using Common.Models;
using CustomerAPI.Messaging;
using CustomerAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAPI.Controllers
{
    /// <summary>
    /// Customers controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        /// <summary>
        /// Initializes controller.
        /// </summary>
        /// <param name="customerService">Used for publishing messages.</param>
        /// <param name="repository">Repository for customers data.</param>
        /// <param name="logger">Used for logging.</param>
        public CustomerController(ICustomerPublisher customerService, ICustomerRepository repository, ILogger<CustomerController> logger)
        {
            CustomerService = customerService;
            Repository = repository;
            Logger = logger;
        }

        /// <summary>
        /// Get all customers.
        /// </summary>
        /// <returns>List of customers.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Customer>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return Ok(await Repository.GetCustomers());
        }

        /// <summary>
        /// Get customer by id.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>Customer if found.</returns>
        [HttpGet("{customerId}", Name = "GetCustomerById")]
        [ProducesResponseType(typeof(IEnumerable<Customer>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer(Guid customerId)
        {
            var customer = await Repository.GetCustomerById(customerId);
            if (customer == null)
            {
                Logger.LogError($"Customer with id: {customerId}, not found.");
                return NotFound($"Customer with id: {customerId}, not found.");
            }

            return Ok(customer);
        }

        /// <summary>
        /// Authenticates user by email and password.
        /// </summary>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>Customer if authenticated.</returns>
        [HttpPost("Authenticate")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Customer>> Authenticate(string email, string password)
        {
            var customer = await Repository.Authenticate(email, password);
            if (customer == null)
            {
                Logger.LogError($"Customer with email: {email} and password: {password}, not found.");
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// Create new customer.
        /// </summary>
        /// <param name="customer">Customer.</param>
        /// <returns>True if created.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> CreateCustomer([FromBody] Customer customer)
        {
            var result = await Repository.CreateCustomer(customer);
            if (result == null)
            {
                Logger.LogError($"Customer with email: {customer.Email}, already exist.");
                return Conflict();
            }

            await CustomerService.CreateCustomer(customer);

            return CreatedAtRoute("GetCustomerById", new { customerId = customer.Id }, customer);
        }

        /// <summary>
        /// Update existing customer.
        /// </summary>
        /// <param name="customer">Customer to be updated/</param>
        /// <returns>True if updated.</returns>
        [HttpPut]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer customer)
        {
            return Ok(await Repository.UpdateCustomer(customer));
        }

        /// <summary>
        /// Delete customer by id.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns>NoContent if deleted.</returns>
        [HttpDelete("{id}", Name = "DeleteCustomer")]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCustomerById(Guid customerId)
        {
            var result = await Repository.DeleteCustomer(customerId);
            if (!result)
                return NotFound();

            return Ok();
        }

        private readonly ICustomerPublisher CustomerService;
        private readonly ICustomerRepository Repository;
        private readonly ILogger<CustomerController> Logger;
    }
}