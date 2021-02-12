using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] Transform canvas = null;
    Transform mainCamera = null;

    [SerializeField] Text nameText = null;
    public Text levelText = null;

    void Start()
    {
        mainCamera = Camera.main.transform;
        nameText.text = transform.name;
        nameText.color = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetColor("_BaseColor");
        levelText.text = "0";
    }

    private void LateUpdate()
    {
        canvas.forward = Vector3.Lerp(canvas.forward, mainCamera.forward, .7f);
    }

    string getLvlText(int num)
    {
        return "Lvl <color=#FFFFFF>" + num.ToString() + "</color>";
    }
}
