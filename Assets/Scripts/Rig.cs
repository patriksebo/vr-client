using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRPoint
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

    public void SetVrTarget(Transform transform)
    {
        vrTarget = transform;
    }
}

public class Rig : MonoBehaviour
{

    public VRPoint head;
    public VRPoint leftHand;
    public VRPoint rightHand;
    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public bool isRemoteRig = false;
    public bool isExternalRig = false;
    public bool useRig2 = false;
    public bool useRig3 = false;

    private Quaternion initialRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = headConstraint.position + headBodyOffset;

        if (useRig2)
        {
            transform.forward = Vector3.ProjectOnPlane(-1 * headConstraint.up, Vector3.up).normalized;
        }
        else if (useRig3)
        {
            transform.rotation = new Quaternion(transform.rotation.x, headConstraint.rotation.y, transform.rotation.z, headConstraint.rotation.w);
        }
        else
        {
            transform.forward = Vector3.ProjectOnPlane(1 * headConstraint.up, Vector3.up).normalized;
        }

        if (isRemoteRig == false)
        {
            head.Map();
            leftHand.Map();
            rightHand.Map();
        }
    }

    public void External()
    {
        head = new VRPoint();
        leftHand = new VRPoint();
        rightHand = new VRPoint();
    }
}
