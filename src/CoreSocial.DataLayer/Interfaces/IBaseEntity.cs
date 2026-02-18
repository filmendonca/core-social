using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IBaseEntity
    {
        #region Common Properties

        public int Id { get; set; }

        #endregion

        #region Data Auditing Properties

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DataCreation CreatedBy { get; set; }
        public DataCreation UpdatedBy { get; set; }

        #endregion
    }
}
