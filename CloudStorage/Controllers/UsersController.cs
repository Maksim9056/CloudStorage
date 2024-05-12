using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudStorageClass.CloudStorageModel;
using CloudStorageWebAPI.Data;

namespace CloudStorageWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DB _context;
        string path = AppDomain.CurrentDomain.BaseDirectory;

        public UsersController(DB context)
        {
            _context = context;
        }


        [HttpGet("{Email}  ,{Password}")]
        public async Task<ActionResult<User>> GetUserAuthorization(string Email, string Password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email == Email && u.Password == Password);

                if (user == null)
                {
                    return NotFound();
                }
                return user;

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //// GET: api/Users
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return user;

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //// PUT: api/Users/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(int id, User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
              var usersq =  await _context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
                if (usersq == null)
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound("Такой пользователь уже есть с таким именем");


                }

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                var Context = _context.Filles.Where(u => u.UserId == user.Id).ToList();
                foreach (var context in Context)
                {
                    System.IO.File.Delete(context.StoragePath);
                }
                Directory.Delete(path + $"\\{user.Name}");
            }catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
