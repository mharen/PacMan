using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
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

        public static Frame AddPlayer(Frame currentFrame, Player player)
        {
            if (currentFrame.Tiles
                .Where(t => !t.IsPassable)
                .Any(t => Equals(t.Position, player.Position)))
            {
                throw new ArgumentOutOfRangeException("player", "Player cannot be added to unpassable tile");
            }

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
    }
}