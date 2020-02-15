using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Assets.Scripts.Data
{
    public class PlayerStats : INotifyPropertyChanged
    {
        private int numHintsRequested;
        private int numBombsExploded;

        public int NumBombsExploded
        {
            get => numBombsExploded;
            set
            {
                if (value == numBombsExploded) return;
                numBombsExploded = value;
                OnPropertyChanged();
            }
        }

        public int NumHintsRequested
        {
            get => numHintsRequested;
            set
            {
                if (value == numHintsRequested) return;
                numHintsRequested = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}