using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidRabbit.Core.Events
{
    public class EnemyMovedEvent
    {
        public Guid EnemyId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
