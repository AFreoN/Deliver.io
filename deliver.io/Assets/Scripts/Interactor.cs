using UnityEngine;
using System.Collections.Generic;

public class Interactor : MonoBehaviour
{
    public static List<Pair> allPairs = new List<Pair>();

    public static void handleInteraction(PlayerController pc, EnemyController ec)
    {
        GameObject g1 = pc.gameObject, g2 = ec.gameObject;
        if (isPairExists(g1, g2))
            return;

        bool havePlate1 = pc.havePlates, havePlate2 = ec.havePlate;
        bool inView1 = g2.transform.IsInView(g1.transform, ControlValues.viewAngle);
        bool inView2 = g1.transform.IsInView(g2.transform, ControlValues.viewAngle);

        var v = doCalc(havePlate1,havePlate2,inView1, inView2);

        pc.GetInteractiosStatus(v.Item1, ec);
        ec.getInteractionStatus(v.Item2, pc);
    }

    public static void handleInteraction(EnemyController ec, PlayerController pc)
    {
        GameObject g1 = ec.gameObject, g2 = pc.gameObject;
        if (isPairExists(g1, g2))
            return;

        bool havePlate1 = ec.havePlate, havePlate2 = pc.havePlates;
        bool inView1 = g2.transform.IsInView(g1.transform, ControlValues.viewAngle);
        bool inView2 = g1.transform.IsInView(g2.transform, ControlValues.viewAngle);

        var v = doCalc(havePlate1, havePlate2, inView1, inView2);

        ec.getInteractionStatus(v.Item1, pc);
        pc.GetInteractiosStatus(v.Item2, ec);
    }

    public static void handleInteraction(EnemyController ec1, EnemyController ec2)
    {
        GameObject g1 = ec1.gameObject, g2 = ec2.gameObject;
        if (isPairExists(g1, g2))
            return;

        bool havePlate1 = ec1.havePlate, havePlate2 = ec2.havePlate;
        bool inView1 = g2.transform.IsInView(g1.transform, ControlValues.viewAngle);
        bool inView2 = g1.transform.IsInView(g2.transform, ControlValues.viewAngle);

        var v = doCalc(havePlate1, havePlate2, inView1, inView2);

        ec1.getInteractionStatus(v.Item1, ec2);
        ec2.getInteractionStatus(v.Item2, ec1);
    }

    static (Interaction,Interaction) doCalc(bool havePlate1, bool havePlate2, bool inView1, bool inView2)
    {
        Interaction I1 = Interaction.None, I2 = Interaction.None;

        if (havePlate1 && havePlate2)        //If both of them have plate
        {
            if (inView1)
                I1 = Interaction.None;
            else
                I1 = Interaction.Steal;

            if (inView2)
                I2 = Interaction.None;
            else
                I2 = Interaction.Steal;
        }
        else if (havePlate1 && havePlate2 == false)
        {
            if (inView2)
            {
                I1 = Interaction.None;
                I2 = Interaction.Dodge;
            }
            else
            {
                I1 = Interaction.Give;
                I2 = Interaction.Steal;
            }
        }
        else if (havePlate1 == false && havePlate2)
        {
            if (inView1)
            {
                I1 = Interaction.Dodge;
                I2 = Interaction.None;
            }
            else
            {
                I1 = Interaction.Steal;
                I2 = Interaction.Give;
            }
        }
        else
        {
            I1 = Interaction.Dodge;
            I2 = Interaction.Dodge;
        }

        return (I1, I2);
    }

    public static bool isPairExists(GameObject g1, GameObject g2)
    {
        bool result = false;

        foreach(Pair p in allPairs)
        {
            if(g1 == p.g1 && g2 == p.g2 || g1 == p.g2 && g2 == p.g1)
            {
                result = true;
                break;
            }
        }

        if(result == false)
        {
            Pair p = new Pair(g1, g2);
            allPairs.Add(p);

            FunctionTimer.Create( () => removePair(p), .25f);
        }

        return result;
    }

    static void removePair(Pair pair)
    {
        if (allPairs.Contains(pair))
            allPairs.Remove(pair);
    }

}

public enum Interaction
{
    Steal,
    Dodge,
    Give,
    None
}

[System.Serializable]
public class Pair
{
    public GameObject g1;
    public GameObject g2;

    public Pair(GameObject gam1, GameObject gam2)
    {
        g1 = gam1;
        g2 = gam2;
    }
}
