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

        this.transform.LookAt(c.transform.position, Vector3.up);
        var e = this.transform.eulerAngles;
        e = new Vector3(0, e.y, 0);
        this.transform.eulerAngles = e;
    }
}
