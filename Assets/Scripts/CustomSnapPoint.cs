using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSnapPoint
{
    public CustomSnapPoint(Vector3 pos, PointBuilder parent)
    {
        this.pos = pos;
        this.parent = parent;
    }
    public CustomSnapPoint(float x,float y, float z,PointBuilder pointBuilder) {
        pos = new Vector3(x,y,z);
        this.parent = pointBuilder;
    }
    public Vector3 pos { get; set; }
    public PointBuilder parent { get; set; }
   
}
