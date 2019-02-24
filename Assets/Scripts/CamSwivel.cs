using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwivel : MonoBehaviour
{
    public List<float> ViewingAngles;
    
    int currentAngle;

    void Update ()
    {
        if (Input.GetButtonDown("Rotate Right"))
        {
            currentAngle--;
        }
        else if (Input.GetButtonDown("Rotate Left"))
        {
            currentAngle++;
        }
        currentAngle = (int) Mathf.Repeat(currentAngle, ViewingAngles.Count);

        Quaternion desiredRot = Quaternion.Euler(0, ViewingAngles[currentAngle], 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, 3 * Time.deltaTime);
    }
}
