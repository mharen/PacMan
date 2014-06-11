using System.Collections.Generic;

namespace Engine
{
    public class Frame
    {
        public long Tick { get; private set; }
        public IReadOnlyList<Player> Players { get; private set; }

        public Frame(long tick = 0, IReadOnlyList<Player> players = null)
        {
            Tick = tick;
            Players = players ?? new List<Player>();
        }
    }
}
