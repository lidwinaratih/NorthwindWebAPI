using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Northwind.Contracts;
using Northwind.Entities.DataTransferObject;
using Northwind.Entities.Models;
using Northwind.Entities.RequestFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindWebApi.Controllers
{
    [Route("api/Customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CustomersController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet,Authorize]
        public async Task<IActionResult> GetAllCustomerAsync()
        {
            var customer = await _repository.Customers.GetAllCustomerAsync(trackChanges: false);
            var customerDto = _mapper.Map<IEnumerable<CustomerDto>>(customer);
            return Ok(customerDto);
        }

        [HttpGet("{id}", Name = "CustomerById")]
        public async Task<IActionResult> GetCustomer(string id)
        {
            var customer = await _repository.Customers.GetCustomerAsync(id, false);

            if (customer == null)
            {
                _logger.LogInfo($"Customer with Id : {id} not found");
                return NotFound();
            }
            else
            {
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody]CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                _logger.LogError("Customer is null");
                return BadRequest("Customer is null");
            }

            //Object modelState digunakan untuk validasi data yang ditangkap oleh customerdto
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid modelstate customerdto");
                return UnprocessableEntity(ModelState);
            }

            var customerEntity = _mapper.Map<Customer>(customerDto);
            _repository.Customers.CreateCustomerAsync(customerEntity);

            await _repository.SaveAsync();

            var customerResult = _mapper.Map<Customer>(customerEntity);
            return CreatedAtRoute("CustomerById", new { id = customerResult.CustomerId }, customerResult);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var customer = await _repository.Customers.GetCustomerAsync(id, trackChanges: false);
            if (customer == null)
            {
                _logger.LogInfo($"Customer with id : {id} doesn't exist in database");
                return NotFound();
            }

            _repository.Customers.DeleteCustomerAsync(customer);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, [FromBody] CustomerUpdateDto customerDto)
        {
            if (customerDto == null)
            {
                _logger.LogError("Customer must not be null");
                return BadRequest("Customer must not be null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for customerdto object");
                return UnprocessableEntity(ModelState);
            }
            var customerEntity = await _repository.Customers.GetCustomerAsync(id, trackChanges: true);

            if (customerEntity == null)
            {
                _logger.LogError($"Company with id : {id} not found");
                return NotFound();
            }

            _mapper.Map(customerDto, customerEntity);
            //_repository.Customer.UpdateCustomer(customerEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateCustomer(string id, [FromBody]
                             JsonPatchDocument<CustomerUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError($"PatchDoc object sent is null");
                return BadRequest("PatchDoc object sent is null");
            }

            var customerEntity = await _repository.Customers.GetCustomerAsync(id, trackChanges: true);

            if (customerEntity == null)
            {
                _logger.LogError($"Customer with id : {id} not found");
                return NotFound();
            }

            var customerPatch = _mapper.Map<CustomerUpdateDto>(customerEntity);

            patchDoc.ApplyTo(customerPatch);

            TryValidateModel(customerPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for pathc document");
                return UnprocessableEntity();
            }

            _mapper.Map(customerPatch, customerEntity);

            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetCustomerPagination([FromQuery]CustomerParameters customerParameters)
        {
            var customerPage = await _repository.Customers.
                                GetPaginationCustomerAsync(customerParameters, trackChanges: false);

            var customerDto = _mapper.Map<IEnumerable<CustomerDto>>(customerPage);
            return Ok(customerDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetCustomerSearch([FromQuery] CustomerParameters customerParameters)
        {
            var customerSearch = await _repository.Customers.
                                GetSearchCustomer(customerParameters, trackChanges: false);

            var customerDto = _mapper.Map<IEnumerable<CustomerDto>>(customerSearch);
            return Ok(customerDto);
        }

        /*[HttpGet]
        public IActionResult GetCustomer()
        {
            try
            {
                var customer = _repository.Customers.GetAllCustomer(trackChanges: false);

                var customerDto = _mapper.Map<IEnumerable<CustomerDto>>(customer);

                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCustomer)} message : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }*/

        /*[HttpGet("{id}", Name = "CustomerById")]
        public IActionResult GetCustomer(string id)
        {
            var customer = _repository.Customers.GetCustomer(id, trackChanges: false);

            if (customer == null)
            {
                _logger.LogInfo($"Customer with Id : {id} doesn't exist");
                return NotFound();
            }
            else
            {
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                _logger.LogError("Customer object is null");
                return BadRequest("Customer object is null");
            }

            var customerEntity = _mapper.Map<Customer>(customerDto);
            _repository.Customers.CreateCustomer(customerEntity);
            _repository.Save();

            var customerResult = _mapper.Map<CustomerDto>(customerEntity);
            return CreatedAtRoute("CategoryById", new { id = customerResult.CustomerId }, customerResult);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(string id)
        {
            var customer = _repository.Customers.GetCustomer(id, trackChanges: false);

            if (customer == null)
            {
                _logger.LogInfo($"Customer with Id : {id} not found");
                return NotFound();
            }

            _repository.Customers.DeleteCustomer(customer);
            _repository.Save();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(string id, [FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                _logger.LogError($"Customer must not be null");
                return BadRequest("Customemr must not be null");
            }

            var customerEntity = _repository.Customers.GetCustomer(id, trackChanges: true);

            if(customerEntity == null)
            {
                _logger.LogInfo($"Customer with id : {id} not found");
                return NotFound();
            }

            _mapper.Map(customerDto, customerEntity);
            _repository.Customers.UpdateCustomerAsync(customerEntity);

            _repository.Save();
            return NoContent();
        }*/
    }
}
