using System;
using Assets.Scripts.Styles;
using Unity_Tools.Components;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class StyleManager : SingletonBehaviour<StyleManager>
    {
        public Color[] NumberColors;

        public Color[] MarkColors;

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

        public StyleManager()
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

        public static Color GetFocusColor(int key)
        {
            if (!CanAccessInstance || Instance.MarkColors == null || Instance.MarkColors.Length == 0)
            {
                return Color.black;
            }

            return Instance.MarkColors[key % Instance.MarkColors.Length];
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
