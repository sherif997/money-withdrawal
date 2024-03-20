﻿using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void Withdraw(decimal amount) {
            if (this.Balance - amount < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to withdraw amount");
            }

            this.Balance -= amount;
            this.Withdrawn += amount;
        }

        public void Deposit(decimal amount)
        {
            if (this.PaidIn + amount > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
            this.Balance += amount;
            this.PaidIn += amount;
        }
    }
}
