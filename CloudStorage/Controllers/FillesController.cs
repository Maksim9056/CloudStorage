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
    public class FillesController : ControllerBase
    {
        private readonly DB _context;
        string path = AppDomain.CurrentDomain.BaseDirectory;

        public FillesController(DB context)
        {
            _context = context;
        }

        //// GET: api/Filles
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Filles>>> GetGroupsChats()
        //{
        //    return await _context.GroupsChats.Where.ToListAsync();
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Filles>>> GetGroupsChats(int id)
        {
            try
            {
                return await _context.Filles.Where(u => u.UserId == id).ToListAsync();

            }
            catch(Exception ex) 
            {
                return NotFound();
            }
        }
        //// GET: api/Filles/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Filles>> GetFilles(int id)
        //{
        //    var filles = await _context.Filles.FindAsync(id);

        //    if (filles == null)
        //    {
        //        return NotFound();
        //    }

        //    return filles;
        //}

        //// PUT: api/Filles/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutFilles(int id, Filles filles)
        //{
        //    if (id != filles.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(filles).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FillesExists(id))
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

        // POST: api/Filles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Filles>> PostFilles(Filles filles)
        {
            try
            {
                string fileExtension = Path.GetExtension(filles.NameFille).Trim('.');
                filles.TypeFiles = fileExtension;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == filles.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                if (!Directory.Exists(path + $"\\{user.Name}"))
                {
                    Directory.CreateDirectory(path + $"\\{user.Name}");
                    using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                    {
                        using (FileStream fileStream = new FileStream(path + $"\\{user.Name}\\{filles.NameFille}", FileMode.OpenOrCreate))
                        {
                             await    memoryStream.CopyToAsync(fileStream);
                            filles.StoragePath = fileStream.Name;
                            filles.Size = fileStream.Length;
                        }
                    }


                    Console.WriteLine($"Папка успешно создана! {path + $"\\{user.Name}"}");
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                    {
                        using (FileStream fileStream = new FileStream(path + $"\\{user.Name}\\{filles.NameFille}", FileMode.OpenOrCreate))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                            filles.StoragePath = fileStream.Name;

                            filles.Size = fileStream.Length;
                        }
                    }
                    Console.WriteLine($"Папка с указанным путем уже существует: {path + $"\\{user.Name}"}");
                }

                _context.Filles.Add(filles);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {              
                NotFound(ex);
            }
            return CreatedAtAction("GetFilles", new { id = filles.Id }, filles);
        }

        // DELETE: api/Filles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilles(int id)
        {
            try
            {


                var filles = await _context.Filles.FindAsync(id);
                if (filles == null)
                {
                    return NotFound();
                }

                _context.Filles.Remove(filles);
                await _context.SaveChangesAsync();
                System.IO.File.Delete(filles.StoragePath);

            }catch (Exception ex)
            {
                return NotFound(ex);


            }
            return NoContent();
        }

        private bool FillesExists(int id)
        {
            return _context.Filles.Any(e => e.Id == id);
        }
    }
}
