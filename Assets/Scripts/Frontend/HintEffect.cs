using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using Assets.Scripts.GameLogic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Frontend
{
    public class HintEffect : MonoBehaviour
    {
        public ParticleSystem Effect;

        private void OnEnable()
        {
            if (Game.CanAccessInstance)
            {
                Game.Instance.HintChanged += HintChanged;
                SetEffectActive(Game.Instance.ActiveHint != null);
            }
            else
            {
                SetEffectActive(false);
            }
        }

        private void OnDisable()
        {
            if (Game.CanAccessInstance)
            {
                Game.Instance.HintChanged -= HintChanged;
            }
        }

        private void HintChanged(object sender, ItemChangedEventArgs<Hint> e)
        {
            if (Effect != null)
            {
                var hint = Game.Instance.ActiveHint;

                if (hint != null)
                {
                    var pos = GameManager.BoardToWorldPosition(Game.Instance.ActiveHint.ConcernedCell);
                    Effect.transform.position = pos;

                    SetEffectActive(true);
                }
                else
                {
                    SetEffectActive(false);
                }
            }
        }

        private void SetEffectActive(bool active)
        {
            if (Effect == null)
            {
                return;
            }

            if (active)
            {
                Effect.Play();
            }
            else
            {
                Effect.Stop();
            }
        }
    }
}
