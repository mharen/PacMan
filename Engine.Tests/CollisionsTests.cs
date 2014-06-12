using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class CollisionsTests
    {
        private static readonly Position DefaultPosition = new Position(0, 0);
        private static readonly Player DefaultPacman = new PacmanPlayer(Guid.NewGuid(), "Michael", DefaultPosition);
        private static readonly Player DefaultPoweredUpPacman = new PacmanPlayer(Guid.NewGuid(), "MICHAEL", DefaultPosition, powerUpTicksRemaining: 1);
        private static readonly Player DefaultGhost = new GhostPlayer(Guid.NewGuid(), "leahciM", DefaultPosition);
        private static readonly Nom DefaultNom = new CherryNom(DefaultPosition);

        private static readonly Frame DefaultFrame =
            new Frame(
                players: new List<Player> { DefaultPacman },
                noms: new List<Nom> { DefaultNom });

        [Test]
        public void FruitDisappearsWhenEaten()
        {
            // arrange
            const int expected = 0;

            // act
            var frame = Engine.ResolveCollisions(DefaultFrame);
            var actual = frame.Noms.Count;

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PacmanEarnsPointsWhenEatingFruit()
        {
            // arrange
            const int expected = 100;

            // act
            var frame = Engine.ResolveCollisions(DefaultFrame);
            var actual = frame.Players.Single(p => p.Id == DefaultPacman.Id).Points;

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PacmanDisappearsWhenCollidesWithGhost()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultPacman, DefaultGhost });
            const int expected = 0;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count(p => p.Id == DefaultPacman.Id);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GhostDisappearsWhenCollidesWithPoweredUpPacman()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultPoweredUpPacman, DefaultGhost });
            const int expected = 0;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count(p => p.Id == DefaultGhost.Id);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GhostRemainsWhenCollidesWithPacman()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultPacman, DefaultGhost });
            const int expected = 1;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count(p => p.Id == DefaultGhost.Id);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PoweredUpPacmanRemainsWhenCollidesWithGhost()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultPoweredUpPacman, DefaultGhost });
            const int expected = 1;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count(p => p.Id == DefaultPoweredUpPacman.Id);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PacmenRemainWhenTheyCollideWithEachOther()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultPoweredUpPacman, DefaultPacman });
            const int expected = 2;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GhostsRemainWhenTheyCollideWithEachOther()
        {
            // arrange
            var frame = new Frame(players: new List<Player> { DefaultGhost, new GhostPlayer(Guid.NewGuid(), "Leahcim2", DefaultPosition) });
            const int expected = 2;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TwoGhostsRemainWhenCollideWithPacman()
        {
            // arrange
            var frame = new Frame(players: new List<Player>
                                           {
                                               DefaultGhost,
                                               new GhostPlayer(Guid.NewGuid(), "Leahcim2", DefaultPosition),
                                               DefaultPacman
                                           });
            const int expected = 2;

            // act
            var result = Engine.ResolveCollisions(frame);
            var actual = result.Players.Count();

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}