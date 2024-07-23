using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTrnasform;


    public void SetTargetTransform(Transform targetTrnasform)
    {
        this.targetTrnasform = targetTrnasform;
    }

    private void LateUpdate()
    {
        if(targetTrnasform == null) { return; }

        transform.position = targetTrnasform.position;
        transform.rotation = targetTrnasform.rotation;
    }
}
