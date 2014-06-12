using System;

namespace Engine
{
    public abstract class Player
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public Position Position { get; private set; }
        public int Points { get; private set; }

        protected Player(string id, string name, Position position, int points = 0)
        {
            Id = id;
            Name = name;
            Position = position;
            Points = points;
        }
    }

    public class PacmanPlayer : Player
    {
        public int PowerUpTicksRemaining { get; private set; }
        public bool IsPoweredUp { get { return PowerUpTicksRemaining > 0; }}

        public PacmanPlayer(string id, string name, Position position, int points = 0, int powerUpTicksRemaining = 0)
            : base(id, name, position, points)
        {
            PowerUpTicksRemaining = powerUpTicksRemaining;
        }
    }

    public class GhostPlayer : Player
    {
        public GhostPlayer(string id, string name, Position position, int points = 0) : base(id, name, position, points)
        {
        }
    }
}