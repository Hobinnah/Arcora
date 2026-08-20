// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;

namespace Arcora.Api.Services.Implementations
{
    public class PreferenceService : IPreferenceService
    {
        private readonly IPreferenceRepository _repository;
        public PreferenceService(IPreferenceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Preference?> GetPreference()
        {
            return await _repository.FirstOrDefault();
        }
    }
}