
namespace SolidRabbit.Core.Events
{
    public class PlayerMovedEvent
    {
        public Guid PlayerId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
