using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using NUnit.Framework;

namespace Assets.Scripts.Tests
{
    class BoardTests
    {
        [Test]
        public void testBombCount()
        {
            Board board = new Board(5,5,4);
            for (int i = 0; i < 4; i++)
            {
                board.SetBombState(i, 0, 0, true);
            }

            board.SetBombState(0,0,0, false);
            board.SetBombState(2,2,2, false);

            Assert.AreEqual(3, board.BombCount);
        }
    }
}
