using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI; 
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class JugadaHechizos : MonoBehaviour
{
    public GestorCartas gc;
    public GestorJugada gj;
    public JugadaEfectos efecto;

    public bool hechizos = false;
    public bool scoreInversion = false;
    public bool jugador0Booster = false;
    public bool jugador0Sapper = false;
    public bool jugador0PuedeHechizos = false;


    public IEnumerator Hechizos(int id){
        Debug.Log("Hechizos, jugador: " + id);
        gc.textoGuia.text = "Esperando al resto de jugadores";
        
        yield return new WaitForSeconds(gj.tiempoEspera);
        gj.tiempoEspera = 0;
        bool hehcizoJugado = false;

        List<Carta> mano = gc.BuscarMano(id);
        if(gj.jugadorVivo[id] && gc.TieneHechizosScore(mano)){
            int booster = 0, sapper = 0, inversion = 0;
            int[] posicionId;
            int[] posicionAtaque;

            ObtenerPosiciones(out posicionId, out posicionAtaque);

            for(int i = 0; i < mano.Count; i++){
                switch(mano[i].nombreImagen){      
                    case "ObjetoScoreBooster":
                    booster++;
                    break;

                    case "ObjetoScoreSapper":
                    sapper++;
                    break;

                    case "ObjetoScoreInversion":
                    inversion++;
                    break;
                }
            }

            bool intercambio = efecto.efecto5Activado != scoreInversion;
            if(inversion > 0 && ((posicionId[0] == id && !efecto.efecto5Activado) || posicionId[posicionId.Length-1] == id && efecto.efecto5Activado)){
                yield return new WaitForSeconds(2);
                ScoreInversion(id, mano);
                hehcizoJugado = true;
            }else if((booster > 0 || sapper > 0) && ((posicionId[posicionId.Length-1] != id && !intercambio) || (posicionId[0] != id && intercambio))){
                int diferenciaNoPerder = -1;
                int diferenciaGanar = -1;
                int noPerder = -1;
                int ganar = -1;

                int posicion = Array.IndexOf(posicionId, id);
                int distancia = 3 + 2 * (efecto.efecto6Activado ? 1 : 0);

                if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[1]){
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[1];
                    noPerder = 1;
                }else if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[2]){
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[2];
                    noPerder = 2;
                }else if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[posicionAtaque.Length-1]){
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[posicionAtaque.Length-1];
                    noPerder = 3;
                }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[2]){
                    diferenciaNoPerder = posicionAtaque[2] - posicionAtaque[posicionAtaque.Length-1];
                    noPerder = 2;
                }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[1]){
                    diferenciaNoPerder = posicionAtaque[1] - posicionAtaque[posicionAtaque.Length-1];
                    noPerder = 1;
                }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[0]){
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[posicionAtaque.Length-1];
                    noPerder = 0;
                }

                if(posicion != posicionAtaque.Length && !intercambio){
                    diferenciaGanar = posicionAtaque[posicion] - posicionAtaque[posicionAtaque.Length-1];
                    ganar = posicionAtaque.Length-1;
                }else if(posicion != 0 && intercambio){
                    diferenciaGanar = posicionAtaque[0] - posicionAtaque[posicion];
                    ganar = 0;
                }

                if((diferenciaNoPerder > -1 && diferenciaNoPerder < distancia) || (diferenciaGanar > -1 && diferenciaGanar < distancia)){
                    yield return new WaitForSeconds(2);

                    if(booster > 0){
                        if(!intercambio)
                            ScoreBooster(id, id, mano);
                        else if(posicion == 3)
                            ScoreBooster(id, noPerder, mano);
                        else
                            ScoreBooster(id, ganar, mano);
                        hehcizoJugado = true;
                    }else if(sapper > 0){
                        if(intercambio)
                            ScoreSapper(id, id, mano);
                        else if(posicion == 3)
                            ScoreSapper(id, noPerder, mano);
                        else if(diferenciaGanar > -1)
                            ScoreSapper(id, ganar, mano);
                        hehcizoJugado = true;
                    }
                }
            }
        }

        if(hehcizoJugado && gc.TieneHechizosScore(gc.manoDelJugador)){
            if(!gc.jugador0Bot){
                gc.textoGuia.text = "Escoge un hechizo para jugar";
                jugador0PuedeHechizos = true;
            }else
                StartCoroutine(Hechizos(0));
            yield break;
        }else if(hehcizoJugado){
            jugador0PuedeHechizos = false;
            StartCoroutine(Hechizos(1));
            yield break;
        }

        for(int i = id; i < 3; i++){
            if(gj.jugadorVivo[i+1]){
                StartCoroutine(Hechizos(i+1));
                yield break;
            }
        }

        hechizos = false;
        Invoke("ObtenerGanadores", 2);
        StartCoroutine(gj.AnimarCartasGanadoras());
    } 

    public void ObtenerGanadores(){
        gj.ObtenerGanadores();
    }

    public void HechizosJugador(GameObject carta, int id){
        if(gc.manoDelJugador[id].esHechizo){
            switch(gc.manoDelJugador[id].nombreImagen){
                case "ObjetoScoreBooster":
                    //ScoreBooster(0, gc.manoDelJugador);
                    //gc.OrdenarCartasMano();
                    gc.ImagenCartasMano[id].gameObject.SetActive(false);
                    gc.textoGuia.text = "Suma 2 de ataque a un jugador";
                    jugador0Booster = true;
                    break;

                case "ObjetoScoreSapper":
                   // gc.manoDelJugador.RemoveAt(id);
                    //gc.OrdenarCartasMano();
                    gc.ImagenCartasMano[id].gameObject.SetActive(false);
                    gc.textoGuia.text = "Quita 2 de ataque a un jugador";
                    jugador0Sapper = true;
                    break;

                case "ObjetoScoreInversion":
                    ScoreInversion(0, gc.manoDelJugador);
                    gc.OrdenarCartasMano();
                    break;

                default:
                    break;
            }
        }
        
        if(!gc.TieneHechizosScore(gc.manoDelJugador)){
            StartCoroutine(Hechizos(1));
            gc.textoGuia.text = "Esperando acciones de los jugadores";
        }else{
            gc.textoGuia.text = "Escoge un hechizo para jugar";
            gj.jugadaEnCurso = false;
        }
    }


    public void ScoreBooster(int id, int idObjetivo, List<Carta> mano){
        Debug.Log("Booster");
        gj.jugadaEnCurso = false;
        if(id == 0 && !gc.jugador0Bot){
            idObjetivo = idObjetivo - 5;
            jugador0Booster = false;
        }
        gj.bonusAtaque[idObjetivo] += 2 + 2 * (efecto.efecto6Activado ? 1 : 0);
        gj.ActualizarAtaques();
        gc.iconosHechizo[idObjetivo*3].gameObject.SetActive(true);
        int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreBooster", mano);
        mano.RemoveAt(idCartaMano);
        gc.GuardarMano(id, mano);
        if(id == 0)
            gc.OrdenarCartasMano();
        gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + "usa Booster sobre " + gc.nombreDinosaurios[idObjetivo];
        gc.AumentarLeyenda();
    }



    public void ScoreSapper(int id, int idObjetivo, List<Carta> mano){
        Debug.Log("Sapper");
        gj.jugadaEnCurso = false;
        if(id == 0 && !gc.jugador0Bot){
            idObjetivo = idObjetivo - 5;
            jugador0Sapper = false;
        }
        gj.bonusAtaque[idObjetivo] -= 2 + 2 * (efecto.efecto6Activado ? 1 : 0);
        gj.ActualizarAtaques();
        gc.iconosHechizo[idObjetivo*3+2].gameObject.SetActive(true);
        int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreSapper", mano);
        mano.RemoveAt(idCartaMano);
        gc.GuardarMano(id, mano);
        if(id == 0)
            gc.OrdenarCartasMano();
        gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + "usa Sapper sobre " + gc.nombreDinosaurios[idObjetivo];
        gc.AumentarLeyenda();
    }


    public void ScoreInversion(int id, List<Carta> mano){
        Debug.Log("Inversion");
        gj.jugadaEnCurso = false;
        scoreInversion = !scoreInversion;
        gj.ActualizarAtaques();
        gc.iconosHechizo[id*3+1].gameObject.SetActive(true);
        int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreInversion", mano);
        mano.RemoveAt(idCartaMano);
        gc.GuardarMano(id, mano);
    }

    void ObtenerPosiciones(out int[] posiciones, out int[] enterosOrdenados){
        // Crear una lista para almacenar los enteros y sus posiciones originales
        List<int[]> listaEnteros = new List<int[]>();

        // Convertir los elementos a enteros y almacenar en la lista
        for (int i = 0; i < gj.textoAtaque.Length; i++){
            if (gj.jugadorVivo[i]){
                int.TryParse(gj.textoAtaque[i].text, out int ataque);
                listaEnteros.Add(new int[] { ataque, i });
            }
        }

        // Ordenar la lista por los enteros
        listaEnteros.Sort((x, y) => x[0].CompareTo(y[0]));

        // Crear dos arrays para almacenar las posiciones y los enteros ordenados
        posiciones = new int[listaEnteros.Count];
        enterosOrdenados = new int[listaEnteros.Count];

        // Llenar los arrays con las posiciones y los enteros ordenados
        for (int i = 0; i < listaEnteros.Count; i++){
            posiciones[i] = listaEnteros[i][1];
            enterosOrdenados[i] = listaEnteros[i][0];
        }
    }
}
