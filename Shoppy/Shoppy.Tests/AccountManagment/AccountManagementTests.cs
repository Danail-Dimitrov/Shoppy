using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shoppy.Areas.AccountManagement.Models.DTO;
using Shoppy.Areas.AccountManagement.Services;
using Shoppy.Data;
using Shoppy.Exceptions;
using Shoppy.Models.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoppy.Tests.AccountManagment
{
    [TestFixture]
    public class AccountManagementTests
    {
        private const int UserId = 1;

        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string Email = "email@email.email";
        private const string UserName = "UserName";
        private const string PasswordHash = "PasswordHash";
        private const decimal Money = 999999m;

        protected ApplicationDbContext Context { get; set; }

        protected AccountManagementService AccountManagementService { get; set; }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Shoppy_Test_Database")
                .Options;

            Context = new ApplicationDbContext(options);

            AccountManagementService = new AccountManagementService(Context, new FakeSignInManager());

            User user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                UserName = UserName,
                PasswordHash = PasswordHash,
                Money = Money
            };

            this.Context.Users.Add(user);
            this.Context.SaveChanges();
        }

        [Test]
        public void AddFunds_AddsFundsToUser_IfDataIsCorrect()
        {
            decimal funds = 10m;
            AddFundsDTO addFundsDTO = new AddFundsDTO
            {
                Money = funds
            };

            this.AccountManagementService.AddFunds(UserId, addFundsDTO);

            User user = this.Context.Users.Find(UserId);
            Assert.AreEqual(Money + funds, user.Money, "AddFunds is not adding funds when data is valid");
        }

        [Test]
        public void AddFunds_ThrowsException_IfAddFundsDTOIsInvalid()
        {
            AddFundsDTO addFundsDTO = new AddFundsDTO
            {
                Money = -1
            };

            var ex = Assert.Throws<ArgumentException>(() => this.AccountManagementService.AddFunds(UserId, addFundsDTO), "AddFunds does not throw Exception when AddFundsDTO is invalid");
            Assert.That(ex.Message, Is.EqualTo("The Amount of money that you add can not be less than or equal to zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void AddFunds_ThrowsException_IFUserIdIsInvalid()
        {
            decimal funds = 10m;
            AddFundsDTO addFundsDTO = new AddFundsDTO
            {
                Money = funds
            };

            var ex = Assert.Throws<ArgumentException>(() => this.AccountManagementService.AddFunds(-1, addFundsDTO), "AddFunds does not throw Exception when user id is invalid");
            Assert.That(ex.Message, Is.EqualTo("User id can not be less than zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void AddFunds_ThrowsException_IFUserIsDeleted()
        {
            decimal funds = 10m;
            AddFundsDTO addFundsDTO = new AddFundsDTO
            {
                Money = funds
            };
            User user = this.Context.Users.Find(UserId);
            user.IsDeleted = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<UserIsDeletedException>(() => this.AccountManagementService.AddFunds(UserId, addFundsDTO), "AddFunds does not throw Exception when user is deleted");
            Assert.That(ex.Message, Is.EqualTo("The user is deleted"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void AddFunds_ThrowsException_IFUserIsNull()
        {
            decimal funds = 10m;
            AddFundsDTO addFundsDTO = new AddFundsDTO
            {
                Money = funds
            };

            var ex = Assert.Throws<UserIsNullException>(() => this.AccountManagementService.AddFunds(2, addFundsDTO), "AddFunds does not throw Exception when user is null");
            Assert.That(ex.Message, Is.EqualTo("User is null"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetUserInfo_RetursnCorrectAccountIfno_IfDataIsCorrect()
        {
            AccountInfoDTO accountInfoDTO = this.AccountManagementService.GetUserInfo(UserId);
           
            Assert.AreEqual(0, accountInfoDTO.SuperUserScore, "GetUserInfo does not correclty return superUserScore");
            Assert.AreEqual(Money, accountInfoDTO.Money, "GetUserInfo does not correclty return Money");
        }

        [Test]
        public void Delete_DeletesUser_IfDataIsCorrect()
        {
            this.AccountManagementService.Delete(UserId);

            User user = this.Context.Users.Find(UserId);
            Assert.IsTrue(user.IsDeleted, "Delete does not delete user");
        }   

        [Test]
        public void GetuserById_ReturnsUserDTOWithCorrectData_IfRecivedDataIsCorrect()
        {
            UserDTO userDTO = this.AccountManagementService.GetUserById(UserId);

            Assert.AreEqual(UserName, userDTO.UserName, "GetuserById does not return DTO with correct data if the daat recived is valid");
        }

        [Test]
        public void RemoveAllOrders_RemovesOffersFromUserAfterHeIsDeleted()
        {
            SellOffer sellOffer = new SellOffer
            {
                UserId = UserId,
            };
            BuyOffer buyOffer = new BuyOffer
            {
                UserId = UserId,
            };
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.SellOffers.Add(sellOffer);
            this.Context.SaveChanges();

            this.AccountManagementService.RemoveAllOrders(UserId);

            List<BuyOffer> buyOffers = this.Context.BuyOffers.Where(x => x.Id == UserId).ToList();
            List<SellOffer> sellOffers = this.Context.SellOffers.Where(x => x.Id == UserId).ToList();

            Assert.AreEqual(0, buyOffers.Count, "RemoveAllOrders does nto remove user's buy offers after he is deleted");
            Assert.AreEqual(0, sellOffers.Count, "RemoveAllOrders does nto remove user's buy offers after he is deleted");
        }

        [Test]
        public void GetTransactionHistories_ReturnsTransactionHistories_IfDataIsCorrect()
        {
            string productTitle = "productTitle";
            TransactionHistory transactionHistory = new TransactionHistory
            {
                ProductTitle = productTitle,
                UserId = UserId
            };
            this.Context.TransactionHistories.Add(transactionHistory);
            this.Context.SaveChanges();

            List<TransactionHistoryDTO> transactionHistoryDTOs = this.AccountManagementService.GetTransactionHistories(UserId);

            Assert.AreEqual(productTitle, transactionHistoryDTOs[0].ProductTitle, "GetTransactionHistories does not return TransactionHistoryDTO correctly when data is valid");
        }

        [Test]
        public void GetProfit_ReturnsProfitCorrectly_IfDataISCorrect()
        {
            decimal profit = 5m;
            string productTitle = "productTitle";
            TransactionHistory transactionHistory = new TransactionHistory
            {
                ProductTitle = productTitle,
                UserId = UserId,
                IsProvit = true,
                MoneyAmaount = profit
            };
            this.Context.TransactionHistories.Add(transactionHistory);
            this.Context.SaveChanges();

            decimal totalProfit = this.AccountManagementService.GetProfit(UserId);

            Assert.AreEqual(profit, totalProfit, "GetProfit does not return profit correcly");
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
