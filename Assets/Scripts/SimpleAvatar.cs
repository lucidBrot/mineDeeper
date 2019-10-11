using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAvatar : MonoBehaviour
{
    public string Layername;

    public Vector3 Offset;

    // Update is called once per frame
    void Update()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 9001, LayerMask.GetMask(Layername)))
        {
            this.transform.position = hit.point + Offset;
        }
    }
}
