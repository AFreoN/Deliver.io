using UnityEngine;

public class Table : MonoBehaviour
{

    public Table getTable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        return this;
    }

    public void notThisTable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
