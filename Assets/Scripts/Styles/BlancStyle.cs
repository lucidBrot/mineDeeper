using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Styles
{
    public class BlancStyle : Style
    {
        public override Color GetColor(Vector3 position)
        {
            return Color.white;
        }
    }
}
