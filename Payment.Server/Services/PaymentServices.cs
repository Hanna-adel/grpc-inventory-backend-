using Grpc.Core;
using Payment.Server.Protos;
using static Payment.Server.Protos.PaymentService;

namespace Payment.Server.Services
{
    public class PaymentServices : PaymentServiceBase
    {
        private static double _balance = 1000.00;

        public override Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
        {
            if (request.Amount <= 0)
            {
                return Task.FromResult(new PaymentResponse
                {
                    Success = false,
                    Message = "Invalid payment amount."
                });
            }

            if (request.Amount > _balance)
            {
                return Task.FromResult(new PaymentResponse
                {
                    Success = false,
                    Message = $"Insufficient balance. Available balance: {_balance}"
                });
            }

            _balance -= request.Amount;

            return Task.FromResult(new PaymentResponse
            {
                Success = true,
                Message = $"Payment of {request.Amount} processed successfully. Remaining balance: {_balance}"
            });
        }
    }
}
