using DaimyoDataSolutions.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaimyoDataSolutions.Domain.Entities
{
    public class ProductImages : BaseModel
    {
        public int ProductId { get; set; }
        public Products Product { get; set; } = null;
        public byte[] ImageData { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}
