﻿using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The display order can't exactly match the name");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();

                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(obj => obj.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(obj => obj.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? category = _unitOfWork.Category.Get(obj => obj.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();

            TempData["success"] = "Category deteled successfully";
            return RedirectToAction("Index");
        }
    }
}
