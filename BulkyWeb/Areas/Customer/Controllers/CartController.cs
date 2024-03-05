using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            shoppingCartVM.OrderHeader.OrderTotal = shoppingCartVM.ShoppingCartList.Sum(i => i.Price * i.Count);

            return View(shoppingCartVM);
        }

        public IActionResult Plus(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(i => i.ProductId == id && i.ApplicationUserId == userId);
            shoppingCart.Count++;
            shoppingCart.Price = ProductPrice(id, shoppingCart.Count);

            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();

            return RedirectToAction("index");
        }

        public IActionResult Minus(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(i => i.ProductId == id && i.ApplicationUserId == userId);

            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
            }
            else
            {
                shoppingCart.Count--;
                shoppingCart.Price = ProductPrice(id, shoppingCart.Count);

                _unitOfWork.ShoppingCart.Update(shoppingCart);
            }

            _unitOfWork.Save();

            return RedirectToAction("index");
        }

        public IActionResult Remove(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(i => i.ProductId == id && i.ApplicationUserId == userId);

            _unitOfWork.ShoppingCart.Delete(shoppingCart);
            _unitOfWork.Save();

            return RedirectToAction("index");
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(i => i.Id == userId);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            shoppingCartVM.OrderHeader.OrderTotal = shoppingCartVM.ShoppingCartList.Sum(i => i.Price * i.Count);

            return View(shoppingCartVM);
        }

        private double? ProductPrice(int productId, int quantity)
        {
            Product product = _unitOfWork.Product.Get(i => i.Id == productId);

            if (quantity <= 50)
            {
                return product.Price;
            }
            else if (quantity <= 100)
            {
                return product.Price50;
            }
            else
            {
                return product.Price100;
            }
        }
    }
}
