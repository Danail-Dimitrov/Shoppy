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
    public class EditSellOfferTests
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
        private const string NewProductTitle = "New Product Title";

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
                UserId = UserId,
                TotalPrice = Quantity * ProductPrice
            };

            this.Context.SellOffers.Add(sellOffer);
            this.Context.SaveChanges();
        }

        [Test]
        public void EditSellOffer_EditsTheSellOffer_IfDataIsCorrect()
        {
            SellOffer sellOffer = this.Context.SellOffers.Where(x => x.ProductTitle == ProductTitle).FirstOrDefault();

            SellOfferDTO sellOfferDTO = new SellOfferDTO(sellOffer.Id, sellOffer.ProductTitle, sellOffer.ProductDescription, sellOffer.ProductPrice, sellOffer.TotalPrice, sellOffer.PriceIsNegotiable, sellOffer.CanReciveBuyOffers, sellOffer.Quantity, null, sellOffer.HasAcceptedBuyOffer);

            sellOfferDTO.ProductTitle = NewProductTitle;

            this.SellService.EditSellOffer(sellOfferDTO, this.UserId);

            Assert.AreEqual(NewProductTitle, this.Context.SellOffers.Find(sellOfferDTO.Id).ProductTitle, "EditSellOffer does not edit the SellOffer if the data is correct");
        }

        [Test]
        public void EditSellOffer_ThrowsException_WhenNonExsitingUsertriesToEditAnOffer()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                Id = 1
            };

            var ex = Assert.Throws<UserIsNullException>(() => this.SellService.EditSellOffer(sellOfferDTO, 2), "EditSellOffer does not throw Exception when user that does not exist tries to edit Offer ");
            Assert.That(ex.Message, Is.EqualTo("The user is null"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void EditSellOffer_ThrowsException_WhenUserTriesTOEditOfferThatIsNotHis()
        {
            SellOfferDTO sellOfferDTO = new SellOfferDTO
            {
                Id = 1,
                Quantity = Quantity,
                ProductTitle = ProductTitle,
                ProductPrice = ProductPrice
            };
            User user = new User();
            this.Context.Users.Add(user);

            var ex = Assert.Throws<InvalidOperationException>(() => this.SellService.EditSellOffer(sellOfferDTO, 2), "EditSellOffer does not throw Exception when user tries to edit offer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not edit offers that are not yours"), "Wrong Exception messege is showing when an Exception is thrown");
        }
        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
