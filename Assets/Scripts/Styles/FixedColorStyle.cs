using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Styles
{
    public class FixedColorStyle : Style
    {
        private Color color;

        public Color Color
        {
            get => color;
            set
            {
                if (color == value)
                {
                    return;
                }

                color = value;
                OnStyleChanged();
            }
        }

        public FixedColorStyle(Color color)
        {
            this.color = color;
        }

        public override Color GetColor(Vector3 position)
        {
            return color;
        }
    }
}
