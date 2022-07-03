
using AutoMapper;
using Chat.Web.Models;
using Chat.Web.ViewModels;

namespace Chat.Web.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(
                    userViewModel => userViewModel.Username, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(user => user.UserName));
            
            CreateMap<UserViewModel, ApplicationUser>();
        }
    }
}
