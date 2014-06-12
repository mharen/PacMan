namespace Engine
{
    public abstract class Tile
    {
        public Position Position { get; private set; }

        protected Tile(Position position)
        {
            Position = position;
        }
    }

    public class WallTile : Tile
    {
        public WallTile(Position position) : base(position)
        {
        }
    }

    public class EmptyTile : Tile
    {
        public EmptyTile(Position position) : base(position)
        {
        }
    }
}
