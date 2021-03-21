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

namespace WebApiFood.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
       
        public class ProductsController : ControllerBase
        {
            private DeliveryDbContext _dbContext;
            public ProductsController(DeliveryDbContext dbContext)
            {
                _dbContext = dbContext;
            }
            // GET: api/Products
            [Authorize(Roles = "Admin,User")]
            [HttpGet]
            public IActionResult Get() //Retorna todos os produtos
            {
                return Ok(_dbContext.Products);
            }

            // GET: api/Products/5
            [HttpGet("{id}")]
            public IActionResult Get(int id) //Retorna um produto pelo ID informado
            {
                return Ok(_dbContext.Products.Find(id));
            }

            // GET: api/Products/ProductsByCategory/5
            [HttpGet("[action]/{categoryId}")]
            public IActionResult ProductsByCategory(int categoryId) //Retorna os produtos pelo ID da categoria informado
            {
                var products = from v in _dbContext.Products
                               where v.CategoryId == categoryId
                               select new
                               {
                                   Id = v.Id,
                                   Name = v.Name,
                                   Price = v.Price,
                                   Detail = v.Detail,
                                   CategoryId = v.CategoryId,
                                   ImageUrl = v.ImageUrl
                               };

                return Ok(products);
            }

            // GET: api/Products/ProductsByRestaurant/5
            [HttpGet("[action]/{restaurantId}")]
            public IActionResult ProductsByRestaurant(int restaurantId) //Retorna os produtos pelo ID do restaurante informado
            {
                var products = from v in _dbContext.Products
                               where v.RestaurantId == restaurantId
                               select new
                               {
                                   Id = v.Id,
                                   Name = v.Name,
                                   Price = v.Price,
                                   Detail = v.Detail,
                                   CategoryId = v.CategoryId,
                                   ImageUrl = v.ImageUrl,
                                   RestaurantId = v.RestaurantId

                               };

                return Ok(products);
            }
            // GET: api/Products/PopularProducts
            [HttpGet("[action]")]
            public IActionResult PopularProducts() //Retorna os produtos populares
            {
                var products = from v in _dbContext.Products
                               where v.IsPopularProduct == true
                               select new
                               {
                                   Id = v.Id,
                                   Name = v.Name,
                                   Price = v.Price,
                                   ImageUrl = v.ImageUrl
                               };

                return Ok(products);
            }


            // POST: api/Products
            [Authorize(Roles = "Admin")]
            [HttpPost]

            public IActionResult Post([FromBody] Product product) //Adiciona um novo produto
            {

                if (product==null)
                {
                    return BadRequest("Produto nulo");
                }
            
                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();
                 return StatusCode(StatusCodes.Status201Created);
                
            }

            // PUT: api/Products/5
            [Authorize(Roles = "Admin")]
            [HttpPut("{id}")]
            public IActionResult Put(int id, [FromBody] Product product) //Altera um produto pelo ID do produto informado
            {
                var entity = _dbContext.Products.Find(id);
                if (entity == null)
                {
                    return NotFound("Produto não enconrado !");
                }

                else
                {     
                   
                    entity.Name = product.Name;
                    entity.ImageUrl = product.ImageUrl;
                    entity.Price = product.Price;
                    entity.Detail = product.Detail;
                    entity.IsPopularProduct = product.IsPopularProduct;
                    entity.CategoryId = product.CategoryId;
                    _dbContext.SaveChanges();
                    return Ok("Produto atualizado com sucesso !");
                }
            }

            // DELETE: api/ApiWithActions/5
            [Authorize(Roles = "Admin")]
            [HttpDelete("{id}")]
            public IActionResult Delete(int id) //Deleta um produto pelo ID do produto informado
            {
                var product = _dbContext.Products.Find(id);
                if (product == null)
                {
                    return NotFound("Não existe produto com esse ID.");
                }
                else
                {
                    _dbContext.Products.Remove(product);
                    _dbContext.SaveChanges();
                    return Ok("Produto deletado !");
                }
            }
        }
}
