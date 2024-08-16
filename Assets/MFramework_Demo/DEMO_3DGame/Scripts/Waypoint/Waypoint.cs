using MFramework;
using System.Collections.Generic;
using UnityEngine;

public enum WaypointMode
{
    Loop,
    PingPong,
    Once
}

public class Waypoint : MonoBehaviour
{
    public WaypointMode mode;
    public float waitTime;
    public List<Transform> waypoints;

#if UNITY_EDITOR
    [Header("Editor Settings")]
    public bool drawGizmos = true;
#endif

    protected Transform m_current;

    protected bool m_pong;
    protected bool m_changing;

    public Transform current
    {
        get
        {
            if (!m_current)
            {
                m_current = waypoints[0];
            }

            return m_current;
        }

        protected set { m_current = value; }
    }

    public int index => waypoints.IndexOf(current);

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Vector3[] waypointsPos = new Vector3[waypoints.Count];
            for (int i = 0; i < waypoints.Count; i++)
            {
                waypointsPos[i] = waypoints[i].position;
            }

            if(mode == WaypointMode.Loop) Gizmos.DrawLineStrip(waypointsPos, true);
            else Gizmos.DrawLineStrip(waypointsPos, false);
        }
    }
#endif

    public virtual void Next()
    {
        if (m_changing)
        {
            return;
        }

        //вд0 1 2 3ЮЊР§
        if (mode == WaypointMode.PingPong)//0->1->2->3->2->1->0->....
        {
            if (!m_pong)
            {
                m_pong = (index + 1 == waypoints.Count);
            }
            else
            {
                m_pong = (index - 1 >= 0);
            }

            int next = m_pong ? index - 1 : index + 1;
            Change(next);
        }
        else if (mode == WaypointMode.Loop)//0->1->2->3->0->1->2->3...
        {
            if (index + 1 < waypoints.Count)
            {
                Change(index + 1);
            }
            else
            {
                Change(0);
            }
        }
        else if (mode == WaypointMode.Once)//0->1->2->3
        {
            if (index + 1 < waypoints.Count)
            {
                Change(index + 1);
            }
        }
    }

    protected virtual void Change(int to)
    {
        m_changing = true;

        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            current = waypoints[to];
            m_changing = false;
        }, waitTime);
    }
}
