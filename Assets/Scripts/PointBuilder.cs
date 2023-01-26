using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[ExecuteAlways]


public class PointBuilder : MonoBehaviour
{
    enum Basis
    {
        AXIS_X,
        AXIS_Y,
        AXIS_Z
    }
    PointBuilder()
    {

    }
    static Dictionary<PointBuilder,List<CustomSnapPoint>> points = new Dictionary<PointBuilder, List<CustomSnapPoint>>();
    
    static GameObject SnapPointsContainer;
    [Min(min: 1)]
    public Vector3Int countPoints = new Vector3Int(1, 1, 1);
    public Dimension dimensionX;
    public Dimension dimensionY;
    public Dimension dimensionZ;
    private Transform oldTransform;

    private Vector3 oldCountPoints;
    public Mesh mesh;
    [Min(0.01f)]
    public float sizePoint =0.1f;
    Dictionary<Basis,List<Vector3>> basis = new Dictionary<Basis, List<Vector3>>();
    private void Start()
    {
        
        points.Add(this, new List<CustomSnapPoint>());
        oldCountPoints = countPoints;
        oldTransform= transform;
        mesh = transform.GetComponent<MeshFilter>().sharedMesh;  
        InitBasis();
        InitDimensions();
        
        UpdateSnapPoint();


    }
    private void InitBasis()
    {
        float minX = float.PositiveInfinity;
        float minY = float.PositiveInfinity;
        float minZ = float.PositiveInfinity;
        int minIndexX = int.MaxValue;
        int minIndexY=int.MaxValue;
        int minIndexZ=int.MaxValue;

        float maxX = float.NegativeInfinity;
        float maxY = float.NegativeInfinity;
        float maxZ = float.NegativeInfinity;
        int maxIndexX = int.MinValue;
        int maxIndexY = int.MinValue;
        int maxIndexZ = int.MinValue;

        int index = 0;
        foreach (var vertex in mesh.vertices)
        {
            if (vertex.x < minX)
            {
                minX = vertex.x;
                minIndexX = index;
            }
                
            if(vertex.y<minY)
            {
                minIndexY=index;
                minY = vertex.y;
            }

            if (vertex.z < minZ)
            {
                minIndexZ=index;
                minZ = vertex.z; 
            }
            if (vertex.x > maxX)
            {
                maxX = vertex.x;
               maxIndexX = index;
            }
            if(vertex.y> maxY)
            {
                maxY = vertex.y;
                maxIndexY = index;
            }
            if (vertex.z > maxZ)
            {
                maxZ = vertex.z;
                maxIndexZ = index;
            }
            index++;    
        }

        Vector3 origin = new Vector3(minX, minY, minZ);
        List<Vector3> axisX = new List<Vector3>() { origin, new Vector3(maxX, minY, minZ) };
        List<Vector3> axisY = new List<Vector3>() { origin, new Vector3(minX, maxY, minZ) };
        List<Vector3> axisZ = new List<Vector3>() { origin, new Vector3(minX, minY, maxZ) };
        basis.Add(Basis.AXIS_X, axisX);
        basis.Add(Basis.AXIS_Y, axisY);
        basis.Add(Basis.AXIS_Z, axisZ);

    }
    public void UpdateSnapPoint()
    {
        RemoveSnapPoint();
        CreateSnapPoint();
    }
    private void Update()
    {
        
        if(UpdateCountPoints())
        {
            UpdateDimensions();
            UpdateSnapPoint();
        }
        else if (transform.hasChanged)
        {
            transform.hasChanged = false;

            UpdateDimensions();
            UpdateSnapPoint();
            Debug.Log("X min" + dimensionX.A + " X max" + dimensionX.B);
            Debug.Log("Y min" + dimensionY.A + " Y max" + dimensionY.B);
            Debug.Log("Z min" + dimensionZ.A + " Z max" + dimensionZ.B);
            
        }
    }
    private void CreateSnapPoint()
    {
        
        foreach (var axisX in dimensionX.Pos)
        {
            
            foreach (var axisY in dimensionY.Pos)
            {
                
                
                foreach (var axisZ in dimensionZ.Pos)
                {
                    float x = transform.InverseTransformPoint(axisX).x;
                    float y = transform.InverseTransformPoint(axisY).y;
                    float z = transform.InverseTransformPoint(axisZ).z;
                    
                    


                    

                    points[this].Add(new CustomSnapPoint(transform.TransformPoint(x,y,z), this));

                }
            }
        }
    }

    public Dictionary<PointBuilder,List<CustomSnapPoint>> GetAllPoints()
    {
        return points;
    }
    public List<CustomSnapPoint> GetTargetPoints(PointBuilder key)
    {
        List<CustomSnapPoint> targetPoints;
        if(points.TryGetValue(key,out targetPoints))
        {
            return targetPoints;
        }
        return null;
    }
   
    private void UpdateDimensions()
    {
        Bounds bounds = transform.GetComponent<MeshRenderer>().bounds;
        Vector3 center = transform.GetComponent<MeshRenderer>().bounds.center;
        
        dimensionX.UpdateValue(transform.TransformPoint(basis[Basis.AXIS_X][0]), transform.TransformPoint(basis[Basis.AXIS_X][1]), center);
        dimensionY.UpdateValue(transform.TransformPoint(basis[Basis.AXIS_Y][0]), transform.TransformPoint(basis[Basis.AXIS_Y][1]), center);
        dimensionZ.UpdateValue(transform.TransformPoint(basis[Basis.AXIS_Z][0]), transform.TransformPoint(basis[Basis.AXIS_Z][1]), center);
        dimensionX.UpdatePos();
        dimensionY.UpdatePos();
        dimensionZ.UpdatePos();
    }
    private void InitDimensions()
    {

        Vector3[] vertex = mesh.vertices;
        Vector3 center = transform.GetComponent<MeshRenderer>().bounds.center;

        dimensionX = new Dimension(transform.TransformPoint(basis[Basis.AXIS_X][0]), transform.TransformPoint(basis[Basis.AXIS_X][1]), center, countPoints.x);
        dimensionY = new Dimension(transform.TransformPoint(basis[Basis.AXIS_Y][0]), transform.TransformPoint(basis[Basis.AXIS_Y][1]), center, countPoints.y);
        dimensionZ = new Dimension(transform.TransformPoint(basis[Basis.AXIS_Z][0]), transform.TransformPoint(basis[Basis.AXIS_Z][1]), center, countPoints.z);

    }
    private bool UpdateCountPoints()
    {
        bool check = false;
        if (UpdateCountPointsX())
        {
            dimensionX.ChangeDimension(countPoints.x);
            check= true;
        }
            
        if (UpdateCountPointsY())
        {
            dimensionY.ChangeDimension(countPoints.y);
            check= true;
        }
            
        if (UpdateCountPointZ())
        {
            dimensionZ.ChangeDimension(countPoints.z);
            check= true;
        }

        return check;
        
    }

    private void RemoveSnapPoint()
    {
        points[this].Clear();
    }
    private bool UpdateCountPointsX()
    {   if (countPoints.x == oldCountPoints.x) 
            return false;
        oldCountPoints = countPoints;
        return true;
    }
    private bool UpdateCountPointsY()
    {   if (countPoints.y == oldCountPoints.y)
            return false;
        oldCountPoints = countPoints;
        return true; }
    private bool UpdateCountPointZ()
    {   if (countPoints.z == oldCountPoints.z)
            return false;
        oldCountPoints = countPoints;
        return true; 
    }

    private void OnDrawGizmos()
    {
        List<CustomSnapPoint> c;
        if (!points.TryGetValue(this, out c))
        {
            return;
        }
        foreach (var point in points[this])
        {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point.pos, sizePoint);
            

        }
        Gizmos.color = Color.red;

        /*foreach (var point in dimensionX.Pos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.15f);
        }
        foreach (var point in dimensionY.Pos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.15f);
        }
        foreach (var point in dimensionZ.Pos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.15f);
        }
        Gizmos.DrawLine(transform.TransformPoint(basis[Basis.AXIS_X][0]), transform.TransformPoint(basis[Basis.AXIS_X][1]));
        Gizmos.DrawLine(transform.TransformPoint(basis[Basis.AXIS_Y][0]), transform.TransformPoint(basis[Basis.AXIS_Y][1]));
        Gizmos.DrawLine(transform.TransformPoint(basis[Basis.AXIS_Z][0]), transform.TransformPoint(basis[Basis.AXIS_Z][1]));*/
    }
}
