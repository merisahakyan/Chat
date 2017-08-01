using AutoMapper;
using BLL.Models;
using ChatWithSignalR.DAL;
using Repo;

namespace BLL
{
    public static class AutoMapperConfiguration
    {
        static Manager m = new Manager();
        
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Room, RoomModel>()
                            .ForMember(dest => dest.RoomID, opt => opt.MapFrom(src => src.RoomID))
                            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
                            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => m.GetUsersByRoom(src.RoomName).Result));
                cfg.CreateMap<User, UserModel>()
                .ForMember(dest => dest.active, opt => opt.MapFrom(src => src.active))
                .ForMember(dest => dest.eMail, opt => opt.MapFrom(src => src.eMail))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.token, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));
                cfg.CreateMap<Message, MessageModel>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.MessageText))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Room.RoomName))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Edited, opt => opt.MapFrom(src => src.Edited))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
                cfg.CreateMap<OldMessage, HistoryModel>()
                            .ForMember(dest => dest.Edited, opt => opt.MapFrom(src => src.Time))
                            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.MessageText));

            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }
    }
}
