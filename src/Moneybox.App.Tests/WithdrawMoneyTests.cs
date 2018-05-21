using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moneybox.App.Tests
{
    [TestClass]
    public class WithdrawMoneyTests
    {

        private User AUser;
        private User BUser;

        private Guid fromAccId;
        private Guid toAccId;

        private Mock<IAccountRepository> mockAccountRepository;
        private Mock<INotificationService> mockNotificationService;

        private Account testFromAcc;
        private Account testToAcc;

        [TestInitialize]
        public void setUp()
        {
            AUser = new User()
            {
                Name = "Joe Bloggs",
                Email = "joe@bloggs.com",
                Id = Guid.NewGuid()
            };

            fromAccId = Guid.NewGuid();
            toAccId = Guid.NewGuid();

            testFromAcc = new Account()
            {
                Id = fromAccId,
                Balance = 10,
                PaidIn = 0,
                User = AUser,
                Withdrawn = 0
            };

            mockAccountRepository = new Mock<IAccountRepository>();
            mockNotificationService = new Mock<INotificationService>();
            mockAccountRepository.Setup(x => x.GetAccountById(fromAccId)).Returns(testFromAcc);
            mockAccountRepository.Setup(x => x.GetAccountById(toAccId)).Returns(testToAcc);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), ("Insufficient funds to make transfer"))]
        public void ThrowsInvalidOpExceptionIfInsufficientFunds()
        {
            var testWithdrawrMoney = new WithdrawMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testWithdrawrMoney.Execute(fromAccId, 11);
        }

        [TestMethod]
        public void NotifiesFundsLow()
        {
            var testWithdrawrMoney = new WithdrawMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testWithdrawrMoney.Execute(fromAccId, 5);

            mockNotificationService.Verify(x => x.NotifyFundsLow(AUser.Email));
            mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Once());
        }

    }
}
