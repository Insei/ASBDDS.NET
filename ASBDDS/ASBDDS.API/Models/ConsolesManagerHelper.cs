using System;
using System.Linq;
using ASBDDS.Shared.Models.Database.DataDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASBDDS.API.Models
{
    public static class ConsolesManagerHelper
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var consolesManager = serviceProvider.GetRequiredService<ConsolesManager>();
            var context = serviceProvider.GetRequiredService<DataDbContext>();
            var consoles =  context.Consoles.Where(c => !c.Disabled).Include(c=>c.SerialSettings).ToList();
            foreach (var console in consoles)
            {
                try
                {
                    consolesManager.Add(console);
                    consolesManager.StartListening(console);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}