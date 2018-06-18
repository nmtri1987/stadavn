using RBVH.Stada.Intranet.Biz.Helpers;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Models
{
    public abstract class EntityBase
    {
        public enum BatchCmd
        {
            Save,
            Delete
        }

        /// <summary>
        /// Item identifier
        /// </summary>
        [ListColumn("UniqueId", true)]
        public virtual Guid? UniqueId { get; set; }

        /// <summary>
        /// Id of item on list
        /// </summary>
        [ListColumn("ID")]
        public virtual int ID { get; set; }

        public List<AttachmentInfo> Attachments { get; set; }

        public BatchCmd BatchCommand { get; set; }

        protected EntityBase()
        {
            BatchCommand = BatchCmd.Save;
            Attachments = new List<AttachmentInfo>();
        }

    }
}
