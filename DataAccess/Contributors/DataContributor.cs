using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Contributors
{
    public static class DataContributor
    {
        public static readonly IEnumerable<Product> products = new[]
        {
            new Product
            {
                ProductId = 1,
            }
        };
    }
}
