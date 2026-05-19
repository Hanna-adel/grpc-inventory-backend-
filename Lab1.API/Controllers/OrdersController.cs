using Grpc.Net.Client;
using Inventory.Server.Protos;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Payment.Server.Protos;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderRequest request)
        {
            var inventoryChannel = GrpcChannel.ForAddress("http://localhost:5266");
            var inventoryClient = new InventoryService.InventoryServiceClient(inventoryChannel);

            foreach (var item in request.orderItems)
            {
                var inventoryResult = await inventoryClient.ProductQuantityAsync(new QuantityRequest
                {
                    ProductId = item.productId,
                    Quantity = item.Quantity
                });

                if (!inventoryResult.Success)
                    return BadRequest($"Inventory error for product {item.productId}: {inventoryResult.Message}");
            }
            var paymentChannel = GrpcChannel.ForAddress("http://localhost:5088");
            var paymentClient = new PaymentService.PaymentServiceClient(paymentChannel);

            var totalAmount = request.orderItems.Sum(i => i.Quantity * 10.0); 

            var paymentResult = await paymentClient.ProcessPaymentAsync(new PaymentRequest
            {
                UserId = request.userId.ToString(),
                Amount = totalAmount
            });

            if (!paymentResult.Success)
                return BadRequest($"Payment error: {paymentResult.Message}");

            return Ok("Order placed successfully!");
        }
    }
}
