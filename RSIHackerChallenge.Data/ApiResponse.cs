using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSIHackerChallenge.Data
{
    public class ApiResponse
    {
        public List<Story> Stories { get; set; }

        public int TotalCount {  get; set; }
    }
}
