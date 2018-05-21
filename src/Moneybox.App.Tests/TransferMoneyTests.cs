using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;

namespace Moneybox.App.Tests
{
    [TestClass]
    public class TransferMoneyTests
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

            BUser = new User()
            {
                Name = "Samantha Bloggs",
                Email = "sam@bloggs.com",
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

            testToAcc = new Account()
            {
                Id = toAccId,
                Balance = 10,
                PaidIn = 0,
                User = BUser,
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
            var testTransferMoney = new TransferMoney(mockAccountRepository.Object,mockNotificationService.Object);
            testTransferMoney.Execute(fromAccId, toAccId, 11);
        }

        [TestMethod]
        public void NotifiesFundsLow()
        {
            var testTransferMoney = new TransferMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testTransferMoney.Execute(fromAccId, toAccId, 5);

            mockNotificationService.Verify(x => x.NotifyFundsLow(AUser.Email));
            mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));  
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), ("Account pay in limit reached"))]
        public void ThrowsInvalidOpExceptionIfPayInLimitExceeded()
        {
            testFromAcc.Balance = 1000;
            testToAcc.PaidIn = 3800;
            var testTransferMoney = new TransferMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testTransferMoney.Execute(fromAccId, toAccId, 501);
        }

        [TestMethod]
        public void NotifiesAproachingLimit()
        {
            testToAcc.PaidIn = 3500;
            testFromAcc.Balance = 550;
            var testTransferMoney = new TransferMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testTransferMoney.Execute(fromAccId, toAccId, 250);

            mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(BUser.Email));
            mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TestTransferTransactionCalculatesCorrectNumbers()
        {
            var testTransferMoney = new TransferMoney(mockAccountRepository.Object, mockNotificationService.Object);
            testTransferMoney.Execute(fromAccId, toAccId, 5);

            Assert.AreEqual(15, testToAcc.Balance);
            Assert.AreEqual(5, testFromAcc.Balance);

            Assert.AreEqual(0, testFromAcc.PaidIn);
            Assert.AreEqual(5, testToAcc.PaidIn);

            Assert.AreEqual(5, testFromAcc.Withdrawn);
            Assert.AreEqual(0, testToAcc.Withdrawn);
        }
    }
}
