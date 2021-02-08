using UnityEngine;
using System.Collections.Generic;

public static class Utilities
{
    public static Transform[] ToTransformArray(this GameObject[] g)
    {
        Transform[] result = new Transform[g.Length];
        for(int i = 0; i < g.Length; i++)
        {
            result[i] = g[i].transform;
        }
        return result;
    }

    public static Component[] ToCustomArray(this GameObject[] gameobjects, System.Type type)
    {
        Component[] c = new Component[gameobjects.Length];
        for (int i = 0; i < gameobjects.Length; i++)
        {
            c[i] = gameobjects[i].GetComponent(type);
        }
        return c;
    }

    public static bool IsInView(this Transform to, Transform target, float maxAngle)
    {
        bool result = false;

        float ang = Vector3.Angle(to.forward, (target.position - to.position).normalized);
        result = ang <= maxAngle * .5f ? true : false;

        return result;
    }

    public static Vector3 normalizeXZ(this Transform t, Space s = Space.World)
    {
        Vector3 pos = s == Space.World ? t.position : t.localPosition;
        pos = new Vector3(pos.x, 0, pos.z);
        return pos.normalized;
    }

    public static void alignTransformPositions(this List<Transform> tList, float stackDistance, Directions dir, Transform parent = null)
    {
        for(int i = 0; i < tList.Count; i++)
        {
            Transform t = tList[i];
            t.SetParent(parent);
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero + dir.enumToVector3(t) * (i * stackDistance);
        }
    }

    public static Vector3 enumToVector3(this Directions dir, Transform t = null)
    {
        Vector3 result = Vector3.up;

        switch(dir)
        {
            case Directions.Up:
                result = t != null ? t.up : Vector3.up;
                break;

            case Directions.Down:
                result = t != null ? -t.up : Vector3.down;
                break;

            case Directions.Forward:
                result = t != null ? t.forward : Vector3.forward;
                break;

            case Directions.Backward:
                result = t != null ? -t.forward : Vector3.back;
                break;

            case Directions.Right:
                result = t != null ? t.right : Vector3.right;
                break;

            case Directions.Left:
                result = t != null ? -t.right : Vector3.left;
                break;
        }

        return result;
    }
}

public enum Directions
{
    Up,
    Down,
    Forward,
    Backward,
    Right,
    Left
}
