using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var c = Camera.main;

        if (c == null)
        {
            return;
        }

        var dir = this.transform.position - c.transform.position;
        dir.y = 0;
        var rot = Quaternion.LookRotation(dir, Vector3.up);
        this.transform.rotation = rot;
    }
}
