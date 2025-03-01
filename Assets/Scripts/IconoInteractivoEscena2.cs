using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconoInteractivoEscena2 : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public GestorCartas gc;
    public GestorJugada gj;

    private void OnMouseDown(){
        if(gc.etapaDescarte){
            gc.DarCartaDesastre(0, id);
            Debug.Log("15");
        }else if(!gj.iconoEscogido && gj.acabarAnimacion)
            gc.Efecto1MostrarCartas(id);
    }
}
