using System.Collections.Generic;

namespace Engine
{
    public class Frame
    {
        public long Tick { get; private set; }
        public IReadOnlyList<Player> Players { get; private set; }
        public IReadOnlyList<Tile> Tiles { get; private set; }
        public IReadOnlyList<Nom> Noms { get; private set; }

        public Frame(
            long tick = 0, 
            IReadOnlyList<Player> players = null, 
            IReadOnlyList<Tile> tiles = null,
            IReadOnlyList<Nom> noms = null)
        {
            Tick = tick;
            Players = players ?? new List<Player>();
            Tiles = tiles ?? new List<Tile>();
            Noms = noms ?? new List<Nom>();
        }
    }
}
