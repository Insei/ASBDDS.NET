namespace ASBDDS.Shared.Models.Database.DataDb
{
    public enum DeviceMachineState
    {
        /// <summary>
        /// Device is not in use
        /// </summary>
        Free = 0,
        /// <summary>
        /// Creating state (Prepare)
        /// </summary>
        Creating,
        /// <summary>
        /// Provisioning state
        /// </summary>
        Provisioning,
        /// <summary>
        /// Erase machine storage
        /// </summary>
        Erasing,
        /// <summary>
        /// Device is ready to use
        /// </summary>
        Ready,
        /// <summary>
        /// Device is ready to use in ipxe mode only
        /// </summary>
        IPXEOnly,
    }
}
