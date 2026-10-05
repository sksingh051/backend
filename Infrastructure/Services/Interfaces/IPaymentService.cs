namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces
{
    using Phase_07_Poc_01.DTO;
    using Phase_07_Poc_01.DTO.PaymentDtos;

    public interface IPaymentService
    {
        Task<ApiResponseResult<bool>> SimulatePaymentAsync(SimulatePaymentDto dto, int userId);
    }
}
