using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moq;
using System;
using Xunit;

namespace Moneybox.App.Features.Tests
{
    public class WithdrawMoneyTests
    {
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<INotificationService> notificationServiceMock;
        private WithdrawMoney withdrawMoney;

        public WithdrawMoneyTests()
        {
            // Set up mocks
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationServiceMock = new Mock<INotificationService>();

            // Create instance of class under test with dependencies injected
            withdrawMoney = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);
        }
        [Fact()]
        public void WithdrawMoney_Successful()
        {
            Account from = new Account { Id = Guid.NewGuid(), Balance = 500m };

            accountRepositoryMock.Setup(r => r.GetAccountById(from.Id)).Returns(from);

            withdrawMoney.Execute(from.Id, 300);
            Assert.Equal(200m, from.Balance);
        }

        [Fact()]
        public void WithdrawMoney_InsufficientFunds()
        {
            Account from = new Account { Id = Guid.NewGuid(), Balance = 500m };

            accountRepositoryMock.Setup(r => r.GetAccountById(from.Id)).Returns(from);
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                withdrawMoney.Execute(from.Id, 600);
            });
            Assert.Equal("Insufficient funds to withdraw amount", exception.Message);
  
            
        }
    }
}