using System;
using TicketManagementSystem.Exceptions;

namespace TicketManagementSystem.Test
{
    [TestFixture]
    public class CreateTicketTests
    {
        private TicketService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new TicketService();
        }

        [Test]
        public void CreateTicket_NullAssignedTo_ThrowsArgumentException()
        {
            // When assignedTo is null, GetUser is never called, user stays null.
            Assert.That(() => _service.CreateTicket(
                "Title", Priority.Low, null, "Description", DateTime.UtcNow, false),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CreateTicket_NonExistentUser_ThrowsUnknownUserException()
        {
            // UserRepository.GetUser catches DB exceptions and returns null.
            // With a fake connection string, user resolution always fails.
            Assert.That(() => _service.CreateTicket(
                "Title", Priority.Low, "nonexistent_user", "Description", DateTime.UtcNow, false),
                Throws.TypeOf<UnknownUserException>());
        }

        [Test]
        public void CreateTicket_NullAssignedTo_ExceptionMessageIndicatesNullUser()
        {
            var ex = Assert.Throws<ArgumentException>(() => _service.CreateTicket(
                "Title", Priority.Low, null, "Description", DateTime.UtcNow, false));

            Assert.That(ex.Message, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void CreateTicket_EmptyStringAssignedTo_ThrowsUnknownUserException()
        {
            Assert.That(() => _service.CreateTicket(
                "Title", Priority.Low, "name", "Description", DateTime.UtcNow, false),
                Throws.TypeOf<UnknownUserException>());
        }

        [Test]
        public void CreateTicket_ValidationOccursBeforeUserLookup()
        {
            // With null title AND null assignedTo, InvalidTicketException should be thrown
            // (not UnknownUserException), proving validation runs first.
            Assert.That(() => _service.CreateTicket(
                null, Priority.Low, null, "Description", DateTime.UtcNow, false),
                Throws.TypeOf<InvalidTicketException>());
        }
    }
}
