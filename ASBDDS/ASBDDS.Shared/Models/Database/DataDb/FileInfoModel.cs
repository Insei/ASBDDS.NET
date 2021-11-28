using System;

namespace ASBDDS.Shared.Models.Database.DataDb
{
    public class FileInfoModel : DbBaseModel
    {
        /// <summary>
        /// File name without extension
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// File extension
        /// </summary>
        public string Extension  { get; set; }
        /// <summary>
        /// File name with extension
        /// </summary>
        public string FullName { get; set; }

        public FileInfoModel() : base()
        {
            Id = Guid.NewGuid();
        }
    }
}