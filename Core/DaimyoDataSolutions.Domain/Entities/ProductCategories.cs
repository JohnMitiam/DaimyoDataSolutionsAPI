using DaimyoDataSolutions.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class ProductCategories : BaseModel
    {
        public int ProductId { get; set; }
        public Products Product { get; set; } = null;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null;
        public string CategoryName { get; set; }
    }
}
