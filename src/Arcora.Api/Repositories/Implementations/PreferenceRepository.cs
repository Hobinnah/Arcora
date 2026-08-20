// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;

namespace Arcora.Api.Repositories.Implementations
{
    public class PreferenceRepository : Repository<Preference>, IPreferenceRepository
    {
        public PreferenceRepository(ArcoraDbContext context) : base(context)
        {
        }
    }
}