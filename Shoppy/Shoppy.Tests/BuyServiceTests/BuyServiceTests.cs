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
        private const bool PriceIsNegotiable = true;

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
                PriceIsNegotiable = PriceIsNegotiable,
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
        public void CreateBuyOffer_ThrowsException_IfUserIsDeleted()
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

        [Test]
        public void GetRandomSellOffers_GetsSellOffers_IfDataIsCorrect()
        {
            int numberOfOffersPerPage = 2;

            SellOffer sellOffer = new SellOffer
            {
                UserId = UserId,
                CanReciveBuyOffers = true
            };
            User user = new User
            {
                Money = 99999m,
                IsDeleted = false
            };
            this.Context.SellOffers.Add(sellOffer);
            this.Context.Users.Add(user);
            this.Context.SaveChanges();


            List<SellOfferDTO> sellOfferDTOs = this.BuyService.GetRandomSellOffers(numberOfOffersPerPage, 2);

            Assert.AreEqual(2, sellOfferDTOs.Count, "GetRandomSellOffers does ot return the correctamount of offers");
        }

        [Test]
        public void GetRandomSellOffers_NotReturnOffersOwnedByTheCurrentUseer()
        {
            int numberOfOffersPerPage = 1;
            User user = new User
            {
                Money = 99999m,
                IsDeleted = false
            };
            this.Context.Users.Add(user);
            this.Context.SaveChanges();

            List<SellOfferDTO> sellOfferDTOs = this.BuyService.GetRandomSellOffers(numberOfOffersPerPage, UserId);

            Assert.AreEqual(0, sellOfferDTOs.Count, "GetRandomSellOffers returns offers owned by the current user");
        }

        [Test]
        public void GetRandomSellOffers_ThrowsException_IfNumberOfOffersPerPageIsInvalid()
        {
            var ex = Assert.Throws<ArgumentException>(() => this.BuyService.GetRandomSellOffers(-1, UserId), "GetRandomSellOffers does not throw Exception when number of offers per page is invalud");
            Assert.That(ex.Message, Is.EqualTo("The number of sell offers per page can not be less than or eaqual to zero"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetSellOfferById_ReturnsSellOffer_IfIdIsCorrect()
        {
            SellOfferDTO sellOfferDTO = this.BuyService.GetSellOfferById(SellOfferId);

            Assert.AreEqual(ProductTitle, sellOfferDTO.ProductTitle, "GetSellOfferById does not get SellOffer when Data is correct");
        }

        [Test]
        public void IncreaseUserScore_IncreasesUserScore_IfDataIsCorrect()
        {
            int scoreForPostingBuyoffer = 1;

            this.BuyService.IncreaseUserScore(UserId);

            User user = this.Context.Users.Find(UserId);
            Assert.AreEqual(scoreForPostingBuyoffer, user.SuperUserScore, "IncreaseUserScore does not increase score when data is valid");
        }

        [Test]
        public void GetBuyOfferById_ReturnsBuyOfferDTO_IfDataIsCorrect()
        {
            BuyOfferDTO buyOfferDTO = this.BuyService.GetBuyOfferById(BuyOfferId);

            Assert.AreEqual(OfferedMoney, buyOfferDTO.OfferedMoney, "GetBuyOfferById does not return the correcr BuyOffer whe data is valid");
        }

        [Test]
        public void GetAskedMoney_ReturnsTotalPriceOfSellOffer_IfDataIsCorrect()
        {
            decimal askedMone = this.BuyService.GetAskedMoney(SellOfferId);

            Assert.AreEqual(ProductPrice, askedMone, "GetAskedMoney does not return Total Price Of SellOffer when data is valid");
        }

        [Test]
        public void EditBuyOffer_EditsBuyOffer_IfDataIsCorrect()
        {
            decimal newOfferedMoney = 888m;
            EditBuyOfferDTO editBuyOfferDTO = new EditBuyOfferDTO
            {
                Id = BuyOfferId,
                MoneyOffered = newOfferedMoney
            };

            this.BuyService.EditBuyOffer(editBuyOfferDTO, UserId);
            BuyOffer buyOffer = this.Context.BuyOffers.Find(BuyOfferId);

            Assert.AreEqual(newOfferedMoney, buyOffer.OfferedMoney, "EdditBuyOffer does not update the buy offer when data is valid");
        }

        [Test]
        public void EditBuyOffer_ThrowsExcepion_IfEditBuyOfferdtoIsInvalid()
        {
            EditBuyOfferDTO editBuyOfferDTO = new EditBuyOfferDTO
            {
                Id = BuyOfferId,
                MoneyOffered = -1m
            };

            var ex = Assert.Throws<ArgumentException>(() => this.BuyService.EditBuyOffer(editBuyOfferDTO, UserId), "EditBuyOffer does not throw Exception when EditBuyOfferDTO is invalid");
            Assert.That(ex.Message, Is.EqualTo("The amout of money offered must be betwen 0.01 and 9999999999"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void EditBuyOffer_ThrowsException_IfUserTriesToEditOfferThatIsNotHis()
        {
            decimal newOfferedMoney = 888m;
            EditBuyOfferDTO editBuyOfferDTO = new EditBuyOfferDTO
            {
                Id = BuyOfferId,
                MoneyOffered = newOfferedMoney
            };
            User user = new User
            {
                IsDeleted = false
            };
            this.Context.Users.Add(user);
            this.Context.SaveChanges();

            var ex = Assert.Throws<InvalidOperationException>(() => this.BuyService.EditBuyOffer(editBuyOfferDTO, 2), "EditBuyOffer does not throw Exception when user tries to edit BuyOffer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not edit offers that are not yours"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetBuyOfferWithTitelByIndex_ReturnsDTO_IfDataIsCorrect()
        {
            BuyOfferWithTitelDTO buyOfferWithTitelDTO = this.BuyService.GetBuyOfferWithTitelByIndex(BuyOfferId);

            Assert.AreEqual(ProductTitle, buyOfferWithTitelDTO.ProductTitle, "GetBuyOfferWithTitelByIndex does not correctly redurn BuyOfferWithTitelDTO when data is valid");
        }

        [Test]
        public void DeleteBuyOffer_DeletesBuyOffer_IfDataIsCorrect()
        {
            this.BuyService.DeleteBuyOffer(BuyOfferId, UserId);

            BuyOffer buyOffer = this.Context.BuyOffers.Find(BuyOfferId);

            Assert.IsNull(buyOffer, "DeleteBuyOffer does not deleted BuyOffer when data is valid");
        }
        
        [Test]
        public void DeleteBuyOffer_ThrowsException_UserTriesToDeleteOfferThatIsNotHis()
        {
            User user = new User
            {
                IsDeleted = false
            };
            this.Context.Users.Add(user);
            this.Context.SaveChanges();

            var ex = Assert.Throws<InvalidOperationException>(() => this.BuyService.DeleteBuyOffer(BuyOfferId, 2), "DeleteBuyOffer does not throw Exception when user tries to delete BuyOffer that is not his");
            Assert.That(ex.Message, Is.EqualTo("You can not delete offers that are not yours"), "Wrong Exception messege is showing when an Exception is thrown");
        }

        [Test]
        public void GetSellOffersByName_GetsOffers_IfDataIsCorrect()
        {
            GetSellOfferByNameDTO getSellOfferByNameDTO = new GetSellOfferByNameDTO
            {
                ProductTitle = ProductTitle
            };
            User user = new User
            {
                IsDeleted = false
            };
            this.Context.Users.Add(user);
            this.Context.SaveChanges();

            List<SellOfferDTO> sellOfferDTOs = this.BuyService.GetSellOffersByName(getSellOfferByNameDTO, 2);

            Assert.AreEqual(SellOfferId, sellOfferDTOs[0].Id, "GetSellOffersByName does not return the sell offers that match the critiria");
        }

        [Test]
        public void GetSellOffersByName_DoesNotGetOffersFroCurrentUser()
        {
            GetSellOfferByNameDTO getSellOfferByNameDTO = new GetSellOfferByNameDTO
            {
                ProductTitle = ProductTitle
            };

            List<SellOfferDTO> sellOfferDTOs = this.BuyService.GetSellOffersByName(getSellOfferByNameDTO, UserId);

            Assert.AreEqual(0, sellOfferDTOs.Count, "GetSellOffersByName returns offers owned by the current user");
        }

        [Test]
        public void GetIsThePriceNegotiable_ReturnsCorrecBoolean_IfDataIsCorrect()
        {
            bool proceIsNegotiable = this.BuyService.GetIsThePriceNegotiable(SellOfferId);

            Assert.AreEqual(PriceIsNegotiable, proceIsNegotiable, "GetIsThePriceNegotiable does not correclty return IsThePriceNegotiable when data is valid");
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
        }
    }
}
