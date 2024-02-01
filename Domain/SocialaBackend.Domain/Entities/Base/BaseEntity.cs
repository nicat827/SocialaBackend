using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public  DateTime CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set;}

        public string CreatedBy { get; set; } = null!;
        public string? LastUpdatedBy { get; set; }
    }
}
