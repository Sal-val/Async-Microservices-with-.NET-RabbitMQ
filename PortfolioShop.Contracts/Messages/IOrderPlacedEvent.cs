using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioShop.Contracts.Messages
{
    public record OrderPlacedEvent
    {
        public Guid OrderId { get; init; }
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
