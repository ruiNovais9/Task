using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Contracts.Responses
{
    public class PaginationProjectResponse : ProjectResponse
    {
        public bool HaveMoreProjects { get; set; }
    }
}
