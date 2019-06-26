using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] private RoadSystem roadSystem;
    [SerializeField] private float speed;

    private int currentTarget;
    private List<Vector3> waypoints;
    private bool move = true;
    private int roadCount;

    private void Update()
    {
        if (Waypoints == null) return;
        float distance = Vector3.Distance(transform.position, new Vector3(waypoints[currentTarget].x, transform.position.y, waypoints[currentTarget].z));

        if (move)
        {
            if (distance > 0.1f)
            {
                Vector3 target = waypoints[currentTarget];
                //Vector3 pos = Vector3.MoveTowards(transform.position, new Vector3(target.x, transform.position.y, target.z), speed * Time.deltaTime);
                Vector3 pos = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, target.z), speed * Time.deltaTime);
                GetComponent<Rigidbody>().MovePosition(pos);
                Quaternion targetRotation = Quaternion.Euler( Vector3.up * RotationAngle(target));
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 1f); 
            }
            else
            {
                currentTarget = (currentTarget + 1) % waypoints.Count;
                print("currentPoint: " + currentTarget);


                if (currentTarget == waypoints.Count - 1)
                {
                    //road.ClearPreviousTunnels();
                    move = false;
                }

                if ((currentTarget % 10) == 0)
                {
                    roadCount++;
                    if ((roadCount) % 3 == 0)
                    {
                        //roadSystem.CreateNewTunnels();
                    }
                }
            }
        }
    }

    float RotationAngle(Vector3 target)
    {
        return Mathf.Atan2(target.x - transform.position.x, target.z - transform.position.z) * 180 / Mathf.PI;
    }

    public List<Vector3> Waypoints
    {
        set { waypoints = value; }
        get { return waypoints; }
    }

    public bool Move { set { move = value; } }
}
