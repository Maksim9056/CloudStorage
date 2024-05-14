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

        public FillesController(DB context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<Filles>> PostFilles(Filles filles)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string fileExtension = Path.GetExtension(filles.NameFille).Trim('.');
                filles.TypeFiles = fileExtension;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == filles.UserId);

                List<string> list = new List<string>();
                DateTime dateTime = DateTime.Now;

                string DATE = "";
                var date = dateTime.ToString();
                var dates = date.Replace('.', '_');

                var DA = dates.Replace(':', '_');
                for (int i = 0; i < DA.Length; i++)
                {
                    if (DA[i].ToString() == "13")
                    {
                        DATE += "_";
                    }
                    else
                    {
                        list.Add(DA[i].ToString());

                    }
                }
                DATE = "";
                list.RemoveAt(10);
                list.Insert(10, "_");
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i] == "")
                    {

                    }
                    else
                    {
                        DATE += list[i].ToString();
                    }
                }

                if (user == null)
                {
                    return NotFound();
                }
                var paths = Path.Combine(path, user.Name);
                //!Directory.Exists(path + $"\\{user.Name}"
                if (!Directory.Exists(paths))
                {
                    
                    Directory.CreateDirectory(paths);
                    paths = Path.Combine(paths, Guid.NewGuid().ToString() + "_"+ filles.NameFille);
                    using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                    {
                        //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}"
                        using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                            filles.StoragePath = fileStream.Name;
                            filles.Size = fileStream.Length;
                        }
                    }

                    Console.WriteLine($"Папка успешно создана! {paths}");
                }
                else
                {
                    paths =Path.Combine(paths, Guid.NewGuid().ToString()  + "_"+ filles.NameFille);
                    using (MemoryStream memoryStream = new MemoryStream(filles.Fille))
                    {
                        //$"\\{user.Name}\\{Guid.NewGuid().ToString() + "_DATE_" + DATE + filles.NameFille}
                        using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                            filles.StoragePath = fileStream.Name;

                            filles.Size = fileStream.Length;
                        }
                    }
                    Console.WriteLine($"Папка с указанным путем уже существует: {paths}");
                }

                _context.Filles.Add(filles);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetFilles", new { id = filles.Id }, filles);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
                return StatusCode(404, ex.Message+"Ошибка");
            }
        }

        [HttpGet("user{id}")]
        public async Task<ActionResult<IEnumerable<Filles>>> GetGroupsChats(int id)
        {
            try
            {
                return await _context.Filles.Where(u => u.UserId == id).ToListAsync();

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
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

            }
            catch (Exception ex)
            {
                return NotFound(ex);


            }
            return NoContent();
        }

        //// GET: api/Filles
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Filles>>> GetFilles()
        //{
        //    return await _context.Filles.ToListAsync();
        //}

        // GET: api/Filles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Filles>> GetFilles(int id)
        {
            var filles = await _context.Filles.FindAsync(id);

            if (filles == null)
            {
                return NotFound();
            }

            return filles;
        }

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

        //// POST: api/Filles
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Filles>> PostFilles(Filles filles)
        //{
        //    _context.Filles.Add(filles);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetFilles", new { id = filles.Id }, filles);
        //}

        //// DELETE: api/Filles/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteFilles(int id)
        //{
        //    var filles = await _context.Filles.FindAsync(id);
        //    if (filles == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Filles.Remove(filles);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool FillesExists(int id)
        //{
        //    return _context.Filles.Any(e => e.Id == id);
        //}
    }
}
