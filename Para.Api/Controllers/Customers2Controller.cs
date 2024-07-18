using Microsoft.AspNetCore.Mvc;
using Para.Data.Domain;
using Para.Data.UnitOfWork;

namespace Para.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Customers2Controller : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public Customers2Controller(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<List<Customer>> Get()
        {
            //Include Kullanımı GetAll fonksiyonu içerisine parametre alarak modelleri dinamik olarak dahil edebilir.
            var entityList = await unitOfWork.CustomerRepository.GetAll(x => x.CustomerAddresses, x => x.CustomerPhones, x => x.CustomerDetail);
            return entityList;
        }

        [HttpGet("{customerId}")]
        public async Task<Customer> Get(long customerId)
        {
            var entity = await unitOfWork.CustomerRepository.GetById(customerId);
            return entity;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer value)
        {
            //CustomerNumber unique kontrolü
            var existingCustomer = await unitOfWork.CustomerRepository.Where(c => c.CustomerNumber == value.CustomerNumber);
            if (existingCustomer.Any())
            {
                return BadRequest("Customer with the same CustomerNumber already exists.");
            }
            //Email unique kontrolü
            var existingEmail =await unitOfWork.CustomerRepository.Where(c => c.Email == value.Email);
            if (existingCustomer.Any())
            {
                return BadRequest("Customer with the same Email already exists.");
            }

            await unitOfWork.CustomerRepository.Insert(value);
            await unitOfWork.Complete();
            return Ok();
    
        }

        [HttpPut("{customerId}")]
        public async Task Put(long customerId, [FromBody] Customer value)
        {
            await unitOfWork.CustomerRepository.Update(value);
            await unitOfWork.Complete();
        }

        [HttpDelete("{customerId}")]
        public async Task Delete(long customerId)
        {
            await unitOfWork.CustomerRepository.Delete(customerId);
            await unitOfWork.Complete();
        }
        //Where metodunun kullanımı. FirstName değişkenine göre GenericRepository tarafında yazılmış olan Where metodu filtreleme yapmaktadır.
        [HttpGet("FilterWithWhere")]
        public async Task<List<Customer>> GetFilteredCustomers([FromQuery] string name)
        {
            var filteredCustomers = await unitOfWork.CustomerRepository.Where(
                x => x.FirstName.Contains(name),
                x => x.CustomerAddresses,
                x => x.CustomerPhones,
                x => x.CustomerDetail
            );
            return filteredCustomers;
        }
    }
}