using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using ASBDDS.Shared.Models.Responses;
using GitHub.JPMikkers.DHCP;
using Microsoft.AspNetCore.Authorization;
using DHCPServer = ASBDDS.API.Servers.DHCP.DHCPServer;

namespace ASBDDS.API.Controllers
{
    [Route("api/admin/dhcp/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DhcpController : ControllerBase
    {
        private DHCPServer _dhcpServer;
        public DhcpController(DHCPServer dhcpServer)
        {
            _dhcpServer = dhcpServer;
        }
        [HttpGet("leases/")]
        public ApiResponse<List<DHCPLeaseAdminResponse>> GetLeases()
        {
            var resp = new ApiResponse<List<DHCPLeaseAdminResponse>>();
            try
            {
                var leases = new List<DHCPLeaseAdminResponse>();
                foreach (var lease in _dhcpServer.LeasesManager.GetLeases())
                {
                    leases.Add(new DHCPLeaseAdminResponse(lease));
                }
                resp.Data = leases;
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpPut("leases/{macAddress}/make-static")]
        public ApiResponse MakeStatic(string macAddress, [FromQuery] string ip)
        {
            var resp = new ApiResponse();
            try
            {
                var parsedIp = IPAddress.Parse(ip);
                var isCorrectIp = !parsedIp.Equals(IPAddress.Any) && _dhcpServer.LeasesManager.Pool.InPool(parsedIp);
                if (isCorrectIp)
                {
                    var lease = _dhcpServer.LeasesManager.Get(Utils.HexStringToBytes(macAddress));
                    if (lease == null)
                    {
                        lease = _dhcpServer.LeasesManager.Create(Utils.HexStringToBytes(macAddress));
                    }
                    
                    lease.Address = parsedIp;
                    _dhcpServer.LeasesManager.MakeStatic(lease);
                }
                else
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "IP is not correct";
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
        
        [HttpPut("leases/{macAddress}/make-dynamic")]
        public ApiResponse MakeDynamic(string macAddress)
        {
            var resp = new ApiResponse();
            try
            {
                var lease = _dhcpServer.LeasesManager.Get(Utils.HexStringToBytes(macAddress));
                if (lease is {Static: true})
                {
                    _dhcpServer.LeasesManager.MakeDynamic(lease);
                }
                else
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Lease not found or already dynamic";
                }
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }

        [HttpDelete("leases/{macAddress}")]
        public ApiResponse DeleteLease(string macAddress)
        {
            var resp = new ApiResponse();
            try
            {
                var lease = _dhcpServer.LeasesManager.Get(Utils.HexStringToBytes(macAddress));
                if (lease is {Static: false})
                {
                    _dhcpServer.LeasesManager.Remove(lease);
                }
                else
                {
                    resp.Status.Code = 1;
                    resp.Status.Message = "Make sure the lease exists, is released and is not static";
                }
                
            }
            catch(Exception e)
            {
                resp.Status.Code = 1;
                resp.Status.Message = e.Message;
            }
            return resp;
        }
    }
}
