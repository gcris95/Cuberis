using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotation : MonoBehaviour {

    public GameObject page;
    public GameObject target;
    private Vector3 basePos;
    private Quaternion baseRot;

    private void Start()
    {
        basePos = transform.position;
        baseRot = transform.rotation;

    }

    void Update()
    {
        if (Input.GetMouseButton(0) && page.activeInHierarchy)
        {
            RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * 2.5f);
        }
    }

    void RotateAround(Vector3 center, Vector3 axis, float angle)
    {
        Vector3 pos = this.transform.position;
        Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
        Vector3 dir = pos - center;                         // find current direction relative to center
        dir = rot * dir;                                    // rotate the direction
        this.transform.position = center + dir;             // define new position

        // rotate object to keep looking at the center:
        Quaternion myRot = transform.rotation;
        transform.rotation *= Quaternion.Inverse(myRot) * rot * myRot;
    }

    public void rePosition()
    {
        transform.position = basePos;
        transform.rotation = baseRot;
    }
}