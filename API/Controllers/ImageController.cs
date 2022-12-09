using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models.Persistence;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly DataContext _context;
        public ImageController(DataContext context)
        {
            this._context = context;
        }
        /*
        
    
       
        GET /api/images/populartags
        */

        // /api/Images?pageNumber={pageNumber}&pageSize={pageSize}
        [HttpGet]
        public IActionResult GetAllImages([FromQuery] int pageNumber, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0)
            {
                return BadRequest();
            }
            var result = _context.Images.Include(image => image.User).OrderByDescending(image => image.PostingDate).Skip((pageNumber - 1) * pageSize).Take(pageSize);


            List<Dictionary<string, string>> res = result.Select(image => new Dictionary<string, string>(){
                {"id", image.Id.ToString()},
                {"url", image.Url},
                {"username", image.User.Name}
            }).ToList();

            var totalRecords = _context.Images.Count();

            var response = ResponseHelper<List<Dictionary<string, string>>>.GetPR("/api/Images", res, pageNumber, pageSize, totalRecords);

            return Ok(response);
        }

        //GET /api/images/{id}

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetAllDetailsOfAnImage(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
               

                return BadRequest();
            }

            Image image = _context.Images.Include(image => image.Tags).Include(image => image.User).FirstOrDefault(image => image.Id.Equals(new Guid(id)));

            if (image == null)
            {
               
                return NotFound();
            }
            else
            {
                ImageDTO result = new ImageDTO()
                {
                    Id = image.Id,
                    Url = image.Url,
                    UserName = image.User.Name,
                    UserId = image.User.Id,
                    Tags = image.Tags.Select(t => t.Text).ToList()
                };
                return Ok(result);
            }

            


        }
        //GET /api/images/byTag?tag=cars
        [HttpGet]
        [Route("byTag")]
        public IActionResult GetAllImagesThatIncludeGivenTag([FromQuery] string tag, int pageNumber = 1, int pageSize = 10)
        {
            if (String.IsNullOrEmpty(tag))
            {
            
                return BadRequest();
            }

            if (pageNumber < 1 || pageSize < 1)
            {
                
                return BadRequest();
            }

            var images = _context.Images.Include(image => image.Tags).Include(image => image.User).Where(image => image.Tags.Any(t => t.Text.Equals(tag)));

            if (images.Count() <= 0)
            {
                
                return NotFound();
            }

            var result = images.Select(image =>
            new
            {
                id = image.Id,
                url = image.Url,
                username = image.User.Name
            });

            var response = ResponseHelper<object>.GetPR("/api/Images/byTag", result, pageNumber, pageSize, result.Count());
            return Ok(response);
        }

        //GET /api/images/populartags

        public IActionResult GetTop5PopularTags()
        {
        
            Dictionary<string, int> dictionary = _context.Tags.ToDictionary(tag => tag.Text, tag => 0);

            
         
            _context.Images.Include(image => image.Tags).ForEachAsync(image => image.Tags.ForEach(tag => dictionary[tag.Text]++));

          
            var res = dictionary.OrderByDescending(kv => kv.Value).Take(5).Select(kv => new Dictionary<string, string>(){
                {"tag", kv.Key.ToString()},
                {"count", kv.Value.ToString()}
            });

            return Ok(res);
        }





















        
    }
}