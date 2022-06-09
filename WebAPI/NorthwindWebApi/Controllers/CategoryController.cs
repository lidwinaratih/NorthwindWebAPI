using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Contracts;
using System;
using System.Linq;
using Northwind.Entities.DataTransferObject;
using AutoMapper;
using System.Collections.Generic;
using Northwind.Entities.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Northwind.Entities.RequestFeatures;

namespace NorthwindWebApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CategoryController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoory()
        {
            var category = await _repository.Category.GetAllCategoryAsync(trackChanges: false);
            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(category);
            return Ok(categoryDto);
        }

        [HttpGet("{id}", Name = "CategoryById")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _repository.Category.GetCategoryAsync(id, false);

            if (id == null)
            {
                _logger.LogInfo($"Category with Id : {id} not found");
                return NotFound();
            }
            else
            {
                var categoryDto = _mapper.Map<Category>(category);
                return Ok(categoryDto);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogError("Category is null");
                return BadRequest("Category is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid ModelState categoryDto");
                return UnprocessableEntity(ModelState);
            }

            var categoryEntity = _mapper.Map<Category>(categoryDto);
            _repository.Category.CreateCategoryAsync(categoryEntity);

            await _repository.SaveAsync();

            var categoryResult = _mapper.Map<Category>(categoryEntity);
            return CreatedAtRoute("CategoryById", new { id = categoryResult.CategoryId }, categoryResult);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _repository.Category.GetCategoryAsync(id, trackChanges: false);

            if (id == null)
            {
                _logger.LogInfo($"Category with Id : {id} doesn't exist in database");
                return NotFound();
            }
            
            _repository.Category.DeleteCategoryAsync(category);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            if (categoryUpdateDto == null)
            {
                _logger.LogError("Category must not be null");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for categoryDto object");
                return UnprocessableEntity(ModelState);
            }

            var categoryEntity = await _repository.Category.GetCategoryAsync(id, trackChanges: true);

            if (categoryEntity == null)
            {
                _logger.LogError($"Category with Id : {id} not found");
                return NotFound();
            }

            _mapper.Map(categoryUpdateDto, categoryEntity);
            await _repository.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateCategory(int id, 
                        [FromBody] JsonPatchDocument<CategoryUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError($"PatchDoc object sent is null");
                return BadRequest("PatchDoc object sent is null");
            }

            var categoryEntity = await _repository.Category.GetCategoryAsync(id, trackChanges: true);

            if (categoryEntity == null)
            {
                _logger.LogError($"Category with Id : {id} not found");
                return NotFound();
            }

            var categoryPatch = _mapper.Map<CategoryUpdateDto>(categoryEntity);
            
            patchDoc.ApplyTo(categoryPatch);

            TryValidateModel(categoryPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for patch document");
                return UnprocessableEntity();
            }

            _mapper.Map(categoryPatch, categoryEntity);

            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> GetCategoryPagination([FromQuery] CategoryParameters categoryParameters)
        {
            var categoryPage = await _repository.Category.GetPaginationCategoryAsync(categoryParameters, trackChanges: false);

            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>> (categoryPage);

            return Ok(categoryDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetCategorySearch([FromQuery] CategoryParameters categoryParametes)
        {
            var categorySearch = await _repository.Category.GetSearchCategory(categoryParametes, trackChanges: false);

            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(categorySearch);

            return Ok(categoryDto);
        }

        /*[HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _repository.Category.GetAllCategory(trackChanges: false);

                //replace by categoryDto
                *//*var categoryDto = categories.Select(c => new CategoryDto
                {
                    Id = c.CategoryId,
                    categoryName = c.CategoryName,
                    description = c.Description
                }).ToList();*//*

                var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCategories)} message : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}",Name = "CategoryById")]
        public IActionResult GetCategory(int id)
        {
            var category = _repository.Category.GetCategory(id,trackChanges: false);
            if (category == null)
            {
                _logger.LogInfo($"Category with Id : {id} doesn't exist");
                return NotFound();
            }
            else
            {
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            }
        }

        [HttpPost] 
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogError("Category object is null");
                return BadRequest("Category object is null");
            }

            var categoryEntity = _mapper.Map<Category>(categoryDto);
            _repository.Category.CreateCategory(categoryEntity);
            _repository.Save();

            var categoryResult = _mapper.Map<CategoryDto>(categoryEntity);
            return CreatedAtRoute("CategoryById", new { id = categoryResult.categoryId }, categoryResult);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _repository.Category.GetCategory(id, trackChanges: false);
            if(category == null)
            {
                _logger.LogInfo($"Category with Id : {id} not found");
                return NotFound();
            }

            _repository.Category.DeleteCategory(category);
            _repository.Save();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                _logger.LogError($"Category must not be null");
                return BadRequest("Category must not be null");
            }

            var categoryEntity = _repository.Category.GetCategory(id, trackChanges: true);
            if(categoryEntity == null)
            {
                _logger.LogInfo($"Category with id : {id} not found");
                return NotFound();
            }

            _mapper.Map(categoryDto, categoryEntity);
            _repository.Category.UpdateCategory(categoryEntity);

            _repository.Save();
            return NoContent();
        }*/
    }
}
