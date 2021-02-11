using System;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.DevTools
{
    public class SaveAndLoadKeyListener : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // Save to file
                Game.Instance.saveStateToFile("test_save.sav");
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                // Load from file
                Game.Instance.restoreStateFromFile("test_save.sav");
            }
        }
    }
}