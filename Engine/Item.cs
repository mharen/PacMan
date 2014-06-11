using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface ITile
    {
        Position Position { get; }
        bool IsPassable { get; }
        bool IsConsumable { get; }
    }

    public class WallTile : ITile
    {
        public Position Position { get; private set; }
        public bool IsPassable { get { return false; }}
        public bool IsConsumable { get { return false; } }
        
        public WallTile(Position position)
        {
            Position = position;
        }
    }

    public class NomTile : ITile
    {
        public Position Position { get; private set; }
        public bool IsPassable { get { return true; } }
        public bool IsConsumable { get { return true; } }

        public NomTile(Position position)
        {
            Position = position;
        }
    }

    public class FruitTile : ITile
    {
        public Position Position { get; private set; }
        public bool IsPassable { get { return true; } }
        public bool IsConsumable { get { return true; } }

        public FruitTile(Position position)
        {
            Position = position;
        }
    }

    public class EmptyTile : ITile
    {
        public Position Position { get; private set; }
        public bool IsPassable { get { return true; } }
        public bool IsConsumable { get { return false; } }

        public EmptyTile(Position position)
        {
            Position = position;
        }
    }
}
