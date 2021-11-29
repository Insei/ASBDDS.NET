﻿using System;
using ASBDDS.Shared.Models.Database.DataDb;

namespace ASBDDS.Shared.Dtos.BootableImage
{
    public class BootableImageDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Created at
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Updated at
        /// </summary>
        public DateTime? Updated { get; set; }
        /// <summary>
        /// Disable flag
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// Bootable image name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Bootable image version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Bootable image arch
        /// </summary>
        public string Arch { get; set; }
        /// <summary>
        /// OS full name in system
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Bootable image start/installation protocol
        /// </summary>
        public DeviceBootProtocol InProtocol { get; set; }
        /// <summary>
        /// Boot file
        /// </summary>
        public string BootFile { get; set; }
        /// <summary>
        /// Boot protocol after start/installation
        /// </summary>
        public DeviceBootProtocol OutProtocol { get; set; }
        /// <summary>
        /// Options in JSON format
        /// Objects array { "Name": "OpName", "Value": "OpValue" }
        /// </summary>
        public string Options { get; set; }
        /// <summary>
        /// Bootable image type
        /// </summary>
        public BootableImageType Type { get; set; }
    }
}