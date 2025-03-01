using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Carta 
{

    public string nombreImagen;
    public int ataque;
    public bool esHechizo;
    public int id;
    public bool efecto;
    public int idJugador;

    public bool cartaJugada;


    // Start is called before the first frame update
    public Carta(string _nombreImagen, int _ataque, int _id, bool _efecto, bool _esHechizo){
        nombreImagen = _nombreImagen;
        ataque = _ataque;
        id = _id;
        esHechizo = _esHechizo;
        efecto = _efecto;
    }

    public void SetJugador(int id){
        idJugador = id;
    }
}
