// COGARCH COGARCH_Simulation AxisSettings.cs
// Copyright © Jasper Ermatinger

#region usings

using System;

#endregion

namespace Assets.Scripts.GameLogic
{
    #region Usings

    #endregion

    /// <summary>
    ///     The axis settings.
    /// </summary>
    [Serializable]
    public class AxisSettings
    {
        /// <summary>
        ///     The axis name.
        /// </summary>
        public string AxisName;

        /// <summary>
        ///     The invert.
        /// </summary>
        public bool Invert;
    }
}