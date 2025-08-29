using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChandeliersHolderReleaseTrigger : InteractiveObject
{
    public SpringJoint joint;
    public Chandelier chandelier;


    public override void Interaction()
    {
        if (joint != null) 
        {
            joint.connectedBody = null;
            Destroy(joint);
        }

        chandelier.StartFalling();

        Destroy(gameObject);
    }
}
