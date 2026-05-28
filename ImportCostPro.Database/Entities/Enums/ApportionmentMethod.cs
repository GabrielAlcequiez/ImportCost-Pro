namespace ImportCostPro.Database.Entities.Enums
{
    public enum ApportionmentMethod
    {
        ByValue = 1,    // Method to apportion costs based on the value of the items
        ByWeight = 2,   // Method to apportion costs based on the weight of the items
        ByVolume = 3,  // Method to apportion cost based on the volume of the items
        ByQuantity = 4, // Method to apportion costs based on the quantity of the items
    }
}