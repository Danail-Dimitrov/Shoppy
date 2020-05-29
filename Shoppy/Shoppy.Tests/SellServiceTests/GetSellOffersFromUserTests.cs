using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shoppy.Areas.Sell.Services;
using Shoppy.Data;
using Shoppy.Exceptions;
using Shoppy.Models.DBEntities;
using Shoppy.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoppy.Tests.SellServiceTests
{
    [TestFixture]
    public class GetSellOffersFromUserTests
    {
        private const decimal ProductPrice = 999.99m;
        private const string ProductTitle = "Title";
        private const int Quantity = 1;
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string Email = "email@email.email";
        private const string UserName = "UserName";
        private const string PasswordHash = "PasswordHash";
        private const decimal Money = 999999m;

        protected ApplicationDbContext Context { get; set; }

        protected SellService SellService { get; set; }

        protected int UserId { get; set; }

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Shoppy_Test_Database")
                .Options;

            Context = new ApplicationDbContext(options);

            SellService = new SellService(Context);

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

            UserId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;

            SellOffer sellOffer = new SellOffer
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = ProductTitle,
                Quantity = Quantity,
                UserId = UserId
            };

            this.Context.SellOffers.Add(sellOffer);
            this.Context.SaveChanges();
        }

        [Test]
        public void GetSellOffersFromUser_GetsOffersFromUser_WhenDataIsCorrect()
        {
            List<SellOfferDTO> sellOfferDTOs = this.SellService.GetSellOffersFromUser(UserId);

            Assert.AreEqual(ProductTitle, sellOfferDTOs[0].ProductTitle, "GetSellOffersFromUser does not get SellOffers from User when the data is correct");
        }

        [Test]
        public void GetSellOfferFromUser_ThrowsException_WhenUserIdIsInvalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => this.SellService.GetSellOffersFromUser(-1), "GetSellOfferFromUser does not throw Exception when user id is invalid");
            Assert.That(ex.Message, Is.EqualTo("Id can not be less than or equal to zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetSellOfferFromUser_ThrowsException_WhenUserIsDeleted()
        {
            User user = this.Context.Users.Find(UserId);
            user.IsDeleted = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<UserIsDeletedException>(() => this.SellService.GetSellOffersFromUser(UserId), "GetSellOfferFromUser does not throw Exception when user is deleted");
            Assert.That(ex.Message, Is.EqualTo("The user is deleted"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
