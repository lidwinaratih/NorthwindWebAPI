﻿using System;
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
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateCategoryAsync(Category category)
        {
            Create(category);
        }

        public void DeleteCategoryAsync(Category category)
        {
            Delete(category);
        }

        public async Task<IEnumerable<Category>> GetAllCategoryAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                    .OrderBy(c => c.CategoryId)
                    .ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(int id, bool trackChanges) =>
            await FindByCondition(c => c.CategoryId.Equals(id), trackChanges).SingleOrDefaultAsync();

        public async Task<IEnumerable<Category>> GetPaginationCategoryAsync(CategoryParameters categoryParameters, bool trackChanges)
        {
            return await FindAll(trackChanges)
                            .OrderBy(c => c.CategoryName)
                            .Skip((categoryParameters.PageNumber - 1) * categoryParameters.PageSize)
                            .Take(categoryParameters.PageSize)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSearchCategory(CategoryParameters categoryParameters, bool trackChanges)
        {
            if (string.IsNullOrWhiteSpace(categoryParameters.SearchCategoryName))
            {
                return await FindAll(trackChanges).ToListAsync();
            }

            var lowerCaseSearch = categoryParameters.SearchCategoryName.Trim().ToLower();

            return await FindAll(trackChanges)
                .Where(c => c.CategoryName.ToLower().Contains(lowerCaseSearch))
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryId)
                .ToListAsync();
        }

        public void UpdateCategoryAsync(Category category)
        {
            Update(category);
        }

        /*public void CreateCategory(Category category)
        {
            Create(category);
        }

        public void DeleteCategory(Category category)
        {
            Delete(category);
        }

        public IEnumerable<Category> GetAllCategory(bool trackChanges) =>
            FindAll(trackChanges)
                .OrderBy(c => c.CategoryName)
                .ToList();

        public Category GetCategory(int id, bool trackChanges) =>
            FindByCondition(c => c.CategoryId.Equals(id), trackChanges).SingleOrDefault();

        public void UpdateCategory(Category category)
        {
            Update(category);
        }*/
    }
}
