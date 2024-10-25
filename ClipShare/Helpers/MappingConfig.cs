using AutoMapper;
using ClipShare.Core.Entities;
using ClipShare.ViewModels.Admin;
using ClipShare.ViewModels.Moderator;

namespace ClipShare.Helpers
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // From, To
            CreateMap<AppUser, UserDisplayGrid_vm>()
                .ForMember(d => d.ChannelId, opt => opt.MapFrom(s => s.Channel == null ? 0 : s.Channel.Id))
                .ForMember(d => d.ChannelName, opt => opt.MapFrom(s => s.Channel == null ? "" : s.Channel.Name));

            CreateMap<AppUser, UserAddEdit_vm>();

            CreateMap<Video, VideoDisplayGrid_vm>()
                .ForMember(d => d.ChannelName, opt => opt.MapFrom(s => s.Channel.Name))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
        }
    }
}
