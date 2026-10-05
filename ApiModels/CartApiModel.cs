namespace UserProductCart.WebApi.ApiModels;

public class CartItemApiModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}