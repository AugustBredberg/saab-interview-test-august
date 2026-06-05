using System;
using EmailService;
using TicketManagementSystem.Exceptions;
using TicketManagementSystem.Helpers;
using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem
{
    public class TicketService(IUserRepository userRepository, IEmailService emailService)
    {
        private IUserRepository _userRepository = userRepository;
        private IEmailService _emailService = emailService;

        // TODO
        // Remove this constructor when real dep injection can be set up
        // Not possible without changing Program.cs
        public TicketService()
        : this(new UserRepository(System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString), new EmailServiceProxy())
        {
        }

        public int CreateTicket(string title, Priority priority, string assignedTo, string desc, DateTime timestamp, bool isPayingCustomer)
        {
            if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(desc))
            {
                throw new InvalidTicketException("Title or description were null");
            }

            priority = TicketHelpers.AdjustTicketPriority(title, priority, timestamp);
            if (priority == Priority.High)
            {
                _emailService.SendEmailToAdministrator(title, assignedTo);
            }

            var ticket = new Ticket()
            {
                Title = title,
                AssignedUser = GetUser(assignedTo),
                Priority = priority,
                Description = desc,
                Created = timestamp,
                PriceDollars = isPayingCustomer ? TicketHelpers.GetTicketPrice(priority) : 0,
                AccountManager = isPayingCustomer ? _userRepository.GetAccountManager() : null
            };

            return TicketRepository.CreateTicket(ticket);
        }

        public void AssignTicket(int id, string username)
        {
            var user = GetUser(username);
            var ticket = TicketRepository.GetTicket(id);
            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + id);
            }
            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        private User GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty");
            }

            User user = _userRepository.GetUser(username);
            if (user == null)
            {
                throw new UnknownUserException("User not found");
            }

            return user;
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
