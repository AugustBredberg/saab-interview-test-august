using System;
using TicketManagementSystem.Exceptions;

namespace TicketManagementSystem.Test
{
    [TestFixture]
    public class AssignTicketTests
    {
        private TicketService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new TicketService();
        }

        [Test]
        public void AssignTicket_NullUsername_ThrowsArgumentException()
        {
            Assert.That(() => _service.AssignTicket(1, null),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void AssignTicket_NonExistentUser_ThrowsUnknownUserException()
        {
            Assert.That(() => _service.AssignTicket(1, "nonexistent_user"),
                Throws.TypeOf<UnknownUserException>());
        }

        [Test]
        public void AssignTicket_EmptyUsername_ThrowsArgumentException()
        {
            Assert.That(() => _service.AssignTicket(1, ""),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void AssignTicket_UnknownUserExceptionMessage_IsMeaningful()
        {
            var ex = Assert.Throws<UnknownUserException>(() => _service.AssignTicket(1, "test"));

            Assert.That(ex.Message, Is.Not.Null.And.Not.Empty);
        }
    }
}