namespace Engine
{
    public abstract class Nom
    {
        public Position Position { get; private set; }
        public int Points { get; private set; }

        protected Nom(Position position, int points)
        {
            Position = position;
            Points = points;
        }
    }

    public class CherryNom : Nom
    {
        public CherryNom(Position position)
            : base(position, 100)
        {
        }
    }

    public class StrawberryNom : Nom
    {
        public StrawberryNom(Position position)
            : base(position, 300)
        {
        }
    }

    public class OrangeNom : Nom
    {
        public OrangeNom(Position position)
            : base(position, 500)
        {
        }
    }

    public class AppleNom : Nom
    {
        public AppleNom(Position position)
            : base(position, 700)
        {
        }
    }
}