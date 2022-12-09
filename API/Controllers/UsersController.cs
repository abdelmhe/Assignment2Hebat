using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        








    }
}