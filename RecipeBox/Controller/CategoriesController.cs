using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;



namespace RecipeBox.Controllers
{
  [Authorize]
  public class CategoriesController : Controller
  {
    private readonly RecipeBoxContext _db;
    
    private readonly UserManager<ApplicationUser> _userManager;

    public CategoriesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userItems = _db.Categories.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userItems);
    }

    

    public ActionResult Create(string str)
    {
      ViewBag.Same = str;
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Category category, int RecipeId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      category.User = currentUser;
      _db.Categories.Add(category);
      _db.SaveChanges();
      if (RecipeId != 0)
    {
        _db.CategoryRecipe.Add(new CategoryRecipe() { RecipeId = RecipeId, CategoryId = category.CategoryId });
    }
      _db.SaveChanges();
      return RedirectToAction("Index");
    } 
    
      public ActionResult Details(int id)
    {
      var thisCategory = _db.Categories
          .Include(category => category.JoinEntities)
          .ThenInclude(join => join.Recipe)
          .FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    public ActionResult Edit(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost]
    public ActionResult Edit(Category category)
    {
      _db.Entry(category).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      _db.Categories.Remove(thisCategory);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> AddRecipe(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      ViewBag.RecipeId = new SelectList(_db.Recipes.Where(entry => entry.User.Id == currentUser.Id), "RecipeId", "Name");
      return View(thisCategory);
    }

    [HttpPost]
    public ActionResult AddRecipe(Category Category, int RecipeId)
    {
      bool alreadyExists = _db.CategoryRecipe.Any(CategoryRecipe => CategoryRecipe.CategoryId == Category.CategoryId && CategoryRecipe.RecipeId == RecipeId);
      if (RecipeId != 0 && !alreadyExists)
      {
        _db.CategoryRecipe.Add(new CategoryRecipe() { RecipeId = RecipeId, CategoryId = Category.CategoryId });
      }
      _db.SaveChanges();
      if (alreadyExists)
      {
        return RedirectToAction("AddRecipeError");
      }
      return RedirectToAction("Index");
    }

    public ActionResult AddRecipeError()
    {
      return View();
    }

    [HttpPost]
    public ActionResult DeleteRecipe(int joinId, int categoryId)
    {
      var joinEntry = _db.CategoryRecipe.FirstOrDefault(entry => entry.CategoryRecipeId == joinId);
      _db.CategoryRecipe.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = categoryId });
    }

  }
}