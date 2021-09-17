using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ASBDDS.API.Models
{
    public class AuthOptions
    {
        public AuthOptions(string issuer, string audience, string key, int lifetimeMinutes)
        {
            Issuer = issuer;
            Audience = audience;
            Lifetime = lifetimeMinutes;
            Key = key;
        }

        public AuthOptions() { }

        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int Lifetime { get; set; }
        
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}