using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PasswordManagerApp.Models;
//using PasswordManagerApp.Models;

namespace PasswordManagerApp.Controllers
{   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
        public TempController()
        {
        }

        [HttpGet("")]
        public  string GetTModels()
        {
            // TODO: Your code here

            string output = JsonConvert.SerializeObject(new {Imie="Przmek",Nazwisko="Rodzik"});
            return output;
           
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetTModelById(int id)
        {
            // TODO: Your code here
            await Task.Yield();

            return null;
        }

        [HttpPost("")]
        public async Task<ActionResult<User>> PostTModel(User model)
        {
            // TODO: Your code here
            await Task.Yield();

            return null;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTModel(int id, User model)
        {
            // TODO: Your code here
            await Task.Yield();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteTModelById(int id)
        {
            // TODO: Your code here
            await Task.Yield();

            return null;
        }
    }
}