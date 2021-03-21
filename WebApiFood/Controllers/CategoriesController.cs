using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApiFood.Data;
using WebApiFood.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private DeliveryDbContext _dbContext;
        public CategoriesController(DeliveryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: api/Categories
        [HttpGet]
      
        public IActionResult Get() //Retorna todas as categorias
        {
            var categories = from c in _dbContext.Categories
                             select new
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 ImageUrl = c.ImageUrl
                             };


            return Ok(categories);   
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
       
        public IActionResult Get(int id) //Retorna uma categoria pelo ID informado.
        {
            var category = (from c in _dbContext.Categories
                            where c.Id == id
                            select new
                            {
                                Id = c.Id,
                                Name = c.Name,
                                ImageUrl = c.ImageUrl
                            }).FirstOrDefault();


            return Ok(category); 

        }

        // POST: api/Categories
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Category category) //Adiciona uma categoria
        {
            var newCategory = _dbContext.Categories.SingleOrDefault(u => u.Name == category.Name); //Verifica se ja existe o mesmo email cadastrado no banco de dados
            if (newCategory != null) return StatusCode(StatusCodes.Status400BadRequest);

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
            
        }
       
        // PUT: api/Categories/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Category category)  //Altera uma categoria pelo ID informado
        {
            var entity = _dbContext.Categories.Find(id);
            if (entity == null)
            {
                return NotFound("Não existe categoria com esse ID.");
            }       

            entity.Name = category.Name;
            entity.ImageUrl = category.ImageUrl;
            _dbContext.SaveChanges();
            return Ok("Categoria atualizada com sucesso !");
        }

        // DELETE: api/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) //Deleta uma categoria pelo ID informado
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound("Não existe categoria com esse ID.");
            }
            else
            {
                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();
                return Ok("Categoria deletada");
            }
        }
    }
}
