using MediatR;

namespace Phase_07_Poc_01.Infrastructure.Events
{
    // The Event (The Shout) - Carries the data about what happened
    public class OrderPlacedEvent : INotification
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
