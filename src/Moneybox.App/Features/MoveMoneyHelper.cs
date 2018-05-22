using System;
using System.Collections.Generic;
using System.Text;
using Moneybox.App.Domain.Services;
using static Moneybox.App.Account;

namespace Moneybox.App.Features
{
    public class MoveMoneyHelper
    {
        private INotificationService notificationService;
        NotifyUser notifyPayInLimit;
        NotifyUser notifyFundsLow;

        public MoveMoneyHelper(INotificationService notificationService)
        {
            this.notificationService = notificationService;
            notifyPayInLimit = new NotifyUser(NotifyPayInLimit);
            notifyFundsLow = new NotifyUser(NotifyFundsLow);
        }

        /// <summary>
        /// I'm going to change the previous implementation of subtacting the "Withdrawn" amount.
        /// It feels more correct to Add to the withdrawn amount as a guage of accumulated activity. Withdrawn != balance...
        /// If, on the otherhand, it would be appropriate for withdrawn and paid in to net against each other, one could consider making this one variable that tracks the net activity of paid/withdrawn breach a limit
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        internal void Transact(Account from, Account to, decimal amount)
        {
            if (!to.CanCredit(amount)) throw new InvalidOperationException("Account pay in limit reached");
            Transact(from, amount);
            to.Credit(amount, notifyPayInLimit);           
        }

        internal void Transact(Account from, decimal amount)        
        {
            if (!from.CanDebit(amount)) throw new InvalidOperationException("Insufficient funds to make transfer");
            from.Debit(amount, notifyFundsLow);
        }

        void NotifyPayInLimit(string email)
        {
            this.notificationService.NotifyApproachingPayInLimit(email);
        }

        void NotifyFundsLow(string email)
        {
            this.notificationService.NotifyFundsLow(email);
        }
    }
}
