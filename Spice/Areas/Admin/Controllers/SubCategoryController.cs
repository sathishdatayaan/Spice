using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utiliy;
using System.Data;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }


        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET Index

        public async Task<IActionResult> Index()
        {
            var subcategories = await _db.SubCategoryN.Include(s => s.Category).ToListAsync();
            return View(subcategories);

        }

        // Get Create

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = new Models.SubCategoryN(),
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {

            var doesSubCategoryExists = _db.SubCategoryN.Include(s => s.Category).Where(s => s.Name == model.SubCategoryN.Name && s.Category.Id == model.SubCategoryN.CategoryId);

            if (doesSubCategoryExists.Count() > 0)
            {
                //Error
                StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";

            }
            else
            {
                _db.SubCategoryN.Add(model.SubCategoryN);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = model.SubCategoryN,
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }


        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategoryN> subCategories = new List<SubCategoryN>();

            subCategories = await (from subCategory in _db.SubCategoryN
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        // Get Edit

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategoryN.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory==null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = subCategory,
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SubCategoryAndCategoryViewModel model)
        {

            var doesSubCategoryExists = _db.SubCategoryN.Include(s => s.Category).Where(s => s.Name == model.SubCategoryN.Name && s.Category.Id == model.SubCategoryN.CategoryId);

            if (doesSubCategoryExists.Count() > 0)
            {
                //Error
                StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";

            }
            else
            {
                var subCatFromDb = await _db.SubCategoryN.FindAsync(model.SubCategoryN.Id);
                subCatFromDb.Name = model.SubCategoryN.Name;
                
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = model.SubCategoryN,
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategoryN.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = subCategory,
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);


           
        }

        // GET DELETE

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategoryN.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryN = subCategory,
                SubCategoryList = await _db.SubCategoryN.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

          
        }

        //POST Delete

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategoryN.SingleOrDefaultAsync(m => m.Id == id);
            _db.SubCategoryN.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }



}
