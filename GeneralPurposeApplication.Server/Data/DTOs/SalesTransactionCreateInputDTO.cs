namespace GeneralPurposeApplication.Server.Data.DTOs
{
    public class SalesTransactionCreateInputDTO
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string? CustomerId { get; set; } // optional if you support customers
        public List<SalesTransactionItemCreateInputDTO> Items { get; set; } = new();

    }
}
