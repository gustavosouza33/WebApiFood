using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFood.Data;
using WebApiFood.Models;

namespace WebApiFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private DeliveryDbContext _dbContext;
        public OrdersController(DeliveryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      
        // GET: api/Orders/PendingOrders
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult PendingOrders() //Retorna os pedidos em aberto.
        {
            var orders = _dbContext.Orders.Where(order => order.IsOrderCompleted == false);
            return Ok(orders);
        }

        // GET: api/Orders/CompletedOrders
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult CompletedOrders() //Retorna os pedidos finalizados.
        {
            var orders = _dbContext.Orders.Where(order => order.IsOrderCompleted == true);
            return Ok(orders);
        }

        // GET: api/Orders/OrderDetails/5
        [HttpGet("[action]/{orderId}")]
        public IActionResult OrderDetails(int orderId) //Retorna os detalhes do pedido pelo ID do pedido informado.
        {

            var orders = _dbContext.Orders.Where(order => order.Id == orderId)
                   .Include(order => order.OrderDetails)
                   .ThenInclude(product => product.Product);

            return Ok(orders);
        }


        // GET: api/Orders/OrdersCount
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public IActionResult OrdersCount() //Retorna a quantidade de pedidos em aberto
        {
            var orders = (from order in _dbContext.Orders
                          where order.IsOrderCompleted == false
                          select order.IsOrderCompleted).Count();
            return Ok(new { PendingOrders = orders });
        }


        // GET: api/Orders/OrdersByUser/5
        [HttpGet("[action]/{userId}")]
        public IActionResult OrdersByUser(int userId) //Retorna os pedidos pelo ID de um usuário informado
        {
            var orders = _dbContext.Orders.Where(order => order.UserId == userId).OrderByDescending(o => o.OrderPlaced);
            return Ok(orders);
        }

        // POST: api/Orders
        [HttpPost]
        public IActionResult Post([FromBody] Order order) //Cria um novo pedido
        {
            order.IsOrderCompleted = false;
            order.OrderPlaced = DateTime.Now;
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            var shoppingCartItems = _dbContext.ShoppingCartItems.Where(cart => cart.CustomerId == order.UserId);
            foreach (var item in shoppingCartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    Price = item.Price,
                    TotalAmount = item.TotalAmount,
                    Qty = item.Qty,
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                };
                _dbContext.OrderDetails.Add(orderDetail);
            }

            _dbContext.SaveChanges();
            _dbContext.ShoppingCartItems.RemoveRange(shoppingCartItems);
            _dbContext.SaveChanges();

            return Ok(new { OrderId = order.Id });
        }


        // PUT: api/Orders/MarkOrderComplete/5
        [Authorize(Roles = "Admin")]
        [HttpPut("[action]/{orderId}")]
        public IActionResult MarkOrderComplete(int orderId)  //Altera o pedido e marca como finalizado
        {
            var entity = _dbContext.Orders.Find(orderId);
            if (entity == null)
            {
                return NotFound("Nenhum pedido encontrado com esse ID.");
            }
            else
            {
                entity.IsOrderCompleted = true;
                _dbContext.SaveChanges();
                return Ok("Pedido finalizado.");
            }
        }

        //DELETE: api/Orders/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) //Deleta um pedido pelo ID do pedido informado
        {
            var order = _dbContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Não existe pedido com esse ID !");
            }
            _dbContext.Remove(order);
            _dbContext.SaveChanges();
            return Ok("Pedido deletado com sucesso !");
        }
    }
}
