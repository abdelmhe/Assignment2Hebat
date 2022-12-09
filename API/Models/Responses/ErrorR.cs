using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class ErrorR
    {
          public List<Error> errors { get; set; } = new List<Error>();
    }
    public class Error
    {
        public string Status { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }
    }





}