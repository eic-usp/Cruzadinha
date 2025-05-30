using UnityEngine;
using TMPro; 

public class MensagemController : MonoBehaviour
{
    public TextMeshProUGUI textoMensagem;  

    private Coroutine mostrarMensagemCoroutine;

   


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
