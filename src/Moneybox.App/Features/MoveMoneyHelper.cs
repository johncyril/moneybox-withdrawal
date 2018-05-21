using System;
using System.Collections.Generic;
using System.Text;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Features
{
    public class MoveMoneyHelper
    {
        private INotificationService notificationService;

        public MoveMoneyHelper(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        internal void ValidateNewPaidInAmount(Account to, decimal amount)
        {
            var newPainInAmount = to.PaidIn + amount;
            if (newPainInAmount > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (Account.PayInLimit - newPainInAmount < 500m)
            {
                this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }
        }

        internal void ValidateNewFromBalance(Account from, decimal amount)
        {
            var fromBalance = from.Balance - amount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (fromBalance < 500m)
            {
                this.notificationService.NotifyFundsLow(from.User.Email);
            }
        }

        /// <summary>
        /// I'm going to change the previous implementation of subtacting the "Withdrawn" amount.
        /// It feels more correct to Add to the withdrawn amount as a guage of accumulated activity. Withdrawn != balance...
        /// If, on the otherhand, it would be appropriate for withdrawn and paid in to net against each other, one could consider making this one variable that tracks the net activity of paid/withdrawn breach a limit
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        internal void ApplyNewAmounts(Account from, Account to, decimal amount)
        {
            ApplyNewAmounts(from, amount);

            to.Balance = to.Balance + amount;
            to.PaidIn = to.PaidIn + amount;
        }

        internal void ApplyNewAmounts(Account from, decimal amount)
        {
            from.Balance = from.Balance - amount;
            from.Withdrawn = from.Withdrawn + amount;          
        }
    }
}
