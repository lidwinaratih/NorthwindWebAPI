﻿using Northwind.Entities.Models;
using Northwind.Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Contracts.Interfaces
{
    public interface ICustomersRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync(bool trackChanges);
        Task<Customer> GetCustomerAsync(string id, bool trackChanges);
        void CreateCustomerAsync(Customer customer);
        void DeleteCustomerAsync(Customer customer);
        void UpdateCustomerAsync(Customer customer);

        Task<IEnumerable<Customer>> GetPaginationCustomerAsync(CustomerParameters customerParameters, bool trackChanges);

        Task<IEnumerable<Customer>> GetSearchCustomer(CustomerParameters customerParameters, bool trackChanges);
    }
}
