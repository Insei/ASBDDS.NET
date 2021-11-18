using System.Collections.Generic;
using System.Linq;
using ASBDDS.Shared.Models.Database.DataDb;
using GitHub.JPMikkers.DHCP;
using Microsoft.EntityFrameworkCore;

namespace ASBDDS.API.Servers.DHCP
{
    public static class DHCPServerEventHandler
    {
        private static DataDbContext _context;
        private static readonly object _lock = new object();
        private static List<DHCPLeaseDb> _leases;
        public static void Init(DataDbContext context, DHCPServer server)
        {
            _context = context;
            _leases = _context.DHCPLeases.ToList();
            
            server.LeasesManager.OnLeaseChange += OnLeaseChange;
        }

        private static void OnLeaseChange(object sender, DHCPLease lease)
        {
            // we handle only lease changes to save static leases to db
            lock (_lock)
            {
                var savedStaticLease = _leases.FirstOrDefault(l => l.MacAddress == lease.MacAddress);
                if (savedStaticLease != null)
                {
                    if (!lease.Static)
                    {
                        _leases.Remove(savedStaticLease);
                        _context.DHCPLeases.Remove(savedStaticLease);
                    }
                    else
                    {
                        // if we have differences in-memory static leases collection, then we can save static leases changes to db.
                        // This may be overkill, but I think it's best to play it safe.
                        var needSave = false;
                        if (savedStaticLease.Address != lease.Address.ToString())
                        {
                            needSave = true;
                            savedStaticLease.Address = lease.Address.ToString();
                        }
                        if (savedStaticLease.MacAddress != lease.MacAddress)
                        {
                            needSave = true;
                            savedStaticLease.MacAddress = lease.MacAddress;
                        }

                        if (needSave)
                        {
                            _context.Entry(savedStaticLease).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                }
                else if(lease.Static)
                {
                    var newStaticLease = new DHCPLeaseDb
                    {
                        Address = lease.Address.ToString(),
                        MacAddress = lease.MacAddress,
                        Static = true
                    };
                    _context.DHCPLeases.Add(newStaticLease);
                    _context.SaveChanges();
                    _leases.Add(newStaticLease);
                }
            }
        }
    }
}