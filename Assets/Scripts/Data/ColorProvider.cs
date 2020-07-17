using System;
using Assets.Scripts.Styles;
using Unity_Tools.Components;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class ColorProvider : SingletonBehaviour<ColorProvider>
    {
        public Color[] NumberColors;

        [SerializeField]
        private GrayCloudsStyle style;

        public GrayCloudsStyle Style
        {
            get => style;
            set
            {
                if (style == value)
                {
                    return;
                }

                if (style != null)
                {
                    style.StyleChanged -= OnStyleChanged;
                }

                style = value;
                OnStyleChanged();

                if (style != null)
                {
                    style.StyleChanged += OnStyleChanged;
                }
            }
        }

        public ColorProvider()
        {
            Style = new GrayCloudsStyle();
        }

        public event Action StyleChanged;

        public static Color GetCubeColor(Vector3 position)
        {
            if (CanAccessInstance && Instance.Style != null)
            {
                return Instance.Style.GetColor(position);
            }

            return Color.white;
        }

        protected virtual void OnStyleChanged()
        {
            StyleChanged?.Invoke();
        }

        private void OnValidate()
        {
            style.Frequency = style.Frequency;
        }
    }
}
