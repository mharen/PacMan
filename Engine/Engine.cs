using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

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
            Position nextPosition = null,
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

            // resolve ghosts vs. pacmans
            var pacmans = currentPlayers.OfType<PacmanPlayer>().ToList();
            var ghosts = currentPlayers.OfType<GhostPlayer>().ToList();

            // players on the same tile
            var conflictedPlayers = 
                from p in pacmans
                join g in ghosts on p.Position equals g.Position
                select new {Pacman = p, Ghost = g};

            // survivors
            var conflictSurvivors =
                from cp in conflictedPlayers
                let pointsToVictor = cp.Pacman.Points + cp.Ghost.Points
                let victor = cp.Pacman.IsPoweredUp? (Player)cp.Pacman : (Player)cp.Ghost
                select NextPlayer(victor, nextPoints: pointsToVictor);

            // players not on the same tile as anyone else
            var nonconflictedPlayers = currentPlayers
                .Except(conflictedPlayers.Select(cp => cp.Ghost))
                .Except(conflictedPlayers.Select(cp => cp.Pacman));

            // battle results
            var allSurvivors = conflictSurvivors.Union(nonconflictedPlayers);
            
            // resolve pacmans vs. noms
            var fedSurvivors =
                from p in allSurvivors
                let nomPoints = p is GhostPlayer
                    ? 0
                    : currentFrame.Noms.Where(n => Equals(n.Position, p.Position)).Sum(n => n.Points)
                select NextPlayer(p, nextPoints: p.Points + nomPoints);

            // find the uneaten noms
            var noms = currentFrame.Noms
                .Where(n => !fedSurvivors.OfType<PacmanPlayer>().Any(p => Equals(p.Position, n.Position)));

            return NextFrame(
                currentFrame,
                nextPlayers: fedSurvivors.ToList(),
                nextNoms: noms.ToList());
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