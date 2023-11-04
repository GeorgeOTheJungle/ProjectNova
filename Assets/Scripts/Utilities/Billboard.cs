using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Vector3 originalRotation;

    [Header("Lock Rotation")]
    [Space(10)]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private void Start()
    {
        originalRotation = transform.rotation.eulerAngles;
    }
    private void LateUpdate()
    {
        //transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.forward = Camera.main.transform.forward;
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) rotation.x = originalRotation.x;
        if (lockY) rotation.y = originalRotation.y;
        if (lockZ) rotation.z = originalRotation.z;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
