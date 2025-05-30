using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public string url;

    public void Abrir()
    {
        Application.OpenURL(url);
    }

}
