using Bulky.DataAcess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();

            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.Get(i => i.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company, int? id)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    _unitOfWork.Company.Update(company);
                    _unitOfWork.Save();

                    TempData["success"] = "Company updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();

                    TempData["success"] = "Company created successfully";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(company);
            }
        }

        #region API calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();

            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Company? company = _unitOfWork.Company.Get(i => i.Id == id);

            if (company == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Error while deleting"
                });
            }

            _unitOfWork.Company.Delete(company);
            _unitOfWork.Save();

            return Json(new
            {
                success = true,
                message = "Company deleted successfully"
            });
        }

        #endregion
    }
}
