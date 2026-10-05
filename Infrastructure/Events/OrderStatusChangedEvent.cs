using MediatR;

namespace Phase_07_Poc_01.Infrastructure.Events
{
    // Event published when an admin changes an order's status
    public class OrderStatusChangedEvent : INotification
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
