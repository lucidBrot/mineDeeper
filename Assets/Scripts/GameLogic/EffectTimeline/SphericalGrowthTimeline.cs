using UnityEngine;

namespace Assets.Scripts.GameLogic.EffectTimeline
{
    public readonly struct SphericalGrowthTimeline : IEffectTimeline
    {
        public readonly Vector3 Center;

        public readonly float RadiusPerSecond;

        public SphericalGrowthTimeline(Vector3 center, float radiusPerSecond)
        {
            this.Center = center;
            this.RadiusPerSecond = radiusPerSecond;
        }

        /// <inheritdoc />
        public float GetEffectTime(Vector3 position)
        {
            var dist = Vector3.Distance(position, this.Center);
            return dist / this.RadiusPerSecond;
        }
    }
}
