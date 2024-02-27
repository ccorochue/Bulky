using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }

        public IActionResult Detail(int id)
        {
            ShoppingCart shoppingCart = new()
            {
                ProductId = id,
                Product = _unitOfWork.Product.Get(i => i.Id == id, includeProperties: "Category"),
                Count = 1
            };

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Detail(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart existShoppingCart = _unitOfWork.ShoppingCart.Get(i => i.ApplicationUserId == userId && i.ProductId == shoppingCart.ProductId);

            if (existShoppingCart != null)
            {
                existShoppingCart.Count += shoppingCart.Count;
                existShoppingCart.Price = ProductPrice(shoppingCart.ProductId, existShoppingCart.Count);
                TempData["success"] = "Product updated successfully on the shopping cart";
            }
            else
            {
                shoppingCart.Id = 0;
                shoppingCart.Price = ProductPrice(shoppingCart.ProductId, shoppingCart.Count);
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                TempData["success"] = "Product added successfully to the shopping cart";
            }

            _unitOfWork.Save();

            return RedirectToAction("index");
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
