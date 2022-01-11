namespace RecipeBox.Models
{
  public class IngredientRecipe
  {
    public int IngredientRecipeId { get; set; }
    public int IngredientId { get; set; }
    public int RecipeId { get; set; }
    public virtual Ingredient ingredient { get; set; }
    public virtual Recipe recipe { get; set; }
  }
}