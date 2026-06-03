using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this ExpenseType expenseType)
        {
            return expenseType switch
            {
                ExpenseType.InternationalFreight => "Flete Internacional",
                ExpenseType.InternationalInsurance => "Seguro Internacional",
                ExpenseType.PortExpenses => "Gastos Portuarios",
                ExpenseType.LocalTransport => "Transporte Local",
                ExpenseType.CustomsBrokerFees => "Honorarios Aduanales",
                ExpenseType.Storage => "Almacenaje",
                ExpenseType.Handling => "Manejo de Carga",
                ExpenseType.OtherExpenses => "Otros Gastos",
                _ => expenseType.ToString()
            };
        }

        public static string ToDisplayName(this ApportionmentMethod apportionmentMethod)
        {
            return apportionmentMethod switch
            {
                ApportionmentMethod.ByValue => "Por Valor",
                ApportionmentMethod.ByWeight => "Por Peso",
                ApportionmentMethod.ByVolume => "Por Volumen",
                ApportionmentMethod.ByQuantity => "Por Cantidad",
                _ => apportionmentMethod.ToString()
            };
        }

        public static string ToDisplayName(this TransportMode transportMode)
        {
            return transportMode switch
            {
                TransportMode.Maritime => "Marítimo",
                TransportMode.Air => "Aéreo",
                TransportMode.Land => "Terrestre",
                _ => transportMode.ToString()
            };
        }
    }
}
