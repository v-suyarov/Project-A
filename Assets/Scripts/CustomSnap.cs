using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteAlways]
public class CustomSnap : MonoBehaviour
{

    public void Awake()
    {
        if(!transform.GetComponent<PointBuilder>())
        {
            transform.AddComponent<PointBuilder>();
        }
    }

}
