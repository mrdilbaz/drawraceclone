using System.Collections;
using System.Collections.Generic;
using burningmime.curves;
using SplineMesh;
using UnityEngine;
using Vectrosity;
public class LineDrawer : MonoBehaviour {

    public VectorObject2D vector;
    public GameObject splineMesh;

    public SplineMesh.Spline spline;
    private void OnEnable () {
        vector = FindObjectOfType<VectorObject2D> ();

        InvokeRepeating ("LowFrequencyUpdate", 0, 0.033f);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonUp (0)) {
            ProcessLine ();
        }

        if (Input.GetMouseButtonUp (1)) {
            vector.vectorLine.points2.Clear ();
            vector.vectorLine.Draw ();
        }
    }

    void LowFrequencyUpdate () {
        if (Input.GetMouseButton (0)) {
            vector.vectorLine.points2.Add (Input.mousePosition);
            vector.vectorLine.Draw ();
        }
    }

    void ProcessLine () {
        List<Vector2> points = CurvePreprocess.RdpReduce (vector.vectorLine.points2, 2f);
        points = ProcessPoints (points);
        vector.vectorLine.points2 = points;
        vector.vectorLine.Draw ();

        spline.Reset ();


        for (int i = 0; i < points.Count-1; i++) {
            spline.AddNode (new SplineMesh.SplineNode (points[i],points[i] + (points[i+1] - points[i]).normalized));
        }

        spline.AddNode(new SplineMesh.SplineNode(points[points.Count-1],points[points.Count-1] + (points[points.Count-1] - points[points.Count-2]).normalized));
        
        spline.gameObject.GetComponent<SplineSmoother>().SmoothAll();

        

        spline.GetComponent<SplineMeshTiling>().CreateMeshes();
        spline.GetComponent<SplineMeshTiling>().rotation = new Vector3(0,0,-90);
        spline.GetComponent<SplineMeshTiling>().CreateMeshes();
    }

    List<Vector2> ProcessPoints (List<Vector2> _points) {
        Vector2 start = _points[0];
        for (int i = 0; i < _points.Count; i++) {
            _points[i] -= start;
            _points[i] *= 0.1f;
        }
        return _points;
    }
}