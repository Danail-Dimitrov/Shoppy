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
    public class CreateSellOfferTests
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
        }

        [Test]
        public void CreateSellOffer_SavesTheOffer_IfDataIsCorrect()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = ProductTitle,
                Quantity = Quantity
            };

            int userId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;

            this.SellService.CreateSellOffer(sellOfferDTO, userId);

            Assert.AreEqual(ProductTitle, this.Context.SellOffers.Where(x => x.UserId == userId).FirstOrDefault().ProductTitle, "CreateSellOffer does not create SellOffer when the data is corect");
        }

        [Test]
        public void CreateSellOffer_ThrowsArgumentException_WhenSellOfferDTOsProductPriceIsInvalid()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = -1m,
                ProductTitle = ProductTitle,
                Quantity = Quantity
            };

            int userId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;

            var ex = Assert.Throws<ArgumentException>(() => this.SellService.CreateSellOffer(sellOfferDTO, userId), "CreateSellOffer does not throw Exception when data is incorect");
            Assert.That(ex.Message, Is.EqualTo("The price for a single product can not be zero or negative or greter than 9999999999"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void CreateSellOffer_ThrowsArgumentException_WhenSellOfferDTOsProductTitleIsInvalid()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = "ds",
                Quantity = Quantity
            };

            int userId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;

            var ex = Assert.Throws<ArgumentException>(() => this.SellService.CreateSellOffer(sellOfferDTO, userId), "CreateSellOffer does not throw Exception when data is incorect");
            Assert.That(ex.Message, Is.EqualTo("The Titel of a product can not shorter than 3 charecters or grater than 999999"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void CreateSellOffer_ThrowsArgumentException_WhenUseIdIsInvalid()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = ProductTitle,
                Quantity = Quantity
            };


            var ex = Assert.Throws<ArgumentException>(() => this.SellService.CreateSellOffer(sellOfferDTO, -1), "CreateSellOffer does not throw Exception when userId is invalid");
            Assert.That(ex.Message, Is.EqualTo("Id can not be less than or equal to zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void CreateSellOffer_ThrowsArgumentException_WhenUserIsDeleted()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                HasAcceptedBuyOffer = false,
                CanReciveBuyOffers = true,
                PriceIsNegotiable = true,
                ProductPrice = ProductPrice,
                ProductTitle = "ds",
                Quantity = Quantity
            };

            int userId = this.Context.Users.Where(x => x.FirstName == FirstName).FirstOrDefault().Id;
            User user = this.Context.Users.Find(userId);
            user.IsDeleted = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<UserIsDeletedException>(() => this.SellService.CreateSellOffer(sellOfferDTO, userId), "CreateSellOffer does not throw Exception when user is Deleted");
            Assert.That(ex.Message, Is.EqualTo("The user is deleted"), "Wrong Exception messege is showing when an Exception is thrown");
        }


        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
