using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{

    public Transform[] joints;
    private Transform initialJoint;
    private List<float> boneLenghts = new List<float>();
    private float totalLenght = 0;
    public float moveSpeed = 1;
    public float errorMargin = 0.1f;

    private int maxItterations = 100;
    private int currentItteration = 0;

    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        // Setting root joint.
        initialJoint = joints[0];

        // Calculating bone lenght
        for (int i = 0; i < joints.Length - 1; i++)
        {
            boneLenghts.Add((joints[i].position - joints[i + 1].position).magnitude);
        }

        // Calculating total lenght
        for (int i = 0; i < boneLenghts.Count; i++)
        {
            totalLenght += boneLenghts[i];
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
            return;

        //If target is out of reach, return
        //if ((initialJoint.position - target.position).magnitude > totalLenght)
        //    return;

        if ((joints[joints.Length - 1].position - target.position).magnitude > errorMargin)
        {
            BackwardsKinematics();
            ForwardKinematics();
        }
    }

    /// <summary>
    /// Applying inverse kinematics formula from the first to the last joint.
    /// </summary>
    void ForwardKinematics()
    {
        // Setting the first joint back to its original position.
        joints[0].position = initialJoint.position;
        for (int i = 0; i < joints.Length; i++)
        {
            //Skip the overwrite of the first joint
            if (i == 0)
                continue;
            //Move joint[i] to new position calculated from previous joint
            joints[i].position = ((joints[i].position - joints[i - 1].position).normalized * boneLenghts[i - 1] + joints[i - 1].position);
        }
    }

    /// <summary>
    /// Applying inverse kinematics formula from the last to the first joint.
    /// </summary>
    void BackwardsKinematics()
    {
        // Setting the last joint to the target position.
        joints[joints.Length - 1].position = target.position;
        for (int i = joints.Length - 1; i > 0; i--)
        {
            //Skip the overwrite of the last joint
            if (i == joints.Length - 1)
                continue;
            //Move joint[i] to new position calculated from previous joint
            joints[i].position = ((joints[i].position - joints[i + 1].position).normalized * boneLenghts[i] + joints[i + 1].position);
        }
    }
}
