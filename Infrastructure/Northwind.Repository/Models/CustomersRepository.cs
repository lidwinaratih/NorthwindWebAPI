using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwind.Entities.Models;
using Northwind.Contracts.Interfaces;
using Northwind.Entities.Contexts;
using Microsoft.EntityFrameworkCore;
using Northwind.Entities.RequestFeatures;

namespace Northwind.Repository.Models
{
    public class CustomersRepository : RepositoryBase<Customer>, ICustomersRepository
    {
        public CustomersRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateCustomerAsync(Customer customer)
        {
            Create(customer);
        }

        public void DeleteCustomerAsync(Customer customer)
        {
            Delete(customer);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerAsync(string id, bool trackChanges) =>
            await FindByCondition(c => c.CustomerId.Equals(id), trackChanges).SingleOrDefaultAsync();

        public async Task<IEnumerable<Customer>> GetPaginationCustomerAsync(CustomerParameters customerParameters, bool trackChanges)
        {
            return await FindAll(trackChanges)
                            .OrderBy(c => c.CompanyName)
                            .Skip((customerParameters.PageNumber - 1) * customerParameters.PageSize)
                            .Take(customerParameters.PageSize)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetSearchCustomer(CustomerParameters customerParameters, bool trackChanges)
        {
            //cek parameter yg dikirim null atau ga
            if (string.IsNullOrWhiteSpace(customerParameters.SearchCompany))
            {
                return await FindAll(trackChanges).ToListAsync();
            }

            //Trim() = untuk menghapus white space
            var lowerCaseSearch = customerParameters.SearchCompany.Trim().ToLower();

            return await FindAll(trackChanges)
                .Where(c => c.CompanyName.ToLower().Contains(lowerCaseSearch))
                .Include(c => c.Orders) //Include = join tabel
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }

        public void UpdateCustomerAsync(Customer customer)
        {
            Update(customer);
        }

    }
}
