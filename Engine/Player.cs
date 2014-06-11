using System;

namespace Engine
{
    public class Player
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Position Position { get; private set; }

        public Player(Guid id, string name, Position position)
        {
            Id = id;
            Name = name;
            Position = position;
        }
    }
}