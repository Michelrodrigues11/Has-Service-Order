using AutoMapper;
using Moq;
using OsDsII.api.Dtos;
using OsDsII.api.Dtos.Customers;
using OsDsII.api.Exceptions;
using OsDsII.api.Models;
using OsDsII.api.Repository;
using OsDsII.api.Repository.CustomersRepository;
using OsDsII.api.Services.Customers;


namespace CalculadoraSalario.Tests
{
    public class CustomersServiceTests
    {
        private readonly Mock<ICustomersRepository> _mockCustomersRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomersService _service;
        private List<CustomerDto> _lista;

        public CustomersServiceTests()
        {
            _mockCustomersRepository = new Mock<ICustomersRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new CustomersService(_mockCustomersRepository.Object, _mockMapper.Object);
            _lista = new List<CustomerDto>();

        }


        [Fact]
        public async Task GetCustomerAsync_CustomerExists_ReturnsCustomerDto()
        {
            
            var customer = new Customer(1, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");
            var customerDto = new CustomerDto("Michel Thiago", "henriquemichel10@gmail.com", "984648829",null);
            
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(customer);

            _mockMapper.Setup(mapper => mapper.Map<CustomerDto>(customer))
                .Returns(customerDto);

           
            var result = await _service.GetCustomerAsync(1);

            
            Assert.NotNull(result);
            Assert.Equal(customerDto, result);
        }

        [Fact]
        public async Task GetCustomerAsync_CustomerNotFound_ThrowsNotFoundException()
        {
            
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCustomerAsync(1));
        }

        [Fact]
        public async Task CreateAsync_CustomerDoesNotExist_AddsCustomer()
        {
           
            var createCustomerDto = new CreateCustomerDto("Michel Thiago", new object(), "henriquemichel10@gmail.com", "984648829");
            var customer = new Customer(0, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");

            _mockMapper.Setup(m => m.Map<Customer>(createCustomerDto)).Returns(customer);
            _mockCustomersRepository.Setup(repo => repo.FindUserByEmailAsync(createCustomerDto.Email))
                .ReturnsAsync((Customer)null);
            _mockCustomersRepository.Setup(repo => repo.AddCustomerAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            
            await _service.CreateAsync(createCustomerDto);

           
            _mockCustomersRepository.Verify(repo => repo.AddCustomerAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CustomerAlreadyExists_ThrowsConflictException()
        {
            
            var createCustomerDto = new CreateCustomerDto("Michel Thiago", new object(), "henriquemichel10@gmail.com", "984648829");
            var existingCustomer = new Customer(1, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");
            var customer = new Customer(0, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");

            _mockMapper.Setup(m => m.Map<Customer>(createCustomerDto)).Returns(customer);
            _mockCustomersRepository.Setup(repo => repo.FindUserByEmailAsync(createCustomerDto.Email))
                .ReturnsAsync(existingCustomer);

            
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _service.CreateAsync(createCustomerDto));
            Assert.Equal("Customer already exists", exception.Message);
        }


        [Fact]
        public async Task UpdateAsync_CustomerExists_UpdatesCustomer()
        {
            
            var updateCustomerDto = new CreateCustomerDto("Michel Thiago Updated", "Michel Thiago Updated", "henriquemichel10update@gmail.com", "0987654321");
            var existingCustomer = new Customer(1, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingCustomer);

            
            await _service.UpdateAsync(1, updateCustomerDto);

            
            _mockCustomersRepository.Verify(repo => repo.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Once);
            Assert.Equal("Michel Thiago Updated", existingCustomer.Name);
            Assert.Equal("henriquemichel10update@gmail.com", existingCustomer.Email);
            Assert.Equal("0987654321", existingCustomer.Phone);
        }

        [Fact]
        public async Task UpdateAsync_CustomerNotFound_ThrowsNotFoundException()
        {
            
            var updateCustomerDto = new CreateCustomerDto("Michel Thiago Updated", "Michel Thiago Updated", "henriquemichel10update@gmail.com", "0987654321");
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(1, updateCustomerDto));
        }

        [Fact]
        public async Task DeleteAsync_CustomerExists_DeletesCustomer()
        {
            
            var existingCustomer = new Customer(1, "Michel Thiago", "henriquemichel10@gmail.com", "984648829");
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingCustomer);

            
            await _service.DeleteAsync(1);

            
            _mockCustomersRepository.Verify(repo => repo.DeleteCustomer(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CustomerNotFound_ThrowsNotFoundException()
        {
            
            _mockCustomersRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Customer)null);

            
            await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(1));
        }
    }
}