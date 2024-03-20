using Xunit;
using Moneybox.App.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneybox.App.DataAccess;
using Moq;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Features.Tests
{
    public class TransferMoneyTests
    {
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<INotificationService> notificationServiceMock;
        private readonly TransferMoney transferMoney;

        public TransferMoneyTests()
        {
            // Set up mocks
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationServiceMock = new Mock<INotificationService>();

            // Create instance of class under test with dependencies injected
            transferMoney = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);
        }

        [Fact()]
        public void TransferMoney_Successful()
        {
            Account from = new Account { Id = Guid.NewGuid(), Balance = 500m };
            Account to = new Account { Id = Guid.NewGuid(), Balance = 500m };

            accountRepositoryMock.Setup(r => r.GetAccountById(from.Id)).Returns(from);
            accountRepositoryMock.Setup(r => r.GetAccountById(to.Id)).Returns(to);

            transferMoney.Execute(from.Id, to.Id, 300);
            Assert.Equal(200m, from.Balance);
            Assert.Equal(800m, to.Balance);
        }

        [Fact()]
        public void TransferMoney_InsuffiecientFunds()
        {
            Account from = new Account { Id = Guid.NewGuid(), Balance = 500m };
            Account to = new Account { Id = Guid.NewGuid(), Balance = 500m };

            accountRepositoryMock.Setup(r => r.GetAccountById(from.Id)).Returns(from);
            accountRepositoryMock.Setup(r => r.GetAccountById(to.Id)).Returns(to);
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                transferMoney.Execute(from.Id, to.Id, 600);
            });
            Assert.Equal("Insufficient funds to make transfer",exception.Message);
        }

        [Fact()]
        public void TransferMoney_PayInLimit()
        {
            Account from = new Account { Id = Guid.NewGuid(), Balance = 6000m };
            Account to = new Account { Id = Guid.NewGuid(), Balance = 500m };

            accountRepositoryMock.Setup(r => r.GetAccountById(from.Id)).Returns(from);
            accountRepositoryMock.Setup(r => r.GetAccountById(to.Id)).Returns(to);
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                transferMoney.Execute(from.Id, to.Id, 4001);
            });
            Assert.Equal("Account pay in limit reached", exception.Message);
        }
    }
}