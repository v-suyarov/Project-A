using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSnapPoints : MonoBehaviour
{
    public CustomSnapPoints(GameObject parent)
    {
        this.parent = parent;
        snapPoints = new List<CustomSnapPoint>();
    }
    GameObject parent;
    List<CustomSnapPoint> snapPoints;

}
