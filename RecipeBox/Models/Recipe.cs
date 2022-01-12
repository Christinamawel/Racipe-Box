using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.JoinEntities = new HashSet<CategoryRecipe>();
      this.JoinIngredients = new HashSet<IngredientRecipe>();
      this.Shared = false;
    }

    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Directions { get; set; }
    public int Rating { get; set; }
    public virtual ApplicationUser User { get; set; }
    public bool Shared { get; set; }

    public virtual ICollection<CategoryRecipe> JoinEntities { get; }
    public virtual ICollection<IngredientRecipe> JoinIngredients { get; }
  }
}