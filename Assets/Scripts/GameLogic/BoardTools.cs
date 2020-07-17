using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public static class BoardTools
    {
        public static void Reveal(this Board board, BoardCell cell)
        {
            cell.State = CellState.Revealed;

            if (cell.IsNude && !cell.IsBomb)
            {
                foreach (var neighbor in cell.Neighbors)
                {
                    if (neighbor.State != CellState.Default)
                    {
                        continue;
                    }

                    board.Reveal(neighbor);
                }
            }
        }

        public static Revelation RevealSlow(this Board board, BoardCell cell)
        {
            return new Revelation(board, cell, 0.03f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEachNeighbor(this Board board, BoardCell cell, Action<BoardCell> action)
        {
            foreach (var neighbor in cell.Neighbors)
            {
                action(neighbor);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<BoardCell> NeighborsOf(this Board board, BoardCell cell)
        {
            return cell.Neighbors;
        }

        public static int CountNeighbors(this Board board, BoardCell cell, Func<BoardCell, bool> predicate)
        {
            var count = 0;
            foreach (var neighbor in cell.Neighbors)
            {
                if (predicate(neighbor))
                {
                    count++;
                }
            }

            return count;
        }
    }
}