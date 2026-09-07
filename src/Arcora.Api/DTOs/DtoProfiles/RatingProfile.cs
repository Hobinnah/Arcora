// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<Rating, RatingDto>()
                .ForMember(dest => dest.ReviewerFirstName, opt => opt.MapFrom(src =>
                    src.ReviewerUser != null ? src.ReviewerUser.FirstName : null))
                .ForMember(dest => dest.ReviewerLocation, opt => opt.MapFrom(src =>
                    src.Lease != null && src.Lease.RentalUnit != null && src.Lease.RentalUnit.Property != null && src.Lease.RentalUnit.Property.Address != null
                        ? BuildLocation(src.Lease.RentalUnit.Property.Address.City, src.Lease.RentalUnit.Property.Address.ProvinceCode)
                        : null))
                .ForMember(dest => dest.ReviewerPhotoUrl, opt => opt.MapFrom(src =>
                    src.Lease != null && src.Lease.Tenant != null ? src.Lease.Tenant.PhotoUrl : null))
                .ReverseMap()
                .ForMember(dest => dest.ReviewerUser, opt => opt.Ignore())
                .ForMember(dest => dest.Lease, opt => opt.Ignore());
        }

        /// <summary>
        /// Combines city and province/state into a single display location.
        /// </summary>
        private static string? BuildLocation(string? city, string? provinceCode)
        {
            if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(provinceCode))
                return null;
            if (string.IsNullOrWhiteSpace(provinceCode))
                return city;
            if (string.IsNullOrWhiteSpace(city))
                return provinceCode;
            return $"{city}, {provinceCode}";
        }
    }
}