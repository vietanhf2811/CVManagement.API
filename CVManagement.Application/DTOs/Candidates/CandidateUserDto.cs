using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Application.DTOs.Candidates
{
    public class CandidateUserDto
    {
        public string Email { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }
}
