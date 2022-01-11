using System.Collections.Generic;

namespace RecipeBox.Models
{
  public class Recipe
  {
    public Recipe()
    {
      this.JoinEntities = new HashSet<CategoryRecipe>();
    }

    public int RecipeId { get; set; }
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public string Directions { get; set; }
    public int Rating { get; set; }
    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<CategoryRecipe> JoinEntities { get; }
  }
}