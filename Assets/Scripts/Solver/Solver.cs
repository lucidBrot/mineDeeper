using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Solver
{
    class Solver
    {
        private readonly Board board;
        private bool? solvable;

        public Solver(Board board)
        {
            this.board = board;
        }

        public bool IsSolvable()
        {
            if (solvable == null)
            {
                Compute();
            }

            return solvable.Value;
        }

        private void Compute()
        {
            throw new NotImplementedException();
        }
    }
}
