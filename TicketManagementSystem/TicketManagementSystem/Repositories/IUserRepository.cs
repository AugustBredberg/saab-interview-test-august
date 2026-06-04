using System;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories;

public interface IUserRepository : IDisposable
{
    User GetUser(string username);
    User GetAccountManager();
}
