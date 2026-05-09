using AutoMapper;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Alert;
using Farm.Domain.ViewModels.Animal;
using Farm.Domain.ViewModels.Cage;
using Farm.Domain.ViewModels.Disease;
using Farm.Domain.ViewModels.Farm;
using Farm.Domain.ViewModels.Feed;
using Farm.Domain.ViewModels.GrowthLog;
using Farm.Domain.ViewModels.Vaccine;

namespace Farm.Api.Models
{
    /// <summary>
    /// Mapping configuration of Models to Entity and vice versa
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Mapping profile Constructor
        /// </summary>
        public MappingProfile()
        {
            // ===== Existing =====
            CreateMap<CreateAnimalDto, Animal>();
            CreateMap<UpdateAnimalDto, Animal>();
            CreateMap<CreateCageDto, Cage>();
            CreateMap<UpdateCageDto, Cage>();
            CreateMap<CreateFarmDto, Domain.Entities.Farm>();
            CreateMap<UpdateFarmDto, Domain.Entities.Farm>();

            CreateMap<Animal, AnimalDetailsDto>()
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.Cage.FarmId))
                .ForMember(dest => dest.CageName, opt => opt.MapFrom(src => src.Cage.Name))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Cage.Farm.Name));
            CreateMap<Cage, CageDetailsDto>()
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.Name));
            CreateMap<Domain.Entities.Farm, FarmDetailsDto>();

            // ===== Phase 1: Vaccine =====
            CreateMap<CreateVaccineDto, Vaccine>();
            CreateMap<UpdateVaccineDto, Vaccine>();
            CreateMap<Vaccine, VaccineDetailsDto>();

            CreateMap<CreateVaccineScheduleDto, VaccineSchedule>();
            CreateMap<VaccineSchedule, VaccineScheduleDto>()
                .ForMember(d => d.AnimalCode, o => o.MapFrom(s => s.Animal.Code))
                .ForMember(d => d.AnimalName, o => o.MapFrom(s => s.Animal.Name))
                .ForMember(d => d.VaccineName, o => o.MapFrom(s => s.Vaccine.Name));

            // ===== Phase 1: Feed =====
            CreateMap<CreateFeedItemDto, FeedItem>();
            CreateMap<UpdateFeedItemDto, FeedItem>();
            CreateMap<FeedItem, FeedItemDto>();

            CreateMap<CreateFeedTransactionDto, FeedTransaction>();
            CreateMap<FeedTransaction, FeedTransactionDto>()
                .ForMember(d => d.FeedItemName, o => o.MapFrom(s => s.FeedItem.Name))
                .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm.Name));

            CreateMap<CreateFeedConsumptionDto, FeedConsumption>();
            CreateMap<FeedConsumption, FeedConsumptionDto>()
                .ForMember(d => d.FeedItemName, o => o.MapFrom(s => s.FeedItem.Name));

            // ===== Phase 1: Growth =====
            CreateMap<CreateGrowthLogDto, GrowthLog>();
            CreateMap<UpdateGrowthLogDto, GrowthLog>();
            CreateMap<GrowthLog, GrowthLogDto>()
                .ForMember(d => d.AnimalCode, o => o.MapFrom(s => s.Animal.Code))
                .ForMember(d => d.AnimalName, o => o.MapFrom(s => s.Animal.Name));

            // ===== Phase 1: Disease =====
            CreateMap<CreateDiseaseRecordDto, DiseaseRecord>();
            CreateMap<UpdateDiseaseRecordDto, DiseaseRecord>();
            CreateMap<DiseaseRecord, DiseaseRecordDto>()
                .ForMember(d => d.AnimalCode, o => o.MapFrom(s => s.Animal.Code))
                .ForMember(d => d.AnimalName, o => o.MapFrom(s => s.Animal.Name));

            CreateMap<CreateTreatmentDto, Treatment>();
            CreateMap<Treatment, TreatmentDto>()
                .ForMember(d => d.DiseaseName, o => o.MapFrom(s => s.DiseaseRecord != null ? s.DiseaseRecord.DiseaseName : null));

            // ===== Phase 1: Alert =====
            CreateMap<Alert, AlertDto>()
                .ForMember(d => d.FarmName, o => o.MapFrom(s => s.Farm != null ? s.Farm.Name : null))
                .ForMember(d => d.AnimalCode, o => o.MapFrom(s => s.Animal != null ? s.Animal.Code : null))
                .ForMember(d => d.FeedItemName, o => o.MapFrom(s => s.FeedItem != null ? s.FeedItem.Name : null));
        }
    }
}
