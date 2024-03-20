﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            // Get account to withdraw from using provided Id, specify type to improve readability
            Account from = this.accountRepository.GetAccountById(fromAccountId);

        
            from.Withdraw(amount);
            if(from.Balance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to withdraw amount");
            }

            if (from.Balance < 500m)
            {
                this.notificationService.NotifyFundsLow(from.User.Email);
            }
            this.accountRepository.Update(from);


        }
    }
}
