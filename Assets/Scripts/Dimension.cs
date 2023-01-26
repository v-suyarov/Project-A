using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class Dimension 
{
    public Dimension(Vector3 a, Vector3 b, Vector3 center, int count)
    {
        Pos = new List<Vector3> { };
        A = a;
        B = b;
        Centre = center;
        if (A == B)
            Debug.LogError("Min and max must not be equal");
        ChangeDimension(count);

    }

    public Vector3 A { get; set; }
    public Vector3 Centre { get; set; }
    public Vector3 B { get; set; }
    public List<Vector3> Pos { get; }
    public void UpdateValue(Vector3 a,Vector3 b,Vector3 centre)
    {
        A = a;
        Centre = centre;
        B= b;
    }
    public void UpdatePos()
    {
        ChangeDimension(Pos.Count);
    }
    public void ChangeDimension(int count)
    {
        if (count < 1)
            return;
        if (count == 1)
        {
            Pos.Clear();
            Pos.Add(Centre);
        }
        else if (count >= 2)
        {
            Pos.Clear();
  
            int currentStep = 0;

            float progress = 0; 
            while (progress<=1)
            {
                Pos.Add(Vector3.Lerp(A,B,progress));
                currentStep++;
                progress = (float)currentStep / (count-1);
            }
        }
    }
}
