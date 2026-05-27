namespace ImportCostPro.Database.Entities.Enums
{
    public enum ExpenseType
    {
        InternationalFreight = 1,     // Flete internacional (CIF calculation)
        InternationalInsurance = 2,    // Seguro internacional (CIF calculation)
        PortExpenses = 3,              // Gastos portuarios
        LocalTransport = 4,            // Transporte local
        CustomsBrokerFees = 5,         // Honorarios aduanales
        Storage = 6,                   // Almacenaje
        Handling = 7,                  // Manejo de carga
        OtherExpenses = 8              // Otros gastos
    }
}
