namespace ImportCostPro.Database.Entities.Enums
{
    public enum ImportOrderStatus
    {
        Open = 1,        // Abierta: Estado inicial. La orden puede editarse y se le pueden agregar productos y gastos.
        Calculated = 2,  // Calculada: La orden ya tiene un cálculo oficial de landed cost guardado.
        Closed = 3,      // Cerrada: La orden fue finalizada y ya no puede modificarse.
        Cancelled = 4    // Cancelada: La orden fue anulada y no debe ser modificada ni calculada.
    }
}