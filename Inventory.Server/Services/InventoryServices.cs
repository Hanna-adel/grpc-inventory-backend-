using Grpc.Core;
using Inventory.Server.Protos;
using static Inventory.Server.Protos.InventoryService;

namespace Inventory.Server.Services
{
    public class InventoryServices: InventoryServiceBase
    {
        private static int _stock = 200;
        public override async Task<QuantityResponse> ProductQuantity(QuantityRequest request, ServerCallContext context)
        {
            if(request.Quantity > _stock)
            {
                return await Task.FromResult(new QuantityResponse
                {
                    Success = false,
                    Message = $"Not enough stock. Available quantity: {_stock}"
                });
            }

            _stock -= request.Quantity;
            return await Task.FromResult(new QuantityResponse
            {
                Success = true,
                Message = $"Stock updated successfully. Remaining quantity: {_stock}"
            });
        }
    }
}
