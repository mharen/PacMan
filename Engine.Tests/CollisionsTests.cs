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
        private static readonly Player DefaultPlayer = new PacmanPlayer(Guid.NewGuid(), "Michael", DefaultPosition);
        private static readonly Nom DefaultNom = new CherryNom(DefaultPosition);

        private static readonly Frame DefaultFrame =
            new Frame(
                players: new List<Player> {DefaultPlayer},
                noms: new List<Nom> {DefaultNom});

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
            var actual = frame.Players.Single(p => p.Id == DefaultPlayer.Id).Points;

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}