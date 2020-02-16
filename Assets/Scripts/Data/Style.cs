using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public abstract class Style
    {
        public abstract Color GetColor(Vector3 position);

        public event Action StyleChanged;

        protected void OnStyleChanged()
        {
            StyleChanged?.Invoke();
        }
    }
}
