using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NuGet.Packaging.Signing;
using Spice.Data;
using Spice.Data.Migrations;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utiliy;
using System.Data;
using System.IO;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class MenuItemController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem()
            };
        }


        //GET INDEX
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();

            return View(menuItems);
        }

        //GET CREATE

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST CREATE

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Image Saving Section

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //Files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extensions = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extensions), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extensions;
            }
            else

            {
                //no file was uploaded , default 
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";

            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        //GET EDIT

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == Id);
            MenuItemVM.SubCategory = await _db.SubCategoryN.Where(s => s.CategoryId == MenuItemVM.MenuItem.Id).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST EDITPOST

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditPOST(int? id)
        {

            {
                if (id == null)
                {
                    return NotFound();
                }
                MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

                if (ModelState.IsValid)
                {
                    MenuItemVM.SubCategory = await _db.SubCategoryN.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                    return View(MenuItemVM);
                }

                //Work on the image saving section

                string webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                if (files.Count > 0)
                {
                    //New Image has been uploaded
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension_new = Path.GetExtension(files[0].FileName);

                    //Delete the original file
                    var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    //we will upload the new file
                    using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }
                    menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
                }

                menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
                menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }

        //GET DETAILS

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == Id);
            MenuItemVM.SubCategory = await _db.SubCategoryN.Where(s => s.CategoryId == MenuItemVM.MenuItem.Id).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET DELETE

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == Id);
            MenuItemVM.SubCategory = await _db.SubCategoryN.Where(s => s.CategoryId == MenuItemVM.MenuItem.Id).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST DeletePOST

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeletePOST(int? id)
        {

            var menuItem = await _db.MenuItem.SingleOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return View();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);
            var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
