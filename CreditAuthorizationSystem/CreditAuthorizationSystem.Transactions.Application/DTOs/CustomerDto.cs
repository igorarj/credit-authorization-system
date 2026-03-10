using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditAuthorizationSystem.Transactions.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Document { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
    }
}
