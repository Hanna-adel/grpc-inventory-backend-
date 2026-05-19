
using Grpc.Core;
using Grpc.Net.Client;
using Inventory.Server.Protos;
using Order.API.Protos;
using Payment.Server.Protos;
using static Order.API.Protos.OrderService;

namespace Order.API.Services
{
    public class OrderService : OrderServiceBase
    {
        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var inventoryChannel = GrpcChannel.ForAddress("http://localhost:5266");
            var inventoryClient = new InventoryService.InventoryServiceClient(inventoryChannel);

            foreach (var item in request.OrderItems)
            {
                var inventoryResult = await inventoryClient.ProductQuantityAsync(new QuantityRequest
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });

                if (!inventoryResult.Success)
                    return new PlaceOrderResponse { Success = false, Message = "Insufficient quantity" };
            }
            var paymentChannel = GrpcChannel.ForAddress("http://localhost:5088");
            var paymentClient = new PaymentService.PaymentServiceClient(paymentChannel);

            var totalAmount = request.OrderItems.Sum(i => i.Quantity * 10.0);

            var paymentResult = await paymentClient.ProcessPaymentAsync(new PaymentRequest
            {
                UserId = request.UserId.ToString(),
                Amount = totalAmount
            });

            if (!paymentResult.Success)
                return new PlaceOrderResponse { Success = false, Message = "Payment error" };

            return new PlaceOrderResponse { Success = true, Message = "Order placed!" };
        }
    }
}
