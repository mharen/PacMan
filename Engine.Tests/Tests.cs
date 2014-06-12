using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Position DefaultPosition = new Position(0, 0);
        private static readonly Player DefaultPlayer = new PacmanPlayer("1", "Michael", DefaultPosition);
        private static readonly Frame DefaultFrame = new Frame(players: new List<Player> { DefaultPlayer });

        [Test]
        public void AddingPlayerAddsPlayer()
        {
            // arrange
            var frame = new Frame();
            var player = DefaultPlayer;
            var expected = DefaultPlayer;

            // act
            var nextFrame = Engine.AddPlayer(frame, player);
            var actual = nextFrame.Players.Single();

            // assert
            Assert.AreEqual(expected.Name, actual.Name);
        }

        [Test]
        public void AddingPlayerToUnpassableSpaceThrows()
        {
            // arrange
            var wallTile = new WallTile(new Position(0, 0));
            var frame = new Frame(tiles: new List<Tile> {wallTile});
            var player = DefaultPlayer;
            // ReSharper disable once NotResolvedInText
            var expected = new ArgumentOutOfRangeException("player");

            // act
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Engine.AddPlayer(frame, player));

            // assert
            Assert.AreEqual(expected.ParamName, actual.ParamName);
        }

        [Test]
        public void AddingPlayerDoesNotIncrementTick()
        {
            // arrange
            var frame = new Frame();
            var player = DefaultPlayer;
            const int expected = 0;

            // act
            var nextFrame = Engine.AddPlayer(frame, player);
            var actual = nextFrame.Tick;

            // assert
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void MovePlayerNorth()
        {
            // arrange
            const Direction direction = Direction.North;
            var expected = new Position(0, -1);

            // act / assert
            MovePlayerTester(DefaultFrame, DefaultPlayer, direction, expected);
        }

        [Test]
        public void MovePlayerEast()
        {
            // arrange
            const Direction direction = Direction.East;
            var expected = new Position(1, 0);

            // act / assert
            MovePlayerTester(DefaultFrame, DefaultPlayer, direction, expected);
        }
        [Test]
        public void MovePlayerSouth()
        {
            // arrange
            const Direction direction = Direction.South;
            var expected = new Position(0, 1);

            // act / assert
            MovePlayerTester(DefaultFrame, DefaultPlayer, direction, expected);
        }
        [Test]
        public void MovePlayerWest()
        {
            // arrange
            const Direction direction = Direction.West;
            var expected = new Position(-1, 0);

            // act / assert
            MovePlayerTester(DefaultFrame, DefaultPlayer, direction, expected);
        }

        [Test]
        public void MovePlayerIntoWallYieldsNoMovement()
        {
            // arrange
            var wallTile = new WallTile(new Position(0, 1));
            var frame = new Frame(
                players: new List<Player> { DefaultPlayer }, 
                tiles: new List<Tile> { wallTile });
            const Direction direction = Direction.South;
            var expected = new Position(0, 0);

            // act / assert
            MovePlayerTester(frame, DefaultPlayer, direction, expected); 
        }

        private static void MovePlayerTester(Frame frame, Player player, Direction direction, Position expected)
        {
            // act
            var nextFrame = Engine.MovePlayer(frame, player.Id, direction);
            var actual = nextFrame.Players.Single(p => p.Id == player.Id).Position;

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AdvancingTickActuallyAdvancesTheTick()
        {
            // arrange
            var frame = new Frame();
            const int expected = 1;

            // act
            var next = Engine.AdvanceTick(frame);
            var actual = next.Tick;

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
