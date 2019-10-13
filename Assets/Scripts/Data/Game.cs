﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assets.Scripts.GameLogic;
using JetBrains.Annotations;

namespace Assets.Scripts.Data
{
    public class Game : SingletonBehaviour<Game>, INotifyPropertyChanged
    {
        private Board gameBoard;
        private PlayerStats playerStats;
        private Hint PreviousHint;

        public Board GameBoard
        {
            get => gameBoard;
            set
            {
                if (Equals(value, gameBoard)) return;
                gameBoard = value;
                OnPropertyChanged();
            }
        }

        public int NextBoardWidth { get; set; }

        public int NextBoardHeight { get; set; }

        public int NextBoardDepth { get; set; }

        public int NextBombCount { get; set; }

        public Game()
        {
            NextBoardWidth = 10;
            NextBoardHeight = 10;
            NextBoardDepth = 10;
            NextBombCount = 50;
        }

        public void StartNewGame()
        {
            var generator = new Generator.Generator();
            GameBoard = generator.Generate((uint) NextBoardWidth, (uint) NextBoardHeight, (uint) NextBoardDepth,
                (uint) NextBombCount);
            playerStats = new PlayerStats();
        }

        public void StartDebugFlagsGame()
        {
            Board board = new Board(5, 3, 1);
            board.SetBombState(2, 1, 0, true);
            board.SetBombState(4, 1, 0, true);

            GameBoard = board;
            foreach (var boardCell in board)
            {
                boardCell.State = CellState.Default;
            }
            board[1, 0, 0].State = CellState.Revealed;
            board[1, 1, 0].State = CellState.Suspect; // wrong flag
            board[2, 1, 0].State = CellState.Suspect; // good flag
            playerStats = new PlayerStats();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reveal(BoardCell cell)
        {
            GameBoard.Reveal(cell);
            if (cell.IsBomb)
            {
                playerStats.NumBombsExploded++;
            }
        }

        public void RequestHint()
        {
            Hint hint = Solver.Solver.Hint(GameBoard);
            if (hint.IsSameHintAs(PreviousHint))
            {   // same hint requested again. Show text
                throw new NotImplementedException("Cannot Display Text Hint on GUI yet");

            } else { 
                playerStats.NumHintsRequested++;
            }

            foreach (BoardCell cell in hint.CellsToHighlight)
            {
                GameBoard.Highlight(cell);
            }

            PreviousHint = hint;
        }
    }
}
