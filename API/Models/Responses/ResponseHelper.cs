using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class ResponseHelper<T>
    {
        public static PagedResponse<T> GetPR(string url, IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            var response = new PagedResponse<T>(data);
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            response.Meta.Add("TotalRecords", totalRecords.ToString());
            response.Meta.Add("TotalPages", totalPage.ToString());
            response.Links.Add("frstPage", $"{url}?pagenumber=1&pagesize={pageSize}");
            response.Links.Add("lastPage", $"{url}?pagenumber={totalPage}&pagesize={pageSize}");
            response.Links.Add("next", pageNumber >= totalPage ? null : $"{url}?pagenumber={pageNumber + 1}&pagesize={pageSize}");
            response.Links.Add("previous", pageNumber <= 1 ? null : $"{url}?pagenumber={pageNumber - 1}&pagesize={pageSize}");

            return response;
        }

        public static PagedResponse<Dictionary<string, string>> GetPR(string url, IEnumerable<Dictionary<string, string>> data, int pageNumber, int pageSize, int totalRecords)
        {
            var response = new PagedResponse<Dictionary<string, string>>(data);
            int totalPage = (int)Math.Ceiling(totalRecords / (double)pageSize);
            response.Meta.Add("TotalRecords", totalRecords.ToString());
            response.Meta.Add("TotalPages", totalPage.ToString());
            response.Links.Add("frstPage", $"{url}?pagenumber=1&pagesize={pageSize}");
            response.Links.Add("lastPage", $"{url}?pagenumber={totalPage}&pagesize={pageSize}");
            response.Links.Add("next", pageNumber >= totalPage ? null : $"{url}?pagenumber={pageNumber + 1}&pagesize={pageSize}");
            response.Links.Add("previous", pageNumber <= 1 ? null : $"{url}?pagenumber={pageNumber - 1}&pagesize={pageSize}");

            return response;
        }

        
    }
}