using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shoppy.Areas.Buy.Models.DTO;
using Shoppy.Areas.Buy.Services;
using Shoppy.Data;
using Shoppy.Exceptions;
using Shoppy.Models.DBEntities;
using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoppy.Tests.BuyServiceTests
{
    [TestFixture]
    public class BuyServiceTests
    {
        private const int BuyOfferId = 1;
        private const int SellOfferId = 1;
        private const int UserId = 1;

        private const decimal ProductPrice = 999.99m;
        private const string ProductTitle = "Title";
        private const int Quantity = 1;
        private const decimal OfferedMoney = 999.99m;
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string Email = "email@email.email";
        private const string UserName = "UserName";
        private const string PasswordHash = "PasswordHash";
        private const decimal Money = 999999m;

        protected ApplicationDbContext Context { get; set; }

        protected BuyService BuyService { get; set; }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Shoppy_Test_Database")
                .Options;

            Context = new ApplicationDbContext(options);

            BuyService = new BuyService(Context);

            User user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                UserName = UserName,
                PasswordHash = PasswordHash,
                Money = Money
            };
            SellOffer sellOffer = new SellOffer
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = ProductTitle,
                Quantity = Quantity,
                UserId = UserId,
                TotalPrice = Quantity * ProductPrice
            };
            BuyOffer buyOffer = new BuyOffer
            {
                OfferedMoney = OfferedMoney,
                UserId = UserId,
                SellOfferId = SellOfferId
            };

            this.Context.SellOffers.Add(sellOffer);
            this.Context.Users.Add(user);
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.SaveChanges();
        }

        [Test]
        public void CreateBuyOffer_CreatsBuyOffer_IfDataIsCorrect()
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO
            {
                OfferedMoney = OfferedMoney,
                SellOfferId = SellOfferId
            };

            this.BuyService.CreateBuyOffer(buyOfferDTO, UserId);

            BuyOffer buyOffer = this.Context.BuyOffers.Find(BuyOfferId);

            //Since we already create BuyOffer in the start up this is the second one, it's it is = 2
            Assert.AreEqual(OfferedMoney, buyOffer.OfferedMoney, "CreateBuyOffer does not create BuyOffers corectly");
        }

        [Test]
        public void CreateBuyOffer_ThrowsArgumentException_WhenBuyOfferDTOsProductPriceIsInvalid()
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO
            {
                OfferedMoney = -1m,
                SellOfferId = SellOfferId
            };

            var ex = Assert.Throws<ArgumentException>(() => this.BuyService.CreateBuyOffer(buyOfferDTO, UserId), "CreateBuyOffer does not throw Exception when data is incorect");
            Assert.That(ex.Message, Is.EqualTo("The amout of money offered must be betwen 0.01 and 9999999999"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void CreateBuyOffer_ThrowsException_IfUserIdIsInvalid()
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO
            {
                OfferedMoney = OfferedMoney,
                SellOfferId = SellOfferId
            };

            var ex = Assert.Throws<ArgumentException>(() => this.BuyService.CreateBuyOffer(buyOfferDTO, -1), "CreateBuyOffer does not throw Exception when userId is invalid");
            Assert.That(ex.Message, Is.EqualTo("User id can not be less than or equal zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void CreateBuyOffer_THrowsException_IfUserIsDeleted()
        {
            BuyOfferDTO buyOfferDTO = new BuyOfferDTO
            {
                OfferedMoney = OfferedMoney,
                SellOfferId = SellOfferId
            };

            int userId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;
            User user = this.Context.Users.Find(userId);
            user.IsDeleted = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<UserIsDeletedException>(() => this.BuyService.CreateBuyOffer(buyOfferDTO, userId), "CreateBuyOffer does not throw Exception when user is Deleted");
            Assert.That(ex.Message, Is.EqualTo("The user is deleted"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetBuyOffersFromUser_GetsBuyOffers_IfDataIsCorrect()
        {
            List<BuyOfferWithTitelDTO> buyOfferWithTitelDTOs = this.BuyService.GetBuyOffersFromUser(UserId);

            Assert.AreEqual(OfferedMoney, buyOfferWithTitelDTOs[0].OfferedMoney, "GetBuyOffersFromUser does not correctly get the BuyOffers from a a user");
        }

        public void GetRandomSellOffers_GetsSellOffers_IfDataIsCorrect()
        {
            decimal offerMoney = 9m;
            BuyOffer buyOffer = new BuyOffer
            {
                UserId = UserId,
                OfferedMoney = offerMoney
            };

        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
