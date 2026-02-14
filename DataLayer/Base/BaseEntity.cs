using DataLayer.Enums;
using DataLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Base
{
    public abstract class BaseEntity : IBaseEntity
    {
        #region Common Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        #endregion

        #region Data Auditing Properties

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = null;
        public DataCreation CreatedBy { get; set; }
        public DataCreation UpdatedBy { get; set; }

        #endregion
    }
}
