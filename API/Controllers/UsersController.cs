using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

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


        

        /*
        POST /api/users/
        POST /api/users/{id}/image
        GET /api/users/{id}
        GET /api/users/{id}/images
        DELETE /api/users/{id}
        */

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
        








    }
}