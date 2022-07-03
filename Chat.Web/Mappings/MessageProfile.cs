
using AutoMapper;
using Chat.Web.Helpers;
using Chat.Web.Models;
using Chat.Web.ViewModels;

namespace Chat.Web.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, MessageViewModel>()
                .ForMember(
                    messageViewModel => messageViewModel.From, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(message => message.FromUser.FullName))
                .ForMember(
                    messageViewModel => messageViewModel.Room, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(message => message.ToRoom.Name))
                .ForMember(
                    messageViewModel => messageViewModel.Avatar, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(message => message.FromUser.Avatar))
                .ForMember(
                    messageViewModel => messageViewModel.Content, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(message => BasicEmojis.ParseEmojis(message.Content)))
                .ForMember(
                    messageViewModel => messageViewModel.Timestamp, 
                    memberConfigurationExpression => memberConfigurationExpression.MapFrom(message => message.Timestamp));
            
            CreateMap<MessageViewModel, Message>();
        }
    }
}
