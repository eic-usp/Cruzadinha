using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject painel; 

    //chama a função no botão e deixa ela verdaderia (visivel)
    public void MostrarPainel(){

        painel.SetActive(true);
    }

    public void FecharPainel()
    {
        painel.SetActive(false);
    }
}
