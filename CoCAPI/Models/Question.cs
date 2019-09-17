using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoCAPI.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuexText { get; set; }
        public string Level { get; set; }
        public string QuexId { get; set; }
    }
}
