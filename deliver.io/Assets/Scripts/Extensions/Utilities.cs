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

    public static Transform[] ToTransformArray(this Component[] components)
    {
        Transform[] t = new Transform[components.Length];
        for(int i = 0; i < components.Length; i++)
        {
            t[i] = components[i].transform;
        }
        return t;
    }

    public static bool IsInView(this Transform to, Transform target, float maxAngle)
    {
        bool result = false;

        float ang = Vector3.Angle(to.forward, (target.position.xz() - to.position.xz()).normalized);
        result = ang <= maxAngle ? true : false;

        //Debug.Log("angle = " + ang);
        //if(to.gameObject.CompareTag(TagsLayers.enemyTag))

        return result;
    }

    public static bool IsInView(this Transform t, Vector3 pos, float maxAngle)
    {
        bool result = false;

        float ang = Vector3.Angle(t.forward, (pos.xz() - t.position.xz()).normalized );
        result = ang <= maxAngle ? true : false;
        //Debug.Log("New Angle = " + ang);

        return result;
    }

    public static bool IsInView(this Collision collision, float maxAngle)
    {
        bool result = false;
        Transform transform = collision.transform;
        Vector3 position = collision.GetContact(0).point.xz();

        float ang = Vector3.Angle(transform.forward, (position.xz() - transform.position.xz()).normalized);
        result = ang <= maxAngle ? true : false;
        Debug.Log("angle = " + ang);

        return result;
    }

    public static Vector3 normalizeXZ(this Transform t, Space s = Space.World)
    {
        Vector3 pos = s == Space.World ? t.position : t.localPosition;
        pos = new Vector3(pos.x, 0, pos.z);
        return pos.normalized;
    }

    public static Vector3 xz(this Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public static Vector3 yz(this Vector3 v)
    {
        return new Vector3(0, v.y, v.z);
    }

    public static Vector3 xy(this Vector3 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector3 getFaceDirection(this Transform t, Transform facing)
    {
        Vector3 direction = facing.position - t.position;
        direction = direction.xz() + Vector3.up * t.position.y;

        return direction;
    }

    public static void alignTransformPositions(this List<Transform> tList, float stackDistance, Directions dir, Transform parent = null, Space space = Space.World)
    {
        for(int i = 0; i < tList.Count; i++)
        {
            Transform t = tList[i];
            t.SetParent(parent);
            t.localRotation = Quaternion.identity;

            Vector3 direction = space == Space.World ? dir.enumToVector3() : dir.enumToVector3(t);
            t.localPosition = Vector3.zero + direction * (i * stackDistance);
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
