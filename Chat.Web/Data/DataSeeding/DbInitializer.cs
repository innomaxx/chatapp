using Chat.Web.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chat.Web.Data.DataSeeding
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            await Reset(db);
            await CreateUsers(userManager);
            await CreateRooms(db);
            await CreateMessages(db);
        }

        private static async Task Reset(ApplicationDbContext db)
        {
            db.Messages.RemoveRange(db.Messages.ToList());
            await db.SaveChangesAsync();

            db.Rooms.RemoveRange(db.Rooms.ToList());
            await db.SaveChangesAsync();
        }

        private static async Task CreateUsers(UserManager<ApplicationUser> userManager)
        {
            var user1 = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin@admin.com",
                FullName = "James Smith",
                Avatar = "avatar1.png"
            };

            var user2 = new ApplicationUser()
            {
                UserName = "admin2",
                Email = "admin2@admin.com",
                FullName = "Kate Harris",
                Avatar = "avatar2.png"
            };

            string password = "admin";
            var result1 = await userManager.CreateAsync(user1, password);
            var result2 = await userManager.CreateAsync(user2, password);
            Console.WriteLine(result1.ToString());
            Console.WriteLine(result2.ToString());
        }

        private static async Task CreateRooms(ApplicationDbContext db)
        {
            var users = db.Users.ToList();

            var rooms = new List<Room>()
            {
                new Room(){Name = "Lobby", Admin = users[0] },
                new Room(){Name = "Marketing", Admin = users[1] },
                new Room(){Name = "Desingers", Admin = users[0] },
                new Room(){Name = "Developers", Admin = users[0] },
                new Room(){Name = "Brainstorming", Admin = users[1] },
                new Room(){Name = "Support", Admin = users[1] },
                new Room(){Name = "Cool Stories", Admin = users[0] },
            };

            db.Rooms.AddRange(rooms);
            await db.SaveChangesAsync();
        }

        private static async Task CreateMessages(ApplicationDbContext db)
        {
            var lobby = db.Rooms.FirstOrDefault(r => r.Name == "Lobby");
            var users = db.Users.ToList();

            var messagesStr = new string[]
            {
                "Hey guys, could you help me with vacation plans? any suggestion?",
                $"What about Greece? It has many beautiful islands...",
                "That's a good suggestion!",
                "Thank you..."
            };

            var messages = new List<Message>()
            {
                new Message()
                {
                    Content = Regex.Replace(messagesStr[0], @"<.*?>", string.Empty),
                    FromUser = users[0],
                    ToRoom = lobby,
                    Timestamp = DateTime.Now.AddMinutes(new Random().Next(-15, -12))
                },
                new Message()
                {
                    Content = Regex.Replace(messagesStr[1], @"<.*?>", string.Empty),
                    FromUser = users[1],
                    ToRoom = lobby,
                    Timestamp = DateTime.Now.AddMinutes(new Random().Next(-11, -8))
                },
                new Message()
                {
                    Content = Regex.Replace(messagesStr[2], @"<.*?>", string.Empty),
                    FromUser = users[0],
                    ToRoom = lobby,
                    Timestamp = DateTime.Now.AddMinutes(new Random().Next(-7, -5))
                },
                new Message()
                {
                    Content = Regex.Replace(messagesStr[3], @"<.*?>", string.Empty),
                    FromUser = users[1],
                    ToRoom = lobby,
                    Timestamp = DateTime.Now.AddMinutes(new Random().Next(-2, 0))
                }
            };

            db.Messages.AddRange(messages);
            await db.SaveChangesAsync();
        }
    }
}
