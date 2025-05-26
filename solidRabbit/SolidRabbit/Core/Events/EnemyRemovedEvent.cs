
namespace SolidRabbit.Core.Events
{
    public class EnemyRemovedEvent
    {
        public Guid EnemyId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
