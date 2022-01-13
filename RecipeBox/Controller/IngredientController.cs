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
  public class IngredientController : Controller
  {
    private readonly RecipeBoxContext _db;

    public IngredientController(RecipeBoxContext db)
    {
      _db = db;
    }

    public ActionResult Index()
    {
      List<Ingredient> model = _db.Ingredients.OrderBy(x => x.Name).ToList();
      return View(model);
    }

    [HttpPost]
    public ActionResult Create(string name)
    {
      bool alreadyExists = _db.Ingredients.Any(x => x.Name == name);
      if(!alreadyExists)
      {
        Ingredient newIng = new Ingredient();
        newIng.Name = name.ToLower();
        _db.Ingredients.Add(newIng);
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisIng = _db.Ingredients
          .Include(ing => ing.JoinEntities)
          .ThenInclude(join => join.recipe)
          .FirstOrDefault(ing => ing.IngredientId == id);
      return View(thisIng);
    }

    public ActionResult Delete (int id)
    {
      var thisIng = _db.Ingredients.FirstOrDefault(ing => ing.IngredientId == id);
      return View(thisIng);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisIng = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      _db.Ingredients.Remove(thisIng);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}