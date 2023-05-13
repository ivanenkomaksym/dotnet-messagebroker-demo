using System.Net;
using Microsoft.AspNetCore.Mvc;
using CustomerAPI.Entities;
using CustomerAPI.Repositories;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICustomerService customerService, ICustomerRepository repository, ILogger<CustomerController> logger)
        {
            CustomerService = customerService;
            Repository = repository;
            Logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Customer>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return Ok(await Repository.GetCustomers());
        }

        [HttpGet("{email}", Name = "GetCustomer")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Customer>> GetCustomerByEmail(string email)
        {
            var customer = await Repository.GetCustomerByEmail(email);
            if (customer == null)
            {
                Logger.LogError($"Customer with email: {email}, not found.");
                return NotFound();
            }

            return Ok(customer);
        }

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

            return CreatedAtRoute("GetCustomer", new { email = customer.Email }, customer);
        }

        [HttpDelete("{id}", Name = "DeleteCustomer")]
        [ProducesResponseType(typeof(Customer), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCustomerById(Guid id)
        {
            var result = await Repository.DeleteCustomer(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        private readonly ICustomerService CustomerService;
        private readonly ICustomerRepository Repository;
        private readonly ILogger<CustomerController> Logger;
    }
}
