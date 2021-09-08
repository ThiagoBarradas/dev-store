using System.Threading;
using System.Threading.Tasks;
using DevStore.Core.Messages;
using DevStore.Customers.API.Application.Events;
using DevStore.Customers.API.Models;
using FluentValidation.Results;
using MediatR;

namespace DevStore.Customers.API.Application.Commands
{
    public class CustomerCommandHandler : CommandHandler,
        IRequestHandler<NewCustomerCommand, ValidationResult>,
        IRequestHandler<AddAddressCommand, ValidationResult>
    {
        private readonly ICustomerRepository _customereRepository;

        public CustomerCommandHandler(ICustomerRepository customereRepository)
        {
            _customereRepository = customereRepository;
        }

        public async Task<ValidationResult> Handle(NewCustomerCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var customer = new Customer(message.Id, message.Name, message.Email, message.SocialNumber);

            var customerExist = await _customereRepository.GetBySocialNumber(customer.SocialNumber);

            if (customerExist != null)
            {
                AddError("Already has this social number.");
                return ValidationResult;
            }

            _customereRepository.Add(customer);

            customer.AddEvent(new NewCustomerAddedEvent(message.Id, message.Name, message.Email, message.SocialNumber));

            return await PersistData(_customereRepository.UnitOfWork);
        }

        public async Task<ValidationResult> Handle(AddAddressCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var endereco = new Address(message.StreetAddress, message.BuildingNumber, message.SecondaryAddress, message.Neighborhood, message.ZipCode, message.City, message.State, message.CustomerId);
            _customereRepository.AddAddress(endereco);

            return await PersistData(_customereRepository.UnitOfWork);
        }
    }
}