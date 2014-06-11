using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Frame
    {
        public readonly long Tick;
        public readonly IReadOnlyList<Player> Players;

        public Frame(long tick = 0, IReadOnlyList<Player> players = null)
        {
            Tick = tick;
            Players = players ?? new List<Player>();
        }
    }

    public class Position : IEquatable<Position>
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }
    }

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

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public class Engine
    {
        private static Frame NextFrame(Frame current, 
            long? nextTick = null, 
            IReadOnlyList<Player> nextPlayers = null)
        {
            return new Frame(
                tick: nextTick ?? current.Tick,
                players: nextPlayers ?? current.Players);
        }

        private static Player NextPlayer(Player current,
            string nextName = null, 
            Position nextPosition = null)
        {
            return new Player(
                current.Id,
                nextName ?? current.Name,
                nextPosition ?? current.Position);
        }

        public static Frame AddPlayer(Frame currentFrame, Player player)
        {
            var nextPlayers = currentFrame.Players.ToList();
            nextPlayers.Add(player);

            return NextFrame(
                currentFrame,
                nextPlayers: nextPlayers);
        }

        public static Frame AdvanceTick(Frame current)
        {
            return NextFrame(current, nextTick: current.Tick + 1);
        }

        public static Frame MovePlayer(Frame currentFrame, Guid playerId, Direction direction)
        {
            var currentPlayer = currentFrame.Players.ToList().Single(p => p.Id == playerId);
            
            var nextPosition = NextPosition(currentPlayer.Position, direction);
            var nextPlayer = NextPlayer(currentPlayer, nextPosition: nextPosition);
            var nextPlayers = currentFrame.Players.Where(p => p.Id != playerId).ToList();
            
            nextPlayers.Add(nextPlayer);

            return NextFrame(currentFrame, nextPlayers: nextPlayers);
        }

        private static Position NextPosition(Position current, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Position(current.X, current.Y + 1);
                case Direction.East:
                    return new Position(current.X + 1, current.Y);
                case Direction.South:
                    return new Position(current.X, current.Y - 1);
                case Direction.West:
                    return new Position(current.X - 1, current.Y);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }
    }
}
