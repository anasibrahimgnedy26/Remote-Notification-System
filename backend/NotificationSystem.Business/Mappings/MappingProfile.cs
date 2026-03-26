using AutoMapper;
using NotificationSystem.Core.Entities;
using NotificationSystem.Business.DTOs;

namespace NotificationSystem.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Notification mappings
            CreateMap<NotificationRequestDTO, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsSent, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationLogs, opt => opt.Ignore());

            CreateMap<Notification, NotificationResponseDTO>();

            // Device mappings
            CreateMap<DeviceRegistrationDTO, Device>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RegisteredAt, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationLogs, opt => opt.Ignore());

            CreateMap<Device, DeviceRegistrationDTO>();
        }
    }
}
