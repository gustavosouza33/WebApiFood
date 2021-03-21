using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiFood.Data;
using AuthenticationPlugin;
using Microsoft.AspNetCore.Authorization;
using WebApiFood.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;        
using System.IdentityModel.Tokens.Jwt;


namespace WebApiFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private DeliveryDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public AccountsController(DeliveryDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbContext = dbContext;
        }

        // GET: api/Accounts
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_dbContext.Users); //Retorna todos os usuários registrados.
        }

        // GET: api/Accounts/5
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_dbContext.Users.Find(id)); //Retorna o usuário pelo ID informado.
        }
      
        [HttpPost("[action]")]
        [AllowAnonymous] //Permite o acesso de usuários não autenticados a se registrar.
        public async Task<IActionResult> Register(User user)
        {
            var userWithSameEmail = _dbContext.Users.SingleOrDefault(u => u.Email == user.Email); //Verifica se ja existe o mesmo email cadastrado no banco de dados
            if (userWithSameEmail != null) return StatusCode(StatusCodes.Status400BadRequest);

            //Cria um objeto User com os dados informados e realiza a proteção da senha por Hash.
            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "User"
            };
            //Adiciona e salva o novo usuário ao banco de dados e retorna o StatusCode201Created.
            _dbContext.Users.Add(userObj);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Login(User user)
        {
            var userEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email); //Verifica se existe o email no banco de dados
            if (userEmail == null) return StatusCode(StatusCodes.Status401Unauthorized);
            var hashedPassword = userEmail.Password;

            if (!SecurePasswordHasherHelper.Verify(user.Password, hashedPassword)) //Verifica se a senha informada é igual a senha do banco de dados protegida por hash
            {
                return StatusCode(StatusCodes.Status401Unauthorized); // retorna Status401Unauthorized quando a senha for diferente
            }
            //Cria uma objeto claim para transmitir as informações com segurança. 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, userEmail.Role)
            };
            //Retorna o objeto que contém o acesso token e todas as outras informações para o usuário.
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                token_type = token.TokenType,
                user_Id = userEmail.Id,
                user_name = userEmail.Name,
                expires_in = token.ExpiresIn,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                roles_user = userEmail.Role,
            });
        }
        [HttpPut("{id}")]
        [AllowAnonymous]
        public IActionResult Put(int id, [FromBody] User user)
        {
            var entity = _dbContext.Users.Find(id);
            if (entity == null)
            {
                return NotFound("Usuário não enconrado !");
            }
            entity.Name = user.Name;
            entity.Email = user.Email;
            entity.Password = SecurePasswordHasherHelper.Hash(user.Password);
            //Atualiza e salva o novo usuário ao banco de dados e retorna o StatusCode201Created.
            _dbContext.SaveChanges();
            return Ok("Usuário atualizado com sucesso !");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete (int id)
        {
            var user = _dbContext.Users.Find(id); //Verifica se existe usuário com o ID informado.
            if (user == null)
            {
                return NotFound("Não existe usuário com esse ID.");
            }
            _dbContext.Remove(user);
            _dbContext.SaveChanges();
            return Ok("Usuário deletado com sucesso !");
        }
    }
}
