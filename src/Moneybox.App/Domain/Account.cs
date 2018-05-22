using System;

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

        internal void Debit(decimal amount, NotifyUser callback)
        {
            Balance = Balance - amount;
            Withdrawn = Withdrawn + amount;

            if (Balance < 500m)
            {
                callback(User.Email);
            }
        }

        internal void Credit(decimal amount, NotifyUser callback)
        {
            Balance = Balance + amount;
            PaidIn = PaidIn + amount;

            if (PayInLimit - PaidIn < 500m)
            {
               callback(User.Email);
            }
        }

        internal bool CanDebit(decimal amount)
        {
            if (Balance - amount < 0m) return false;
            return true;
        }

        internal bool CanCredit(decimal amount)
        {
            if (PaidIn + amount > PayInLimit) return false;
            return true;
        }

        public delegate void NotifyUser(string email);
    }
}
