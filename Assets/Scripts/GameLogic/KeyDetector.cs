using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class KeyDetector : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        foreach (var keycode in Enum.GetValues(typeof(KeyCode)).OfType<KeyCode>())
        {
            if (Input.GetKeyDown(keycode))
            {
                this.GetComponent<Text>().text = keycode.ToString();
            }
        }
    }
}
