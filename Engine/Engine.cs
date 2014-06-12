using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine
    {
        private static Frame NextFrame(Frame current, 
            long? nextTick = null, 
            IReadOnlyList<Player> nextPlayers = null,
            IReadOnlyList<Nom> nextNoms = null)
        {
            return new Frame(
                tick: nextTick ?? current.Tick,
                players: nextPlayers ?? current.Players,
                noms: nextNoms ?? current.Noms);
        }

        private static Player NextPlayer(
            Player current, 
            Position nextPosition,
            string nextName = null,
            int? nextPoints = null)
        {
            var pacman = current as PacmanPlayer;
            if (pacman != null)
            {
                return NextPacmanPlayer(pacman, nextName, nextPosition, nextPoints);
            }

            var ghost = (GhostPlayer) current;
            return NextGhostPlayer(ghost, nextName, nextPosition, nextPoints);
        }

        private static PacmanPlayer NextPacmanPlayer(
            PacmanPlayer current,
            string nextName = null,
            Position nextPosition = null,
            int? nextPoints = null,
            int? nextPowerUpTicksRemaining = null)
        {
            return new PacmanPlayer(
                current.Id,
                nextName ?? current.Name,
                nextPosition ?? current.Position,
                nextPoints ?? current.Points,
                nextPowerUpTicksRemaining ?? current.PowerUpTicksRemaining);

        }

        private static Player NextGhostPlayer(
            Player current,
            string nextName = null,
            Position nextPosition = null,
            int? nextPoints = null)
        {
            return new GhostPlayer(
                    current.Id,
                    nextName ?? current.Name,
                    nextPosition ?? current.Position,
                    nextPoints ?? current.Points);
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

        public static Frame ResolveCollisions(Frame currentFrame)
        {
            var currentPlayers = currentFrame.Players.ToList();

            var pacmans = currentPlayers.OfType<PacmanPlayer>().ToList();
            var ghosts = currentPlayers.OfType<GhostPlayer>().ToList();

            var destroyedPacmans = pacmans.Join(ghosts, p => p.Position, g => g.Position, (p, g) => p);
            var survivingPacmans = pacmans.Except(destroyedPacmans).ToList();

            // always return the ghosts
            var players = new List<Player>(ghosts);

            // and the surviving pacmans after feeding them
            players.AddRange(
                from pacman in survivingPacmans
                let points = currentFrame.Noms.Where(n => Equals(n.Position, pacman.Position)).Sum(n => n.Points)
                select NextPacmanPlayer(pacman, nextPoints: pacman.Points + points));

            // return the uneaten noms
            var destroyedNoms = currentFrame.Noms.Where(n => survivingPacmans.Any(p => Equals(p.Position, n.Position)));
            var noms = currentFrame.Noms.Except(destroyedNoms).ToList();

            return NextFrame(
                currentFrame,
                nextPlayers: players,
                nextNoms: noms);
        }

        public static Frame AddPlayer(Frame currentFrame, Player player)
        {
            if (currentFrame.Tiles
                .OfType<WallTile>()
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
            var players = current.Players
                .Select(p =>
                        {
                            // process known temporal things
                            var pacman = p as PacmanPlayer;
                            if (pacman != null && pacman.IsPoweredUp)
                            {
                                return NextPacmanPlayer(pacman,
                                    nextPowerUpTicksRemaining: pacman.PowerUpTicksRemaining - 1);
                            }

                            // don't do anything for others
                            return p;
                        })
                .ToList();
               
            return NextFrame(
                current, 
                nextTick: current.Tick + 1,
                nextPlayers: players);
        }

        public static Frame MovePlayer(Frame currentFrame, Guid playerId, Direction direction)
        {
            var currentPlayer = currentFrame.Players.ToList().Single(p => p.Id == playerId);

            var nextPosition = NextPosition(currentPlayer.Position, direction);

            if (currentFrame.Tiles
                .OfType<WallTile>()
                .Any(t => Equals(t.Position, nextPosition)))
            {
                // player can't move through unpassable tiles so they stay where they were
                return currentFrame;
            }

            var nextPlayer = NextPlayer(currentPlayer, nextPosition: nextPosition);
            var nextPlayers = currentFrame.Players.Where(p => p.Id != playerId).ToList();
            
            nextPlayers.Add(nextPlayer);

            return NextFrame(currentFrame, nextPlayers: nextPlayers);
        }
    }
}