﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Position DefaultPosition = new Position(0, 0);
        private static readonly Player DefaultPlayer = new Player(Guid.NewGuid(), "Michael", DefaultPosition);
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
            var expected = new Position(0, 1);

            // act / assert
            MovePlayerTester(DefaultPlayer, direction, expected);
        }

        [Test]
        public void MovePlayerEast()
        {
            // arrange
            const Direction direction = Direction.East;
            var expected = new Position(1, 0);

            // act / assert
            MovePlayerTester(DefaultPlayer, direction, expected);
        }
        [Test]
        public void MovePlayerSouth()
        {
            // arrange
            const Direction direction = Direction.South;
            var expected = new Position(0, -1);

            // act / assert
            MovePlayerTester(DefaultPlayer, direction, expected);
        }
        [Test]
        public void MovePlayerWest()
        {
            // arrange
            const Direction direction = Direction.West;
            var expected = new Position(-1, 0);

            // act / assert
            MovePlayerTester(DefaultPlayer, direction, expected);
        }

        private static void MovePlayerTester(Player player, Direction direction, Position expected)
        {
            // act
            var nextFrame = Engine.MovePlayer(DefaultFrame, player.Id, direction);
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