using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Engine.Tests
{
    [TestFixture]
    public class AdvanceTickTests
    {
        private static readonly Position DefaultPosition = new Position(0, 0);
        
        [Test]
        public void VeryPoweredUpPacmanStaysPoweredUpAfterATick()
        {
            // arrange
            var player = new PacmanPlayer(Guid.NewGuid(), "Michael", DefaultPosition, powerUpTicksRemaining: 2);
            var frame = new Frame(players: new List<Player> { player });

            const bool expected = true;

            // act
            var result = Engine.AdvanceTick(frame);
            var actual = result.Players.OfType<PacmanPlayer>().Single(p => p.Id == player.Id).IsPoweredUp;

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BarelyPoweredUpPacmanReturnsToNormalAfterATick()
        {
            // arrange
            var player = new PacmanPlayer(Guid.NewGuid(), "Michael", DefaultPosition, powerUpTicksRemaining: 1);
            var frame = new Frame(players: new List<Player> { player });

            const bool expected = false;

            // act
            var result = Engine.AdvanceTick(frame);
            var actual = result.Players.OfType<PacmanPlayer>().Single(p => p.Id == player.Id).IsPoweredUp;

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
