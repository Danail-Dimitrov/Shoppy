using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shoppy.Areas.Sell.Models.DTO;
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
    //READ ME: Due to time limitations all remaining tests for the SellService class will be here. Since almost all validations in this class are other methods, which have been testen in GetSellOffersFromUserTests, EditSellOfferTests and CreateSellOffer tests, again due to time limitations they will not be tested for the rest of the methods that call them   
    public class SellServiceTests
    {
        //Since for every test there is a new db and we always Create a new User and a SellOffer we can be sure that their Ids are = 1
        private const int SellOfferId = 1;
        private const int UserId = 1;

        private const decimal ProductPrice = 999.99m;
        private const string ProductTitle = "Title";
        private const int Quantity = 1;
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string Email = "email@email.email";
        private const string UserName = "UserName";
        private const string PasswordHash = "PasswordHash";
        private const decimal Money = 999999m;
        private const int UseScoreForPostingSellOffer = 5;

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
        public void GetSellOfferById_ReturnsTheCorrectSellOffer_IfDataIsCorrect()
        {
            SellOfferDTO sellOfferDTO = this.SellService.GetSellOfferById(SellOfferId);

            Assert.AreEqual(ProductTitle, sellOfferDTO.ProductTitle, "GetSellOfferById does not retun the correct SellOffer when the data is correct");
        }

        [Test]
        public void GetSellOfferById_ThrowsException_IfUserIsThatOwnsItIsDeleted()
        {
            User user = this.Context.Users.Find(UserId);
            user.IsDeleted = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<UserIsDeletedException>(() => this.SellService.GetSellOfferById(SellOfferId), "GetSellOfferById does not throw Exception when the user that owns the OfferIsDeleted");
            Assert.That(ex.Message, Is.EqualTo("The user is deleted"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void Delete_DeletesSellOffer_IfParametersAreCorrect()
        {

            this.SellService.Delete(SellOfferId, UserId);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);

            Assert.AreEqual(null, sellOffer, "Delete does not delete the SellOffer id the data it recives is correct");
        }

        [Test]
        public void Delete_ThrowsException_IfUserTriesToDeleteOfferThatIsNotHis()
        {
            User user = new User();
            this.Context.Users.Add(user);
            this.Context.SaveChanges();
            
            //Since we are sure that the user created in this test is the second one in the db, we know that his id is = 2
            var ex = Assert.Throws<InvalidOperationException>(() => this.SellService.Delete(SellOfferId, 2), "EditSellOffer does not throw Exception when user tries to edit offer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not delete offers that are not yours"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        //ValidateSellOffer has been indirectly Tested in CreateSellOfferTests, due to lack of time it will not be tested here

        [Test]
        public void IncreaseUserScore_IncreasesTheSuperUserScoreOfUser_IfDataIsCorrect()
        {
            this.SellService.IncreaseUserScore(UserId);

            User user = this.Context.Users.Find(UserId);

            Assert.AreEqual(UseScoreForPostingSellOffer, user.SuperUserScore);
        }

        [Test]
        public void GetBuyOffers_GetsTheByOffersForSellOffer_IfDataIsCorrect()
        {
            List<BuyOffer> buyOffers = new List<BuyOffer>
            {
                new BuyOffer{SellOfferId = SellOfferId, UserId = UserId},
                new BuyOffer{SellOfferId = SellOfferId, UserId = UserId},
                new BuyOffer{SellOfferId = SellOfferId, UserId = UserId},
                new BuyOffer{SellOfferId = SellOfferId, UserId = UserId}
            };

            this.Context.BuyOffers.AddRange(buyOffers);
            this.Context.SaveChanges();

            List<ShowBuyOffersDTO> buyOfferDTOs = this.SellService.GetBuyOffers(SellOfferId);
            int buyOfferId = buyOfferDTOs[2].Id;

            //We can hard code the expected value because we are thaking the 3rd Buy value, for which we know that it will be 3rd in the db
            Assert.AreEqual(buyOfferId, 3, "GetBuyOffers is not correctly getting the BuyOffers for a specific SellOffer");
        }

        [Test]
        public void AcceptBuyOffer_AcceptsBuyOffer_IdDataIsCorrect()
        {
            BuyOffer buyOffer = new BuyOffer 
            {
                SellOfferId = SellOfferId, UserId = UserId 
            };
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.SaveChanges();

            this.SellService.AcceptBuyOffer(1, UserId);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);

            Assert.IsTrue(sellOffer.HasAcceptedBuyOffer, "AcceptBuyOffer does not mark that the SellOffer has accepted a BuyOffer");
            Assert.AreEqual(1, sellOffer.AcceptedBuyOfferId, "AcceptBuyOffer does not store the id of the accepted BuyOffer in the SellOffer");
        }

        [Test]
        public void AcceptBuyOffer_ThrowsException_IfUserAccepsOfferForSellOfferThatHeDoesNotOwn()
        {
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = UserId
            };
            User user = new User();
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.Users.Add(user);
            this.Context.SaveChanges();

            //We are sure what the Ids of the BuyOffer and the second User are, so we can hardcode them
            var ex = Assert.Throws<InvalidOperationException>(() => this.SellService.AcceptBuyOffer(1, 2), "AcceptBuyOffer does not throw Exception when a user tries to accept a BuyOffer for a SellOffer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not accept offers for SellOffers you do not own"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void FinisSale_FinishesSale_IfDataISCorrect()
        {
            decimal offerdMoney = 10m;
            User user = new User { Money = 10m };
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = 2,
                OfferedMoney = offerdMoney
            };
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.Users.Add(user);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);
            sellOffer.AcceptedBuyOfferId = 1;
            sellOffer.HasAcceptedBuyOffer = true;
            this.Context.SaveChanges();

            this.SellService.FinishSale(SellOfferId, UserId);

            Assert.AreEqual(Money + 10m, this.Context.Users.Find(UserId).Money, "FinishSale does not properly increase funds for the seller");
            Assert.AreEqual(null, this.Context.SellOffers.Find(SellOfferId), "FinishSale does not remove the SellOffer");
            Assert.AreEqual(null, this.Context.BuyOffers.Find(1), "FinishSale does not remove the BuyOffer");
        }

        [Test]
        public void FinishSale_ThrowsException_IfBuyerHasInsufficient()
        {
            decimal offerdMoney = 10m;
            User user = new User { Money = 1m };
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = 2,
                OfferedMoney = offerdMoney
            };
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.Users.Add(user);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);
            sellOffer.AcceptedBuyOfferId = 1;
            sellOffer.HasAcceptedBuyOffer = true;
            this.Context.SaveChanges();

            

            var ex = Assert.Throws<BuyerHasInsufficientFundsException>(() => this.SellService.FinishSale(SellOfferId, UserId), "FinishSale does not throw Exception when a the Buyer has InsufficientFunds");
            Assert.That(ex.Message, Is.EqualTo("Buyer has insufficient funds!"), "Wrong Exception messege is showing when an Exception is thrown");
        }
        
        [Test]
        public void FinishSale_SavesTransactionHistoriesAfterSale_IfDataIsCorrect()
        {
            decimal offerdMoney = 10m;
            User user = new User { Money = 10m };
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = 2,
                OfferedMoney = offerdMoney
            };
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.Users.Add(user);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);
            sellOffer.AcceptedBuyOfferId = 1;
            sellOffer.HasAcceptedBuyOffer = true;
            this.Context.SaveChanges();

            this.SellService.FinishSale(SellOfferId, UserId);

            Assert.AreEqual(ProductTitle, this.Context.TransactionHistories.Find(1).ProductTitle, "FinishSale does not properly store TransactionHistories after a sale has been finished");
            Assert.AreEqual(ProductTitle, this.Context.TransactionHistories.Find(2).ProductTitle, "FinishSale does not properly store TransactionHistories after a sale has been finished");
        }

        [Test]
        public void UnAcceptBuyOffer_UnacceptsBuyOffer_IfDataIsCorrect()
        {
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = 2,
            };
            this.Context.BuyOffers.Add(buyOffer);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);
            sellOffer.AcceptedBuyOfferId = 1;
            sellOffer.HasAcceptedBuyOffer = true;
            this.Context.SaveChanges();

            this.SellService.UnaccpetBuyOffer(SellOfferId, UserId);

            Assert.IsFalse(this.Context.SellOffers.Find(SellOfferId).HasAcceptedBuyOffer, "UnAcceptBuyOffer does not mark that the SellOffer has no accepted BuyOffer");

            Assert.AreEqual(0, this.Context.SellOffers.Find(SellOfferId).AcceptedBuyOfferId, "UnAcceptBuyOffer does not mark that the id of the accepted BuyOffer is 0");
        }

        [Test]
        public void UnacceptBuyOffer_ThrowsException_IfUserUnaccepsOfferForSellOfferThatHeDoesNotOwn()
        {
            BuyOffer buyOffer = new BuyOffer
            {
                SellOfferId = SellOfferId,
                UserId = 2,
            };
            User user = new User();
            this.Context.BuyOffers.Add(buyOffer);
            this.Context.Users.Add(user);
            SellOffer sellOffer = this.Context.SellOffers.Find(SellOfferId);
            sellOffer.AcceptedBuyOfferId = 1;
            sellOffer.HasAcceptedBuyOffer = true;
            this.Context.SaveChanges();

            var ex = Assert.Throws<InvalidOperationException>(() => this.SellService.UnaccpetBuyOffer(SellOfferId, 2), "UnacceptBuyOffer does not throw Exception when a user tries to unaccept a BuyOffer for a SellOffer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not unaccept offers for SellOffers you do not own"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
