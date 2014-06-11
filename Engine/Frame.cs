using System.Collections.Generic;

namespace Engine
{
    public class Frame
    {
        public long Tick { get; private set; }
        public IReadOnlyList<ITile> Tiles { get; private set; }
        public IReadOnlyList<Player> Players { get; private set; }

        public Frame(
            long tick = 0, 
            IReadOnlyList<Player> players = null, 
            IReadOnlyList<ITile> tiles = null)
        {
            Tick = tick;
            Tiles = tiles ?? new List<ITile>();
            Players = players ?? new List<Player>();
        }
    }
}
