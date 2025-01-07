using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public GameObject panel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
