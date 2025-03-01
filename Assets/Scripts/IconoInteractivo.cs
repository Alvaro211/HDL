using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconoInteractivo : MonoBehaviour
{
    // Start is called before the first frame update
    public SelecciónJugador sj;
    public int id;
    private void OnMouseDown()
    {
        // Cuando se hace clic en la carta, llama a la función para desactivarla
        sj.NuevoJugador(id);
    }
}
