namespace UnitySC.Equipment.Abstractions
{
    public class RecipeEventArgs : System.EventArgs
    {
        public RecipeEventArgs(string recipeName)
        {
            RecipeName = recipeName;
        }

        public string RecipeName { get; }
    }
}
