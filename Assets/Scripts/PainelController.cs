using UnityEngine;public class PainelController : MonoBehaviour
{
    public GameObject painel;  // Referência ao painel que será controlado

    // Função para mostrar o painel
    public void MostrarPainel()
    {
        painel.SetActive(true);  // Torna o painel visível
    }

    // Função para fechar o painel
    public void FecharPainel()
    {
        painel.SetActive(false); // Torna o painel invisível
    }
}
