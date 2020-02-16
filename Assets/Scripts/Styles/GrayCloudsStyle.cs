using System;
using Assets.Scripts.Data;
using CoherentNoise.Generation;
using CoherentNoise.Generation.Fractal;
using UnityEngine;

namespace Assets.Scripts.Styles
{
    [Serializable]
    public class GrayCloudsStyle : Style
    {
        private CoherentNoise.Generator colorGen;

        [SerializeField] private float min = 0.0f;
        [SerializeField] private float max = 1f;
        [SerializeField] private float frequency = 0.15f;
        [SerializeField] private float larcunarity = 1.5f;
        [SerializeField] private float persistence = 0.3f;
        [SerializeField] private int octaveCount = 3;

        public float Min
        {
            get => min;
            set
            {
                min = value;
                OnStyleChanged();
            }
        }

        public float Max
        {
            get => max;
            set
            {
                max = value;
                OnStyleChanged();
            }
        }

        public float Frequency
        {
            get => frequency;
            set
            {
                frequency = value;
                Initialize();
                OnStyleChanged();
            }
        }

        public float Larcunarity
        {
            get => larcunarity;
            set
            {
                larcunarity = value;
                Initialize();
                OnStyleChanged();
            }
        }

        public float Persistence
        {
            get => persistence;
            set
            {
                persistence = value;
                Initialize();
                OnStyleChanged();
            }
        }

        public int OctaveCount
        {
            get => octaveCount;
            set
            {
                octaveCount = value;
                Initialize();
                OnStyleChanged();
            }
        }

        public GrayCloudsStyle()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.colorGen = new PinkNoise(0)
            {
                Frequency = frequency,
                Lacunarity = larcunarity,
                Persistence = persistence,
                OctaveCount = octaveCount
            };
        }

        public override Color GetColor(Vector3 position)
        {
            var gray = min + (colorGen.GetValue(position) + 1f) * (max - min) * 0.5f;

            return new Color(gray, gray, gray);
        }
    }
}