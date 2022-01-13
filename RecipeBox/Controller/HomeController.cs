using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBox.Controllers
{
  public class HomeController : Controller
  {
    private readonly RecipeBoxContext _db;

    public HomeController(RecipeBoxContext db)
    {
      _db = db;
    }


    [HttpGet("/")]
    public ActionResult Index()
    {
      List<Recipe> model = _db.Recipes.Where(entry => entry.Shared == true).OrderByDescending(x => x.Rating).ToList();
      return View(model);
    }

    public ActionResult Details(int id)
    {
      var thisRecipe = _db.Recipes
          .Include(recipe => recipe.JoinIngredients)
          .ThenInclude(join => join.ingredient)
          .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }
  }
}