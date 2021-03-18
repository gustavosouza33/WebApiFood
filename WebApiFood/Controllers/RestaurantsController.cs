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

    public class RestaurantsController : ControllerBase
    {
        private DeliveryDbContext _dbContext;
        public RestaurantsController(DeliveryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        // GET: api/Restaurants
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.Restaurants);
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var restaurant = (from c in _dbContext.Restaurants
                              where c.Id == id
                              select new
                              {
                                  Id = c.Id,
                                  Name = c.Name,
                                  ImageUrl = c.ImageUrl
                              }).FirstOrDefault();


            return Ok(restaurant);

        }

        // POST: api/Restaurants
        [Authorize(Roles = "Admin")]
        [HttpPost]

        public IActionResult Post([FromBody] Restaurant restaurant)
        {
           var NameRestaurant= _dbContext.Restaurants.SingleOrDefault(u => u.Name == restaurant.Name);
            if (NameRestaurant != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest); //Já possui restaurante com esse nome.
            }
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
            
        }

        // PUT: api/Restaurants/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Restaurant restaurant )
        {
            var entity = _dbContext.Restaurants.Find(id);
            if (entity == null)
            {
                return NotFound("Restaurante não enconrado !");
            }          
            else
            {              
                entity.Name = restaurant.Name;
                entity.ImageUrl = restaurant.ImageUrl;
                _dbContext.SaveChanges();
                return Ok("Restaurante atualizado com sucesso !");
            }
        }

        // DELETE: api/Restaurants/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var restaurant = _dbContext.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound("Não existe restaurante com esse ID.");
            }
            else
            {
                _dbContext.Restaurants.Remove(restaurant);
                _dbContext.SaveChanges();
                return Ok("Restaurante deletado !");
            }
        }
    }
}