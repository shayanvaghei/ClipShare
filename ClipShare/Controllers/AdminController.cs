using ClipShare.Core.Entities;
using ClipShare.Utility;
using ClipShare.ViewModels;
using ClipShare.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ClipShare.Controllers
{
    [Authorize(Roles = $"{SD.AdminRole}")]
    public class AdminController : CoreController
    {
        public IActionResult Category()
        {
            return View();
        }

        #region API Endpoints

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await UnitOfWork.CategoryRepo.GetAllAsync();
            var toReturn = categories.Select(x => new CategoryAddEdit_vm
            {
                Id = x.Id,
                Name = x.Name,
            });

            return Json(new ApiResponse(200, result: toReturn));
        }

        [HttpPost]
        public async Task<IActionResult> AddEditCategory(CategoryAddEdit_vm model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    UnitOfWork.CategoryRepo.Add(new Category() { Name = model.Name });
                    await UnitOfWork.CompleteAsync();
                    return Json(new ApiResponse(201, "Created", "New Category was added"));
                }
                else
                {
                    var category = await UnitOfWork.CategoryRepo.GetByIdAsync(model.Id);
                    if (category == null) return Json(new ApiResponse(404));

                    var oldName = category.Name;
                    category.Name = model.Name;
                    await UnitOfWork.CompleteAsync();
                    return Json(new ApiResponse(200, "Editted", $"Category of {oldName} has been renamed to {model.Name}"));
                }
            }

            return Json(new ApiResponse(400, message: "Name is required"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await UnitOfWork.CategoryRepo.GetByIdAsync(id);
            if (category != null)
            {
                UnitOfWork.CategoryRepo.Remove(category);
                await UnitOfWork.CompleteAsync();

                return Json(new ApiResponse(200, "Deleted", "Category of " + category.Name + " has been removed"));
            }

            return Json(new ApiResponse(404, message: "The requested category was not found"));
        }
        #endregion
    }
}
