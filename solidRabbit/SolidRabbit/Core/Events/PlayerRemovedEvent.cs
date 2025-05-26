namespace SolidRabbit.Core.Events
{
    public class PlayerRemovedEvent
    {
        public Guid PlayerId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}