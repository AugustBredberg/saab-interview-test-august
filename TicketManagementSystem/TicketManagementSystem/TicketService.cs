using System;
using System.IO;
using System.Text.Json;
using EmailService;
using TicketManagementSystem.Exceptions;
using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem
{
    public class TicketService
    {
        private IUserRepository _userRepository;
        private IEmailService _emailService;

        // TODO
        // Remove this constructor when real dep injection can be set up
        // Not possible without changing Program.cs
        public TicketService()
        : this(new UserRepository(), new EmailServiceProxy())
        {
        }

        public TicketService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public int CreateTicket(string t, Priority p, string assignedTo, string desc, DateTime d, bool isPayingCustomer)
        {
            // Validate input parameters
            if (String.IsNullOrEmpty(t) || String.IsNullOrEmpty(desc))
            {
                throw new InvalidTicketException("Title or description were null");
            }

            var priority = AdjustTicketPriority(t, p, d);
            if (priority == Priority.High)
            {
                _emailService.SendEmailToAdministrator(t, assignedTo);
            }

            var ticket = new Ticket()
            {
                Title = t,
                AssignedUser = GetUser(assignedTo),
                Priority = priority,
                Description = desc,
                Created = d,
                PriceDollars = isPayingCustomer ? GetTicketPrice(priority) : 0,
                AccountManager = isPayingCustomer ? _userRepository.GetAccountManager() : null
            };

            return TicketRepository.CreateTicket(ticket);
        }

        private Priority AdjustTicketPriority(string title, Priority priority, DateTime timestamp)
        {
            var titleContainsKeywords = 
                    title.Contains("Crash") || 
                    title.Contains("Important") || 
                    title.Contains("Failure");

            var isOlderThanOneHour = timestamp < DateTime.UtcNow - TimeSpan.FromHours(1);
            if (isOlderThanOneHour || titleContainsKeywords)
            {
                switch (priority)
                {
                    case Priority.Low:
                        priority = Priority.Medium;
                        break;
                    case Priority.Medium:
                        priority = Priority.High;
                        break;
                }
            }

            return priority;
        }

        private double GetTicketPrice(Priority priority)
        {
            switch (priority)
            {
                case Priority.High:
                    return 100;
                default:
                    return 50;
            }
        }

        public void AssignTicket(int id, string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty");
            }

            var ticket = TicketRepository.GetTicket(id);
            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + id);
            }

            ticket.AssignedUser = GetUser(username);
            TicketRepository.UpdateTicket(ticket);
        }

        private User GetUser(string username)
        {
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
