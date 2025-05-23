using UnityEngine;
using TMPro; // se você usa TextMeshPro, senão use UnityEngine.UI para Text

public class MensagemController : MonoBehaviour
{
    public TextMeshProUGUI textoMensagem;  // arraste aqui o texto UI no Inspector

    private Coroutine mostrarMensagemCoroutine;

    // Método para mostrar a mensagem por 5 segundos


    public void MostrarMensagemTemporaria(string mensagem)
    {
        if (mostrarMensagemCoroutine != null)
        {
            StopCoroutine(mostrarMensagemCoroutine);
        }
        mostrarMensagemCoroutine = StartCoroutine(MostrarMensagem(mensagem));
    }

    private System.Collections.IEnumerator MostrarMensagem(string mensagem)
    {
        textoMensagem.text = mensagem;
        textoMensagem.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        textoMensagem.gameObject.SetActive(false);
        mostrarMensagemCoroutine = null;
    }
}
