using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Origins
{
    public class OriginResponse
    {
        public int OriginId { get; set; }
        public string OriginName { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}