using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.EffectTimeline;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity_Tools.Components;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class Game : SingletonBehaviour<Game>, INotifyPropertyChanged
    {
        private Hint _activeHint;

        private Board gameBoard;

        private PlayerStats playerStats;

        public Hint ActiveHint
        {
            get => _activeHint;
            private set
            {
                if (_activeHint == value)
                {
                    return;
                }

                var old = _activeHint;
                _activeHint = value;
                OnHintChanged(_activeHint, old);
            }
        }

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

        public PlayerStats PlayerStats
        {
            get => playerStats;
            set
            {
                if (Equals(value, playerStats)) return;
                playerStats = value;
                OnPropertyChanged();
            }
        }

        public int NextBoardWidth { get; set; }

        public int NextBoardHeight { get; set; }

        public int NextBoardDepth { get; set; }

        public int NextBombCount { get; set; }

        public event EventHandler<ItemChangedEventArgs<Hint>> HintChanged;

        public event EventHandler<EventArgs> NewGameStarting; 

        public event EventHandler<EventArgs> NewGameStarted;

        public Game()
        {
            NextBoardWidth = 10;
            NextBoardHeight = 10;
            NextBoardDepth = 10;
            NextBombCount = 50;
        }

        private Generator.Generator _lastStartedGenerator;

        public void StartNewGame()
        {
            this.OnNewGameStarting();

            this.GameBoard = new Board(0, 0, 0);
            this.PlayerStats = new PlayerStats();

            // Abort generator that is currently running
            _lastStartedGenerator?.Abort();
            // Create new generator to avoid issues with reuse after aborting - not certain if necessary.
            _lastStartedGenerator = new Generator.Generator();
            // Generate in background Thread
            MainThreadDispatch.Initialize();
            Task task = Task.Run(() =>
            {
                Board board = _lastStartedGenerator.Generate((uint) NextBoardWidth, (uint) NextBoardHeight,
                    (uint) NextBoardDepth,
                    (uint) NextBombCount);

                if (board != null)
                {
                    MainThreadDispatch.InvokeAsync(() => { GameBoard = board; this.OnNewGameStarted();});
                }
            });
        }

        public void Reveal(BoardCell cell)
        {
            GameBoard.Reveal(cell);
            if (cell.IsBomb)
            {
                PlayerStats.NumBombsExploded++;
            }

            TestWhetherHintStillValid();
        }

        public Revelation RevealSlow(BoardCell cell)
        {
            var revelation = GameBoard.RevealSlow(cell);

            revelation.FollowWith(() =>
            {
                if (cell.IsBomb)
                {
                    PlayerStats.NumBombsExploded++;
                }

                TestWhetherHintStillValid();
            });

            return revelation;
        }

        private void TestWhetherHintStillValid()
        {
            if (ActiveHint != null)
            {
                Hint nextHint = Solver.Solver.Hint(GameBoard);
                if (!nextHint.IsEquivalentTo(ActiveHint))
                {
                    UILayer.Instance.HintText = null;
                    ActiveHint?.CellsToHighlight.ForEach(c => c.Highlighted = false);
                    ActiveHint = null;
                }
            }
        }

        public void RequestHint()
        {
            var hint = Solver.Solver.Hint(GameBoard);
            if (ActiveHint != null && hint.IsEquivalentTo(ActiveHint))
            {   // same hint requested again. Show text
                UILayer.Instance.HintText = hint.Text;

            } 
            else 
            { 
                PlayerStats.NumHintsRequested++;
                RedrawHintHighlights(hint);
                ActiveHint = hint;
            }
        }

        public void RedrawHintHighlights([CanBeNull] Hint hint = null)
        {
            if (hint == null)
            {
                hint = ActiveHint;
            }

            if (hint == null)
            {
                // nothing to highlight
                return;
            }

            foreach (BoardCell cell in hint.CellsToHighlight)
            {
                GameBoard.Highlight(cell);
            }
        }

        public void ToggleMarking(BoardCell cell)
        {
            var oldState = cell.State;
            cell.ToggleMarking();

            if (oldState != CellState.Suspect && cell.State == CellState.Suspect)
            {
                GameBoard.FlagCount++;
            }
            else if (oldState == CellState.Suspect && cell.State != CellState.Suspect)
            {
                GameBoard.FlagCount--;
            }

            if (oldState != CellState.Unknown && cell.State == CellState.Unknown)
            {
                GameBoard.UnknownCount++;
            }
            else if (oldState == CellState.Unknown && cell.State != CellState.Unknown)
            {
                GameBoard.UnknownCount--;
            }

            CheckIfGameFinished();
            TestWhetherHintStillValid();
        }

        public void CheckIfGameFinished()
        {
            var flags = GameBoard.Where(c => c.State == CellState.Suspect);
            if (flags.Count() == GameBoard.BombCount - GameBoard.Count(b => b.IsBomb && b.State == CellState.Revealed)
                && GameBoard.All(c => c.State==CellState.Revealed || (c.State==CellState.Suspect && c.IsBomb)))
            {
                // Game Won!
                FinishGame();
            }
        }

        private void FinishGame()
        {
            UILayer.Instance.HintText = "Game Finished!";
            foreach (var boardCell in GameBoard)
            {
                boardCell.Highlighted = true;
            }
        }

        protected virtual void OnHintChanged(Hint newHint, Hint oldHint)
        {
            HintChanged?.Invoke(this, new ItemChangedEventArgs<Hint>(newHint, oldHint));
        }

        protected virtual void OnNewGameStarted()
        {
            this.NewGameStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnNewGameStarting()
        {
            this.NewGameStarting?.Invoke(this, EventArgs.Empty);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
