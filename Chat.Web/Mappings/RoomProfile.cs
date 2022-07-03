
using AutoMapper;
using Chat.Web.Models;
using Chat.Web.ViewModels;

namespace Chat.Web.Mappings
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomViewModel>();
            CreateMap<RoomViewModel, Room>();
        }
    }
}
