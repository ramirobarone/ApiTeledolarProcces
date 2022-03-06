using System;

namespace Domain.Common
{
    public class AuditableEntity
    {
        public string CreatedBy { get; set; }

        public DateTime GeneratedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
