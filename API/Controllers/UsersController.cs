using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Persistence;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public DataContext _context { get; set; }

        public UsersController(DataContext context)
        {
            this._context = context;
        }


        

        

        //POST /api/users/

          [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            bool isEmailExisting = UserHelper.IsEmailPresent(_context, user.Email);
            ErrorR errorR = new ErrorR();

           

            if (String.IsNullOrEmpty(user.Name))
            {
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Name should not be empty",
                    Detail = "Name should not be empty"
                });

                return BadRequest();
            }
            else if (String.IsNullOrEmpty(user.Email))
            {
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Email should not be empty",
                    Detail = "Email should not be empty"
                });
            }
            else if (isEmailExisting)
            {
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Email already exists",
                    Detail = "This email already exists in the database"
                });
            }
            else if (!UserHelper.IsEmailValid(user.Email))
            {
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Email is not valid",
                    Detail = "This email is not valid"
                });
            }
            else
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(errorR);
        }

        //POST /api/users/{id}/image

        // /api/users/{id}/image
        [HttpPost]
        [Route("{id}/image")]
        public async Task<IActionResult> AddImageForSpecificUser(string id, [FromBody] Image image = null)
        {

           
            User user = _context.Users.FirstOrDefault(u => u.Id.Equals(new Guid(id)));
            
            if (user == null)
            {
                ErrorR errorR = new ErrorR();
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Invalid user id",
                    Detail = "The user id is invalid"
                });
                return BadRequest(errorR);
            }

            
            if (image == null)
            {
                ErrorR errorR = new ErrorR();
                errorR.errors.Add(new Error()
                {
                    Status = "400",
                    Title = "Invalid image information",
                    Detail = "The image information is invalid"
                });

                return BadRequest(errorR);
            }
            else
            {
                IEnumerable<string> tags = ImageHelper.GetTags(image.Url);
                List<Tag> tagList = TagHelper.AddTagsIfNotPresent(_context, tags);

                Image img = new Image()
                {
                    Url = image.Url,
                    PostingDate = DateTime.Now,
                    Tags = tagList,
                    User = user
                };

               
                _context.Images.Add(img);

                await _context.SaveChangesAsync();

                var images = _context.Images.Where(i => i.User.Equals(user)).OrderByDescending(x => x.PostingDate).Take(10);

                UserDTO userDTO = new UserDTO()
                {
                    Id = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    ImagesUrls = new List<string>()
                };

                foreach (var i in images)
                {
                    userDTO.ImagesUrls.Add(i.Url);
                }

                return Ok(userDTO);
            }
        }

        //GET /api/users/{id}

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetUser(string id)
        {
            User user = _context.Users.Include(user => user.Images.OrderByDescending(x => x.PostingDate).Take(10)).FirstOrDefault(user => user.Id.Equals(new Guid(id)));
            if (user == null)
            {
                ErrorR errorR = new ErrorR();
                errorR.errors.Add(new Error()
                {
                    Status = "404",
                    Title = "User not found",
                    Detail = "The user is not found"
                });
                return NotFound(errorR);
            }
            else
            {
                return Ok(new UserDTO()
                {
                    Id = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    ImagesUrls = user.Images.Select(image => image.Url).ToList()
                });
            }
        }

        // GET /api/users/{id}/images

        [HttpGet]
        [Route("{id}/images")]
        public IActionResult GetAllImagesOfGivenUser(string id, [FromQuery] int pageNumber, int pageSize = 10)
        {
            User user = _context.Users.Include(u => u.Images).FirstOrDefault(u => u.Id.Equals(new Guid(id)));
            if (user == null)
            {
                ErrorR errorR = new ErrorR();
                errorR.errors.Add(new Error()
                {
                    Status = "404",
                    Title = "User not found",
                    Detail = "The user is not found"
                });
                return NotFound(errorR);
            }
            else
            {
                List<ImageModifiedDTO> result = user.Images.OrderByDescending(image => image.PostingDate).Select(image => new ImageModifiedDTO()
                {
                    Id = image.Id,
                    Url = image.Url
                }).ToList();

                var totalRecords = result.Count();

                PagedResponse<ImageModifiedDTO> response = ResponseHelper<ImageModifiedDTO>.GetPR($"/api/Users/{id}/images", result, pageNumber, pageSize, totalRecords);
                return Ok(response);
            }
        }

        // DELETE /api/users/{id}

          [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            User user = _context.Users.Include(u => u.Images).FirstOrDefault(u => u.Id.Equals(new Guid(id)));
            if (user == null)
            {
                ErrorR errorR = new ErrorR();
                errorR.errors.Add(new Error()
                {
                    Status = "404",
                    Title = "User not found",
                    Detail = "The user is not found"
                });

                return NotFound(errorR);
            }
            else
            {
                _context.Images.RemoveRange(user.Images);
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                return Ok();
            }

        }




        








    }
}