using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using RecipeBox.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers
{ 
  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> userManager, RecipeBoxContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userRecipes.OrderByDescending(x => x.Rating));
    }

    public async Task<ActionResult> Create()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      ViewBag.CategoryId = new SelectList(_db.Categories.Where(entry => entry.User.Id == currentUser.Id), "CategoryId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe, int CategoryId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      _db.SaveChanges();
      if (CategoryId !=0)
      {
        _db.CategoryRecipe.Add(new CategoryRecipe() { CategoryId = CategoryId, RecipeId = recipe.RecipeId });
      }
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public ActionResult Details (int id)
    {
      var thisRecipe = _db.Recipes
          .Include(recipe => recipe.JoinEntities)
          .ThenInclude(join => join.Category)
          .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult Edit (int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe recipe, int CategoryId)
    {
      if (CategoryId != 0)
      {
        _db.CategoryRecipe.Add(new CategoryRecipe() { CategoryId = CategoryId, RecipeId = recipe.RecipeId });
      }
      _db.Entry(recipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete (int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteCategory(int joinId, int recipeId)
    {
      var joinEntry = _db.CategoryRecipe.FirstOrDefault(entry => entry.CategoryRecipeId == joinId);
      _db.CategoryRecipe.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = recipeId });
    }

    public async Task<ActionResult> AddCategory(int id)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisRecipe = _db.Recipes.FirstOrDefault(recipes => recipes.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories.Where(entry => entry.User.Id == currentUser.Id), "CategoryId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddCategory(Recipe recipe, int CategoryId)
    {
      bool alreadyExists = _db.CategoryRecipe.Any(CategoryRecipe => CategoryRecipe.CategoryId == CategoryId && CategoryRecipe.RecipeId == recipe.RecipeId);
      if (CategoryId != 0 && !alreadyExists)
      {
        _db.CategoryRecipe.Add(new CategoryRecipe() { CategoryId = CategoryId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      if(alreadyExists)
      {
        return RedirectToAction("AddCategoryError");
      }
      return RedirectToAction("Index");
    }
    
    public ActionResult AddCategoryError()
    {
      return View();
    }

    public ActionResult AddIngredient(int id, string str)
    {
      ViewBag.Same = str;
      var thisRecipe = _db.Recipes
          .Include(recipe => recipe.JoinIngredients)
          .ThenInclude(join => join.ingredient)
          .FirstOrDefault(recipes => recipes.RecipeId == id);
      ViewBag.IngredientId = new SelectList(_db.Ingredients, "IngredientId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddIngredient(Recipe recipe, int IngredientId)
    {
      bool alreadyExists = _db.IngredientRecipe.Any(IngredientRecipe => IngredientRecipe.IngredientId == IngredientId && IngredientRecipe.RecipeId == recipe.RecipeId);
      if (IngredientId != 0 && !alreadyExists)
      {
        _db.IngredientRecipe.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId });
      }
      _db.SaveChanges();
      if(alreadyExists)
      {
        return RedirectToAction("AddIngredient", new { id = recipe.RecipeId, str = "This Recipe already contains that ingredient" });
      }
      return RedirectToAction("AddIngredient", new { id = recipe.RecipeId, str = "ok" });
    }

    [HttpPost]
    public ActionResult DeleteIngredient(int joinId, int recipeId)
    {
      var joinEntry = _db.IngredientRecipe.FirstOrDefault(entry => entry.IngredientRecipeId == joinId);
      _db.IngredientRecipe.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = recipeId });
    }

    [HttpPost]
    public ActionResult ShareRecipe(int recipeId)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == recipeId);
      thisRecipe.Shared = !thisRecipe.Shared;
      _db.Entry(thisRecipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new {id = thisRecipe.RecipeId});
    }
  }
}