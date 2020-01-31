using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRIK : MonoBehaviour
{

    public Transform[] joints;
    private Vector3 initialJoint;
    private List<float> boneLenghts = new List<float>();
    private float totalLenght = 0;
    public float moveSpeed = 1;
    private float errorMargin = 0.1f;

    private int maxItterations = 100;
    private int currentItteration = 0;

    public Transform target;


    // Start is called before the first frame update
    void Start()
    {
        // Setting root joint.
        initialJoint = joints[0].position;

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
        if (target == null)// || currentItteration == maxItterations)
            return;

        //if target is out of reach, return
        if ((initialJoint - target.position).magnitude > totalLenght)
            return;

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
        joints[0].position = initialJoint;
        //Setting positions from joints
        for (int i = 0; i < joints.Length; i++)
        {
            //Skip the overwrite of the first joint
            if (i == 0)
                continue;

            //Move joint[i] to new position calculated from previous joint
            joints[i].position = ((joints[i].position - joints[i - 1].position).normalized * boneLenghts[i - 1] + joints[i - 1].position);

            if (currentItteration != maxItterations)
                currentItteration++;
        }

        //Rotate bones to look at the next joint
        for (int i = 0; i < joints.Length - 1; i++)
        {
            //Calculating rotation
            Vector3 relativePos = joints[i + 1].position - joints[i].position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            Transform tempJoint;

            //Saving next joints position/rotation
            tempJoint = joints[i + 1];
            //Rotating joint
            joints[i].rotation = rotation;
            joints[i].Rotate(joints[i].rotation.x, joints[i].rotation.y - 90, joints[i].rotation.z - 90);
            //Applying old values to next joint
            joints[i + 1] = tempJoint;
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
