using UnityEngine;
using TMPro;

public class TecladoVirtual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI campoTexto;

    private void Start()
    {
        campoTexto.text = ""; // no primeiro start, inicia a tecla vazia
    }

    public void EntradaDeCaractere(string _caractere)
    {
        campoTexto.text += _caractere; // aqui ele coloca os textos no campo
    }
}