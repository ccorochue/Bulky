using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    [Authorize(Roles = SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                })
            };

            if (id == null || id == 0)
            {
                // Create a new product
                return View(productVM);
            }
            else
            {
                // Update a new product
                productVM.Product = _unitOfWork.Product.Get(i => i.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file, int? id)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImageUrl = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImageUrl)) 
                        {
                            System.IO.File.Delete(oldImageUrl);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    };

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (id != null)
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();

                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();

                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                });
                return View(productVM);
            }
        }


        #region API calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Json(new {data =  products});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Product? product = _unitOfWork.Product.Get(i => i.Id == id);

            if (product == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while deleting"
                });
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImageUrl = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImageUrl))
                {
                    System.IO.File.Delete(oldImageUrl);
                }
            }

            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            return Json(new
            {
                success = true,
                message = "Product deleted successfully"
            });
        }

        #endregion
    }
}
