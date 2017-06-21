using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius; // Радиус своего рода обзора
    [Range(0,360)]
    public float viewAngle;  // Угол обзора 

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float meshResolution;
    public int edgeResolveIteration;
    public float edgeDistThreshold; // для решения проблемы когда наш меш касался двух препятствий

    public float maskCutawayDst = 0.1f;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(FindTargetsWithDelay(0.2f)); // своего рода Update в 0.2 сек
    }
    /*
    private void Update()
    {
        // Конвертер число получается 14.970725
        int step = Mathf.RoundToInt(4.235f * 3.535f); // 15 // топ конвертер
        Debug.Log("RoundToInt: " + step);
        float step1 = Mathf.Round(4.235f * 3.535f); // 15
        Debug.Log("Round: " + step1);
        int step2 = (int)(4.235f * 3.535f); // 14
        Debug.Log("(int) + (int): " + step2);
    }
    */

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay); 
            FindVisibleTargets();
        }
    }
    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius,targetMask);
        

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    Debug.Log("Visible targ: " + target.name);
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector2> viewPoints = new List<Vector2>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDistThreshold;
                if (oldViewCast.hit != newViewCast.hit || oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector2.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector2.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCont = viewPoints.Count + 1;
        Vector3[] verticles = new Vector3[vertexCont]; // vertex - вершина (indices).
        int[] triangles = new int[(vertexCont - 2) * 3];

        verticles[0] = Vector3.zero;
        for (int i = 0; i < vertexCont -1; i++)
        {
            verticles[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector2.up * maskCutawayDst);
            if (i < vertexCont-2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = verticles; // !!!! 12:40 Field of view visualisation Sebastian Lague !!!! try Do 2dMesh by Triangulator.cs
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector2 minPoint = Vector2.zero;
        Vector2 maxPoint = Vector2.zero;
        for (int i = 0; i < edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast (float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

        if (hit)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector2 point;
        public float dst;
        public float angle;

        public ViewCastInfo (bool _hit, Vector2 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    // Для более коректной работы viewVisualization, вместо MeshResolution 10 т.е увеличиваем производительность используя меньшее количество лучей.
    public struct EdgeInfo
    {
        public Vector2 pointA;
        public Vector2 pointB;

        public EdgeInfo(Vector2 _pointA, Vector2 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
