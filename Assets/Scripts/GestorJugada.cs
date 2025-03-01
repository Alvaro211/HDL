using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI; 
using UnityEngine;
using System.Linq;
using System;
using TMPro;


public class GestorJugada : MonoBehaviour{

    public GestorCartas gc;
    public JugadaEfectos efecto;
    public JugadaHechizos hechizo;
    public IA ia;

    //public bool efectos = false;
    // public bool hechizos = false;
    public bool empate = false;
    // public bool efectosFinRonda = false;
    // public int idEfectos = 0;
    public int idJugador = -1;
    public bool iconoEscogido;
    public TextMeshProUGUI[] textoAtaque;
    public int[] bonusAtaque = new int[4];
    public bool jugadaEnCurso = false;

    public List<int> idJugadorEfecto = new List<int>();
   // public List<int> idEfecto = new List<int>();
    //public bool[] efectoJugado = new bool[4];
    public bool[] jugadorVivo = new bool[4];
    

    public Carta[] cartasRonda = new Carta[4];
    private Vector3 escalaInicial;
    private Vector3 escalaFinal = new Vector3(1.2f, 1.2f, 1.2f);
    public bool acabarAnimacion = false;

    public bool esperandoAccion = false;
    // public bool efecto0Activo = false; 
    // public bool efecto2Activo = false;
    // public bool efecto4Acabado;
    // public bool efecto5Activado = false;
    // public bool efecto6Activado = false;
    // public bool efecto7Activo = false;
    // public List<int> efecto8Jugadores = new List<int>();

    // public Carta[] cartaEfecto7 = new Carta[2];

    
    public bool[] idPerdedores = new bool[4];
    public int numPerdedores = 0;

    // int posicion1;
    // int posicion2;
    public int tiempoEspera = 0;
    public bool[] ganadores = new bool[4];
    int algoritmoIA;


    // public bool scoreInversion = false;
    // public bool jugador0Booster = false;
    // public bool jugador0Sapper = false;
    // public bool jugador0PuedeHechizos = false;

    // Start is called before the first frame update
    void Start(){
        algoritmoIA = PlayerPrefs.GetInt("DificultadIA");
        TextoAtaqueSetActive(false);
    }

    public void TextoAtaqueSetActive(bool b){
        for (int i = 0; i < textoAtaque.Length; i++){
            if(jugadorVivo[i])
                textoAtaque[i].gameObject.SetActive(b);
            else    
                textoAtaque[i].gameObject.SetActive(false);
        }
    }


    //Revisa quien tiene la carta de mayor ataque y obtiene el o los ganadores
    public void ObtenerGanadores(){
        int[] ataqueRonda = new int[4];
        int maxAtaque = cartasRonda[0].ataque + bonusAtaque[0];
        int minAtaque = 100;
        numPerdedores = 0;

        //Obtenemos el mayor ataque
        for (int i = 0; i < cartasRonda.Length; i++){
            if(jugadorVivo[i] && (!empate || idPerdedores[i])){
                int ataque = cartasRonda[i].ataque + bonusAtaque[i];
                ataqueRonda[i] = ataque;
                if (ataque > maxAtaque)
                    maxAtaque = ataque;
                if(ataque < minAtaque)
                    minAtaque = ataque;
            }
        }

        //Efecto 5
        if((efecto.efecto5Activado == !hechizo.scoreInversion)){
            gc.leyenda.text += "\n Se intercambian la mejor puntuacion por la peor";
            gc.AumentarLeyenda();
            for (int i = 0; i < ataqueRonda.Length; i++){
                if(jugadorVivo[i]){
                    if(minAtaque == ataqueRonda[i])
                        textoAtaque[i].text = maxAtaque.ToString();
                    else if(maxAtaque == ataqueRonda[i])
                        textoAtaque[i].text = minAtaque.ToString();
                }
            }
        }

        //Obtenemos los ganadores con el mayor ataque
        for (int i = 0; i < ganadores.Length; i++){
            if(jugadorVivo[i] && (!empate || idPerdedores[i])){
                int.TryParse(textoAtaque[i].text, out int ataque);
                ganadores[i] = (ataque == maxAtaque);
                if(minAtaque == ataque){
                    numPerdedores++;
                    idPerdedores[i] = true; 
                }else{
                    idPerdedores[i] = false;
                }
            }
        }
    }


    public IEnumerator AnimarCartasGanadoras(){

        yield return new WaitForSeconds(2);
        tiempoEspera = 0;
        Debug.Log("AnimarCartas");
        gc.textoGuia.text = "Ganadores obtenidos";
        
        int repeticiones = 2;
        float duracionAumento = 0.5f;
        float duracionDisminucion = 0.5f;
        Vector3 escalaInicial = gc.cartaUtilizada[0].rectTransform.localScale;
        Vector3 escalaFinal = new Vector3(1.2f, 1.2f, 1.2f);

        for (int rep = 0; rep < repeticiones; rep++){
            float tiempoInicio = Time.time;
            float tiempoFinAumento = tiempoInicio + duracionAumento;
            float tiempoFinDisminucion = tiempoFinAumento + duracionDisminucion;

            while (Time.time < tiempoFinDisminucion){
                float tiempoPasado = Time.time - tiempoInicio;

                if (Time.time < tiempoFinAumento){
                    // Durante la fase de aumento gradual
                    float fraccionDeTiempoAumento = tiempoPasado / duracionAumento;
                    for (int i = 0; i < ganadores.Length; i++){
                        if (ganadores[i]){
                            // Interpola entre escalaInicial y escalaFinal durante el aumento
                            float factorEscalaAumento = Mathf.Lerp(1f, escalaFinal.x, fraccionDeTiempoAumento);
                            gc.cartaUtilizada[i].rectTransform.localScale = new Vector3(factorEscalaAumento, factorEscalaAumento, factorEscalaAumento);
                            gc.cartaUtilizadaEfectos[i].rectTransform.localScale = new Vector3(factorEscalaAumento, factorEscalaAumento, factorEscalaAumento);
                        }
                    }
                }
                else{
                    // Durante la fase de disminución gradual
                    float fraccionDeTiempoDisminucion = (tiempoPasado - duracionAumento) / duracionDisminucion;
                    for (int i = 0; i < ganadores.Length; i++){
                        if (ganadores[i]){
                            // Interpola entre escalaFinal y escalaInicial durante la disminución
                            float factorEscalaDisminucion = Mathf.Lerp(escalaFinal.x, 1f, fraccionDeTiempoDisminucion);
                            gc.cartaUtilizada[i].rectTransform.localScale = new Vector3(factorEscalaDisminucion, factorEscalaDisminucion, factorEscalaDisminucion);
                            gc.cartaUtilizadaEfectos[i].rectTransform.localScale = new Vector3(factorEscalaDisminucion, factorEscalaDisminucion, factorEscalaDisminucion);
                        }
                    }
                }
                yield return null;
            }
        }
        gc.AcabarRonda(cartasRonda);
    }



    public void ActualizarAtaques(){
        for (int i = 0; i < textoAtaque.Length; i++){
            if(jugadorVivo[i]){
                if(!empate){
                    textoAtaque[i].gameObject.SetActive(true);
                    textoAtaque[i].text = (cartasRonda[i].ataque + bonusAtaque[i]).ToString();
                }else if(idPerdedores[i]){
                    textoAtaque[i].gameObject.SetActive(true);
                    textoAtaque[i].text = (cartasRonda[i].ataque).ToString();
                }
            }
        }
    }

    public IEnumerator EmpezarAutomatico(){
        yield return new WaitForSeconds(0.5f);
        EmpezarJugada();
    }

    //Función para obtener las cartas jugadas de cada jugador y saber como se va a desarrollar la ronda
    public void EmpezarJugada(){
        Debug.Log("Jugada en curso:" + jugadaEnCurso);
        if(!jugadaEnCurso){
            if(gc.cartaUtilizada[0].gameObject.activeSelf)
                jugadaEnCurso = true;
            efecto.efecto4Acabado = true;
            if(esperandoAccion && efecto.idEfectos != 7)
                esperandoAccion = false;

            if(gc.etapaDescarte){
                gc.GuardarVariables();
                SceneManager.LoadScene("Puntuaciones");
                return;
            }

            if(hechizo.hechizos && !hechizo.jugador0Sapper && !hechizo.jugador0Booster){
                StartCoroutine(hechizo.Hechizos(1));
                return;
            }

            //Primera pulsacion del boton para revelar cartas
            if(((gc.cartaUtilizada[0].gameObject.activeSelf || gc.jugador0Bot) || (empate && !idPerdedores[0])) && !efecto.efectosFinRonda && !hechizo.hechizos){
                acabarAnimacion = false;
                escalaInicial = gc.cartaUtilizada[0].rectTransform.localScale;
                
                if(!efecto.efectos){
                    if(!empate){
                        int idCarta;
                        if(!gc.jugador0Bot){
                            cartasRonda[0] = gc.manoDelJugador[gc.idManoCartaSeleccionada];
                            gc.manoDelJugador.RemoveAt(gc.idManoCartaSeleccionada);
                            gc.leyenda.text =  gc.nombreDinosaurios[0] + " ha utilizado la carta " + cartasRonda[0].nombreImagen.Substring(7);
                        }else if(gc.jugador0Bot && jugadorVivo[0]){
                            idCarta = ia.SeleccionAlgoritmoIA(gc.manoDelJugador, gc.Descartes);
                            cartasRonda[0] = gc.manoDelJugador[idCarta];
                            gc.manoDelJugador.RemoveAt(idCarta);
                            gc.cartaUtilizada[0].sprite = Resources.Load<Sprite>(cartasRonda[0].nombreImagen);
                            gc.leyenda.text =  gc.nombreDinosaurios[0] + " ha utilizado la carta " + cartasRonda[0].nombreImagen.Substring(7);
                        }else{
                            cartasRonda[0] = gc.PERDER;
                        }
                        gc.OrdenarCartasMano();
                        gc.scrollbar.gameObject.SetActive(true);
                        if(jugadorVivo[1]){
                            idCarta = ia.SeleccionAlgoritmoIA(gc.manoRival1, gc.Descartes);
                            cartasRonda[1] = gc.manoRival1[idCarta];
                            gc.manoRival1.RemoveAt(idCarta);
                            // cartasRonda[1] = gc.PRUEBA2;
                            // cartasRonda[1].SetJugador(1);
                            Sprite imagen = Resources.Load<Sprite>(cartasRonda[1].nombreImagen);
                            gc.cartaUtilizada[1].sprite = imagen;
                            gc.leyenda.text += "\n" + gc.nombreDinosaurios[1] + " ha utilizado la carta " + cartasRonda[1].nombreImagen.Substring(7);
                        }else
                            cartasRonda[1] = gc.PERDER;

                        if(jugadorVivo[2]){                  
                            idCarta = ia.SeleccionAlgoritmoIA(gc.manoRival2, gc.Descartes);
                            cartasRonda[2] = gc.manoRival2[idCarta];
                            gc.manoRival2.RemoveAt(idCarta);
                            // cartasRonda[2] = gc.PRUEBA2;
                            // cartasRonda[2].SetJugador(2);
                            Sprite imagen = Resources.Load<Sprite>(cartasRonda[2].nombreImagen);
                            gc.cartaUtilizada[2].sprite = imagen;
                            gc.leyenda.text += "\n" + gc.nombreDinosaurios[2] + " ha utilizado la carta " + cartasRonda[2].nombreImagen.Substring(7);
                        }else
                            cartasRonda[2] = gc.PERDER;

                        if(jugadorVivo[3]){
                            idCarta = ia.SeleccionAlgoritmoIA(gc.manoRival3, gc.Descartes);
                            cartasRonda[3] = gc.manoRival3[idCarta];
                            gc.manoRival3.RemoveAt(idCarta);
                            // cartasRonda[3] = gc.PRUEBA2;
                            // cartasRonda[3].SetJugador(3);
                            Sprite imagen = Resources.Load<Sprite>(cartasRonda[3].nombreImagen);
                            gc.cartaUtilizada[3].sprite = imagen;
                            gc.leyenda.text += "\n" + gc.nombreDinosaurios[3] + " ha utilizado la carta " + cartasRonda[3].nombreImagen.Substring(7);
                        }else
                            cartasRonda[3] = gc.PERDER;

                        for(int i = 0; i < gc.jugador.Length; i++){
                            if(jugadorVivo[i])
                                bonusAtaque[i] += gc.jugador[i].colorBonus[gc.mazoDesastres[0].color];
                        }

                        for (int i = 0; i < efecto.efectoJugado.Length; i++) {
                            efecto.efectoJugado[i] = false;
                        }
                    }else{
                        JugadaEmpate();
                        return;
                    }
                }else{
                    if(efecto.efecto0Activo && gc.idManoCartaSeleccionadaEfectos.Count > 0){
                        gc.manoDelJugador.RemoveAt(gc.idManoCartaSeleccionadaEfectos[0]);
                        gc.OrdenarCartasMano();
                    }
                }
                ActualizarAtaques();
                efecto.Efectos();

                if(!esperandoAccion && efecto.efecto4Acabado){
                    hechizo.hechizos = true;
                    if(gc.TieneHechizosScore(gc.manoDelJugador)){
                        if(!gc.jugador0Bot){
                            gc.textoGuia.text = "Escoge un hechizo para jugar";
                            hechizo.jugador0PuedeHechizos = true;
                            return;
                        }else
                            StartCoroutine(hechizo.Hechizos(0));
                    }else{
                        hechizo.jugador0PuedeHechizos = false;
                        StartCoroutine(hechizo.Hechizos(1));
                    }
                }
            }
        }
    }

    // public IEnumerator Hechizos(int id){
    //     Debug.Log("Hechizos, jugador: " + id);
    //     gc.textoGuia.text = "Esperando al resto de jugadores";
        
    //     yield return new WaitForSeconds(tiempoEspera);
    //     tiempoEspera = 0;
    //     bool hehcizoJugado = false;

    //     List<Carta> mano = gc.BuscarMano(id);
    //     if(jugadorVivo[id] && gc.TieneHechizosScore(mano)){
    //         int booster = 0, sapper = 0, inversion = 0;
    //         int[] posicionId;
    //         int[] posicionAtaque;

    //         ObtenerPosiciones(out posicionId, out posicionAtaque);

    //         for(int i = 0; i < mano.Count; i++){
    //             switch(mano[i].nombreImagen){      
    //                 case "ObjetoScoreBooster":
    //                 booster++;
    //                 break;

    //                 case "ObjetoScoreSapper":
    //                 sapper++;
    //                 break;

    //                 case "ObjetoScoreInversion":
    //                 inversion++;
    //                 break;
    //             }
    //         }

    //         bool intercambio = efecto.efecto5Activado != scoreInversion;
    //         if(inversion > 0 && ((posicionId[0] == id && !efecto.efecto5Activado) || posicionId[posicionId.Length-1] == id && efecto.efecto5Activado)){
    //             yield return new WaitForSeconds(2);
    //             ScoreInversion(id, mano);
    //             hehcizoJugado = true;
    //         }else if((booster > 0 || sapper > 0) && ((posicionId[posicionId.Length-1] != id && !intercambio) || (posicionId[0] != id && intercambio))){
    //             int diferenciaNoPerder = -1;
    //             int diferenciaGanar = -1;
    //             int noPerder = -1;
    //             int ganar = -1;

    //             int posicion = Array.IndexOf(posicionId, id);
    //             int distancia = 3 + 2 * (efecto.efecto6Activado ? 1 : 0);

    //             if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[1]){
    //                 diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[1];
    //                 noPerder = 1;
    //             }else if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[2]){
    //                 diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[2];
    //                 noPerder = 2;
    //             }else if(posicion == 0 && !intercambio && posicionAtaque[0] != posicionAtaque[posicionAtaque.Length-1]){
    //                 diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[posicionAtaque.Length-1];
    //                 noPerder = 3;
    //             }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[2]){
    //                 diferenciaNoPerder = posicionAtaque[2] - posicionAtaque[posicionAtaque.Length-1];
    //                 noPerder = 2;
    //             }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[1]){
    //                 diferenciaNoPerder = posicionAtaque[1] - posicionAtaque[posicionAtaque.Length-1];
    //                 noPerder = 1;
    //             }else if(posicion == posicionAtaque.Length-1 && intercambio && posicionAtaque[posicionAtaque.Length-1] != posicionAtaque[0]){
    //                 diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[posicionAtaque.Length-1];
    //                 noPerder = 0;
    //             }

    //             if(posicion != posicionAtaque.Length && !intercambio){
    //                 diferenciaGanar = posicionAtaque[posicion] - posicionAtaque[posicionAtaque.Length-1];
    //                 ganar = posicionAtaque.Length;
    //             }else if(posicion != 0 && intercambio){
    //                 diferenciaGanar = posicionAtaque[0] - posicionAtaque[posicion];
    //                 ganar = 0;
    //             }

    //             if((diferenciaNoPerder > -1 && diferenciaNoPerder < distancia) || (diferenciaGanar > -1 && diferenciaGanar < distancia)){
    //                 yield return new WaitForSeconds(2);

    //                 if(booster > 0){
    //                     if(!intercambio)
    //                         ScoreBooster(id, id, mano);
    //                     else if(posicion == 3)
    //                         ScoreBooster(id, noPerder, mano);
    //                     else
    //                         ScoreBooster(id, ganar, mano);
    //                     hehcizoJugado = true;
    //                 }else if(sapper > 0){
    //                     if(intercambio)
    //                         ScoreSapper(id, id, mano);
    //                     else if(posicion == 3)
    //                         ScoreSapper(id, noPerder, mano);
    //                     else if(diferenciaGanar > -1)
    //                         ScoreSapper(id, ganar, mano);
    //                     hehcizoJugado = true;
    //                 }
    //             }
    //         }
    //     }

    //     if(hehcizoJugado && gc.TieneHechizosScore(gc.manoDelJugador)){
    //         if(!gc.jugador0Bot){
    //             gc.textoGuia.text = "Escoge un hechizo para jugar";
    //             jugador0PuedeHechizos = true;
    //         }else
    //             StartCoroutine(Hechizos(0));
    //         yield break;
    //     }else if(hehcizoJugado){
    //         jugador0PuedeHechizos = false;
    //         StartCoroutine(Hechizos(1));
    //         yield break;
    //     }

    //     for(int i = id; i < 3; i++){
    //         if(jugadorVivo[i+1]){
    //             StartCoroutine(Hechizos(i+1));
    //             yield break;
    //         }
    //     }

    //     hechizos = false;
    //     Invoke("ObtenerGanadores", 2);
    //     StartCoroutine(AnimarCartasGanadoras());
    // } 


    // public void HechizosJugador(GameObject carta, int id){
    //     if(gc.manoDelJugador[id].esHechizo){
    //         switch(gc.manoDelJugador[id].nombreImagen){
    //             case "ObjetoScoreBooster":
    //                 //ScoreBooster(0, gc.manoDelJugador);
    //                 //gc.OrdenarCartasMano();
    //                 gc.ImagenCartasMano[id].gameObject.SetActive(false);
    //                 gc.textoGuia.text = "Suma 2 de ataque a un jugador";
    //                 jugador0Booster = true;
    //                 break;

    //             case "ObjetoScoreSapper":
    //                // gc.manoDelJugador.RemoveAt(id);
    //                 //gc.OrdenarCartasMano();
    //                 gc.ImagenCartasMano[id].gameObject.SetActive(false);
    //                 gc.textoGuia.text = "Quita 2 de ataque a un jugador";
    //                 jugador0Sapper = true;
    //                 break;

    //             case "ObjetoScoreInversion":
    //                 ScoreInversion(0, gc.manoDelJugador);
    //                 gc.OrdenarCartasMano();
    //                 break;

    //             default:
    //                 break;
    //         }
    //     }
        
    //     if(!gc.TieneHechizosScore(gc.manoDelJugador)){
    //         StartCoroutine(Hechizos(1));
    //         gc.textoGuia.text = "Esperando acciones de los jugadores";
    //     }else{
    //         gc.textoGuia.text = "Escoge un hechizo para jugar";
    //         jugadaEnCurso = false;
    //     }
    // }


    // public void ScoreBooster(int id, int idObjetivo, List<Carta> mano){
    //     Debug.Log("Booster");
    //     jugadaEnCurso = false;
    //     if(id == 0 && !gc.jugador0Bot){
    //         idObjetivo = idObjetivo - 5;
    //         jugador0Booster = false;
    //     }
    //     bonusAtaque[idObjetivo] += 2 + 2 * (efecto.efecto6Activado ? 1 : 0);
    //     ActualizarAtaques();
    //     gc.iconosHechizo[idObjetivo*3].gameObject.SetActive(true);
    //     int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreBooster", mano);
    //     mano.RemoveAt(idCartaMano);
    //     gc.GuardarMano(id, mano);
    //     if(id == 0)
    //         gc.OrdenarCartasMano();
    //     gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + "usa Booster sobre " + gc.nombreDinosaurios[idObjetivo];
    //     gc.AumentarLeyenda();
    // }



    // public void ScoreSapper(int id, int idObjetivo, List<Carta> mano){
    //     Debug.Log("Sapper");
    //     jugadaEnCurso = false;
    //     if(id == 0 && !gc.jugador0Bot){
    //         idObjetivo = idObjetivo - 5;
    //         jugador0Sapper = false;
    //     }
    //     bonusAtaque[idObjetivo] -= 2 + 2 * (efecto.efecto6Activado ? 1 : 0);
    //     ActualizarAtaques();
    //     gc.iconosHechizo[idObjetivo*3+2].gameObject.SetActive(true);
    //     int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreSapper", mano);
    //     mano.RemoveAt(idCartaMano);
    //     gc.GuardarMano(id, mano);
    //     if(id == 0)
    //         gc.OrdenarCartasMano();
    //     gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + "usa Sapper sobre " + gc.nombreDinosaurios[idObjetivo];
    //     gc.AumentarLeyenda();
    // }


    // public void ScoreInversion(int id, List<Carta> mano){
    //     Debug.Log("Inversion");
    //     jugadaEnCurso = false;
    //     scoreInversion = !scoreInversion;
    //     ActualizarAtaques();
    //     gc.iconosHechizo[id*3+1].gameObject.SetActive(true);
    //     int idCartaMano = gc.ObtenerIdCartaManoPorNombre("ObjetoScoreInversion", mano);
    //     mano.RemoveAt(idCartaMano);
    //     gc.GuardarMano(id, mano);
    // }



    // void ObtenerPosiciones(out int[] posiciones, out int[] enterosOrdenados){
    //     // Crear una lista para almacenar los enteros y sus posiciones originales
    //     List<int[]> listaEnteros = new List<int[]>();

    //     // Convertir los elementos a enteros y almacenar en la lista
    //     for (int i = 0; i < textoAtaque.Length; i++){
    //         if (jugadorVivo[i]){
    //             int.TryParse(textoAtaque[i].text, out int ataque);
    //             listaEnteros.Add(new int[] { ataque, i });
    //         }
    //     }

    //     // Ordenar la lista por los enteros
    //     listaEnteros.Sort((x, y) => x[0].CompareTo(y[0]));

    //     // Crear dos arrays para almacenar las posiciones y los enteros ordenados
    //     posiciones = new int[listaEnteros.Count];
    //     enterosOrdenados = new int[listaEnteros.Count];

    //     // Llenar los arrays con las posiciones y los enteros ordenados
    //     for (int i = 0; i < listaEnteros.Count; i++){
    //         posiciones[i] = listaEnteros[i][1];
    //         enterosOrdenados[i] = listaEnteros[i][0];
    //     }
    // }



    public void JugadaEmpate(){
        for(int i = 0; i < 4; i++){
            if(idPerdedores[i]){
                if(i == 0 && !gc.jugador0Bot){
                    cartasRonda[0] = gc.manoDelJugador[gc.idManoCartaSeleccionada];
                    gc.manoDelJugador.RemoveAt(gc.idManoCartaSeleccionada);
                    gc.OrdenarCartasMano();
                }else{
                    List<Carta> mano = gc.BuscarMano(i);
                    int idCarta = ia.SeleccionAlgoritmoIAEmpate(mano);
                    if(mano[idCarta].esHechizo){
                        cartasRonda[i] = gc.PERDER;
                        gc.cartaUtilizada[i].sprite = Resources.Load<Sprite>("FondoPerder");
                    }else{
                        cartasRonda[i] = mano[idCarta];
                        gc.cartaUtilizada[i].sprite = Resources.Load<Sprite>(cartasRonda[i].nombreImagen);
                    }
                    gc.cartaUtilizada[i].gameObject.SetActive(true);

                    mano.RemoveAt(idCarta);
                    gc.GuardarMano(i, mano);
                }
            }else{
                cartasRonda[i] = gc.PERDER;
            }
        }
        ActualizarAtaques();

        ObtenerGanadores();
        tiempoEspera += 1;
        StartCoroutine(AnimarCartasGanadoras());
    }


    // //Función para añadir los efectos de las cartas
    // public void Efectos(){
    //     Debug.Log("Efectos");
    //     Carta[] cartasArray = cartasRonda.OrderBy(carta => carta.ataque).ToArray();
    //     List<Carta> cartasOrdenadas = new List<Carta>(cartasArray);
    //     int  jugado = 0, count = 0;
    //     bool motosierra = false;
    //     efectos = true;

    //     for (int i = 0; i < efectoJugado.Length; i++) {
    //         if(efectoJugado[i]) 
    //             jugado++;
    //     }

    //     for(int i = 0; i < cartasRonda.Length; i++){
    //         if(cartasRonda[i].nombreImagen == "Objeto9Motosierra"){
    //             jugado = 4;
    //             motosierra = true;
    //         }
    //     }

    //     if(!motosierra){
    //         for (int i = 0; i < efectoJugado.Length; i++) {
    //             if(cartasRonda[i].nombreImagen == "Objeto5Trampa") 
    //                 efecto5Activado = true;
    //         }
    //     }

    //     for(int i = jugado; i < cartasOrdenadas.Count; i++){
    //         efectoJugado[i] = true;
    //         Debug.Log("Efectos " + i + ", jugador " + cartasOrdenadas[i].idJugador + " " + cartasOrdenadas[i].nombreImagen + " " + cartasOrdenadas[i].efecto);
    //         List<Carta> mano = null;
    //         int id = cartasOrdenadas[i].idJugador;
    //         mano = gc.BuscarMano(id);
            
    //         switch(cartasOrdenadas[i].nombreImagen){
    //         case "Objeto0":
    //             count = 0;
    //             //Comprobamos que tiene alguna carta de ataque más
    //             foreach (Carta carta in mano){
    //                 if (!carta.esHechizo){
    //                     count++;
    //                 }
    //             }

    //             if (count > 0){
    //                 if (id == 0 && !gc.jugador0Bot){
    //                     jugadaEnCurso = false;
    //                     esperandoAccion = true;
    //                     idEfectos = 0;
    //                     gc.textoGuia.text = "Descarte una carta para sumar su ataque";
    //                     return;
    //                 }else{
    //                 // Resto de las acciones para otros jugadores
    //                     Efecto0y3IA(id, mano, 0);
    //                 }
    //             }
    //                 break;

    //         case "Objeto1Dinno":
    //             efectosFinRonda = true;
    //             idEfectos = 1;
    //             idJugador = id;
    //             idJugadorEfecto.Add(idJugador);
    //             idEfecto.Add(1);
    //             iconoEscogido = false;
    //             //Debug.Log("Objeto1Dinno");
    //             break;

    //         case "Objeto2Serpiente":
    //             if(id == 0 && !gc.jugador0Bot){
    //                 if(cartasRonda[1].efecto || cartasRonda[3].efecto){
    //                     esperandoAccion = true;
    //                     idEfectos = 2;
    //                     idJugador = id;
    //                     efecto2Activo = true;
    //                     gc.textoGuia.text = "Intercambie su carta la de su lado, con efecto";
    //                     return;
    //                 }   
    //             }else{
    //                 Efecto2IA(id);
    //             }
    //             break;
    //         case "Objeto3Batidora":
    //             count = 0;
    //             //Comprobamos que tiene alguna carta de ataque más
    //             foreach (Carta carta in mano){
    //                 if (carta.efecto){
    //                     //Debug.Log(carta.efecto + " " + carta.ataque);
    //                     count = count +1;
    //                 }
    //             }

    //             //Debug.Log(id + " " + !gc.jugador0Bot + " " + count);
    //             if (count > 0){
    //                 if (id == 0 && !gc.jugador0Bot){
    //                     jugadaEnCurso = false;
    //                     esperandoAccion = true;
    //                     idEfectos = 3;
    //                     gc.textoGuia.text = "Descarte una carta con texto";
    //                     return;
    //                 }else{
    //                     Debug.Log(count);
    //                     Efecto0y3IA(id, mano, 3);
    //                 }
    //             }
    //             break;
            
    //         case "Objeto4Fuego":
    //                 if(id == 0 && !gc.jugador0Bot){
    //                     idEfectos = 4;
    //                     efecto4Acabado = false;
    //                     gc.textoGuia.text = "Descarte hasta 3 cartas";
    //                     return;
    //                 }else{
    //                     int descartes = 3;
    //                     for (int j = mano.Count - 1; j >= 0; j--){
    //                         if (descartes > 0 && mano[j].ataque < 6){
    //                             mano.RemoveAt(j); // Elimina la carta de la mano
    //                             descartes--;
    //                         }
    //                     }
    //                     gc.GuardarMano(id, mano);
    //                 }
    //             break;

    //         case "Objeto6Estrella":
    //             efecto6Activado = true;
    //             break;

    //         case "Objeto7Prismaticos":
    //             PrepararEfecto7(id, mano);
                
    //             if(id == 0 && !gc.jugador0Bot){
    //                 gc.textoGuia.text = "Obtenga una carta";
    //                 return;
    //             }
    //             break;

    //         case "Objeto8Planta":
    //             if(id == 0 && !gc.jugador0Bot){
    //                 idEfectos = 8;
    //                 esperandoAccion = true;
    //                 gc.textoGuia.text = "Intercambie 2 cartas";
    //                 return;
    //             }else{
    //                 Efecto8IA(id);
    //             }
    //             break;

    //         default:
    //                 break;
    //         }
    //     }
    //     jugadaEnCurso = false;
    // }

//    public void Efecto0(){
//         jugadaEnCurso = false;
//         efecto0Activo = true;
//         gc.cartaUtilizadaEfectos[0].gameObject.SetActive(true);
//         Sprite imagen = Resources.Load<Sprite>(gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].nombreImagen);
//         gc.cartaUtilizadaEfectos[0].sprite = imagen;
//         esperandoAccion = false;
//         bonusAtaque[0] += gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].ataque;
//         ActualizarAtaques();
//         gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha descartado la carta " + gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].nombreImagen.Substring(7)
//          + " para sumarse " + gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].ataque + " de ataque";
//         gc.AumentarLeyenda();
//     }

//     public void Efecto0y3IA(int id, List<Carta> mano, int efecto){
//         int idCarta;
        
//         if(!efecto5Activado){
//             Debug.Log(gc.manoRival1[0].nombreImagen + " " + gc.manoRival1[0].efecto);
//             Debug.Log(gc.manoRival1[1].nombreImagen + " " + gc.manoRival1[1].efecto);
//             Debug.Log(gc.manoRival1[2].nombreImagen + " " + gc.manoRival1[2].efecto);
//             Debug.Log(gc.manoRival1[3].nombreImagen + " " + gc.manoRival1[3].efecto);
//             idCarta = ObtenerIdCartaEfecto0y3IA(id, mano, efecto);
//             Debug.Log("Carta: " + gc.manoRival1[idCarta].nombreImagen + ", tiene efecto: " + gc.manoRival1[idCarta].efecto);
//             StartCoroutine(AgregarCartaEfectos(id, mano, idCarta));
//         }
//     }


//     public int ObtenerIdCartaEfecto0y3IA(int id, List<Carta> mano, int efecto){
//         //Obtenemos el mejor y peor ataque
//         int ataqueMin = 100, idCarta = -1;
//         int ataque, ataqueMax = 0;
//         for(int j = 0; j < 4; j++){
//             if(j != id){
//                 int.TryParse(textoAtaque[j].text, out ataque);
//                 if(ataqueMin > ataque){
//                     ataqueMin = ataque;
//                 }
//                 if(ataqueMax < ataque){
//                     ataqueMax = ataque;
//                 }
//             }
//         }

//         if(efecto5Activado){
//             int minimo = 100;
//             for(int j = 0; j < mano.Count; j++){
//                 if ((efecto == 0 && minimo >= mano[j].ataque) || (efecto == 3 && mano[j].efecto && minimo >= mano[j].ataque + 3)){
//                     idCarta = j;
//                     minimo = mano[j].ataque;
//                 } 
//             }
//         }else{
//             //Buscamos si alguna carta con la que ganemos
//             for(int j = 0; j < mano.Count; j++){
//                 if ((efecto == 0 || (efecto == 3 && mano[j].efecto)) &&
//                 ((cartasRonda[id].ataque + mano[j].ataque + bonusAtaque[id]) >= ataqueMax)){
//                     idCarta = j;
//                     break;
//                 } 
//             }
//             //En caso de no haber encontrado una carta con la que ganemos, buscaremos una carta con la que no perdamos
//             if(idCarta == -1){
//                 for(int j = 0; j < mano.Count; j++){
//                     if((efecto == 0 || (efecto == 3 && mano[j].efecto)) && !mano[j].esHechizo &&
//                     (cartasRonda[id].ataque + mano[j].ataque + bonusAtaque[id]) > ataqueMin){
//                         idCarta = j;
//                         break;
//                     } 
//                 }
                
//                 if(idCarta == -1){
//                     idCarta = UnityEngine.Random.Range(0, mano.Count);
//                     while(mano[idCarta].esHechizo && (efecto == 0 || efecto == 3 && mano[idCarta].efecto)){  
//                         idCarta = UnityEngine.Random.Range(0, mano.Count);
//                     }
//                 }
//             }
//         }
//         return idCarta;
//     }

//     public void Efecto1ObtenerCarta(int id){
//         Carta carta = gc.manoEfecto[id-5];
//         carta.SetJugador(id);
//         gc.manoEfecto.RemoveAt(id-5);
//         gc.GuardarMano(carta.idJugador, gc.manoEfecto);

//         if(gc.idManoCartaSeleccionada <= gc.manoDelJugador.Count){
//             gc.manoDelJugador.Insert(gc.idManoCartaSeleccionada, carta);
//         }else{
//             gc.manoDelJugador.Add(carta);
//         }

//         Sprite imagen = Resources.Load<Sprite>(carta.nombreImagen);
//         gc.ImagenCartasMano[gc.idManoCartaSeleccionada].sprite = imagen;
//         gc.ImagenCartasMano[gc.idManoCartaSeleccionada].gameObject.SetActive(true);
//         efectosFinRonda = false;

//         gc.IconosSetActive(false);
//         gc.CartaUtilizadaSetActive(false);

//         idJugadorEfecto.RemoveAt(idJugadorEfecto.Count-1);
//         idEfecto.RemoveAt(idEfecto.Count-1);
//         gc.textoGuia.text = "";
//         gc.efecto1JugadorSeleccionado = -1;
//         gc.AcabarRondaEfectos();
//         gc.SiguienteRonda();
//     }



//     public void Efecto1IA(){
//         int idCarta = -1;
//         int jugadorId;
//         do{ 
//             jugadorId= UnityEngine.Random.Range(0, 2);
//             if(jugadorId >= idJugadorEfecto[0]){
//                 jugadorId++;
//             }
//         }while(!jugadorVivo[jugadorId]);

//         List<Carta> mano = gc.BuscarMano(idJugador);
//         gc.manoEfecto = gc.BuscarMano(jugadorId);

//         if(PlayerPrefs.GetInt("DificultadIA") == 0)
//             idCarta = UnityEngine.Random.Range(0, gc.manoEfecto.Count);
//         else if(PlayerPrefs.GetInt("DificultadIA") == 1)
//             idCarta = ia.MonteCarloEfecto1y7(gc.manoEfecto, mano);
        

//         Carta carta = gc.manoEfecto[idCarta];
//         carta.SetJugador(jugadorId);
//         mano.Add(carta);
//         gc.manoEfecto.RemoveAt(idCarta);

//         gc.GuardarMano(jugadorId, gc.manoEfecto);
//         gc.GuardarMano(idJugador, mano);
//         efectosFinRonda = false;
//     }

//     public void Efecto2(int idCarta){
//         Carta carta = cartasRonda[0];

//         if(cartasRonda[idCarta-5].efecto){
//             Debug.Log("Efecto2");
//             jugadaEnCurso = false;
//             efecto2Activo = false;
//             cartasRonda[0] = cartasRonda[idCarta-5];
//             cartasRonda[idCarta-5] = carta;
//             cartasRonda[idCarta-5].SetJugador(idCarta-5);

//             Sprite imagen = gc.cartaUtilizada[0].sprite;
//             gc.cartaUtilizada[0].sprite = gc.cartaUtilizada[idCarta-5].sprite;
//             gc.cartaUtilizada[idCarta-5].sprite = imagen;
//             cartasRonda[0].SetJugador(0);
//             //gestorJugada.efectoJugado[] = true; 
           
//             esperandoAccion = false;
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[cartasRonda[idCarta-5].idJugador];
//             gc.AumentarLeyenda();
//             EmpezarJugada();
//         }
//     }


//     public void Efecto2IA(int id){
//         int robo1 = id - 1;
//         int robo2 = id + 1;
//         if(id == 3)
//             robo2 = 0;
//         else if(id == 0)
//             robo1 = 3;

//         if(cartasRonda[robo1].efecto || cartasRonda[robo2].efecto){
//             (int ataque, int idDato, bool efecto)[]datos = {
//                 (cartasRonda[robo1].ataque, robo1, cartasRonda[robo1].efecto),
//                 (cartasRonda[robo2].ataque, robo2, cartasRonda[robo2].efecto),
//                 (cartasRonda[id].ataque, id, true)
//             };

//             var datosOrdenados = datos.Where(d => d.efecto).OrderByDescending(d => d.ataque).ToArray();
//             int[] idOrdenado = datosOrdenados.Select(d => d.idDato).ToArray();

//             if(((idOrdenado[0] == robo1 || idOrdenado[0] == robo2) && !efecto5Activado) ||
//              ((idOrdenado[idOrdenado.Length-1] == robo1 || idOrdenado[idOrdenado.Length-1] == robo2) && !efecto5Activado)){
//                 posicion2 = (efecto5Activado) ? idOrdenado[0] : idOrdenado[idOrdenado.Length-1];
//                 posicion1 = id;
            
//                 tiempoEspera += 2;
//                 Invoke("CambiarSpriteCartasUtilizadas", 2.0f);

//                 gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion2];
//                 gc.AumentarLeyenda();
//             }
//         }
//     }

//     public void Efecto4(int id, Image imagenCarta){
//         if(gc.descartes > 0){
//             jugadaEnCurso = false;
//             Debug.Log("Efecto4");
//             imagenCarta.gameObject.SetActive(false);
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha descartado la carta " + gc.manoDelJugador[id].nombreImagen.Substring(7);
//             gc.AumentarLeyenda();
//             gc.manoDelJugador.RemoveAt(id);
//             gc.OrdenarCartasMano();
//             gc.descartes--;
//         }
//     }

//     public void PrepararEfecto7(int id, List<Carta> mano){
//         gc.RellenarMazo(2);
//         cartaEfecto7[0] = gc.Mazo[0];
//         cartaEfecto7[1] = gc.Mazo[1];
//         gc.Mazo.RemoveAt(0);
//         gc.Mazo.RemoveAt(0);
//         if(id == 0 && !gc.jugador0Bot){
//             jugadaEnCurso = false;
//             idEfectos = 7;
//             efecto7Activo = true;
//             esperandoAccion = true; 
//             gc.IconosSetActive(false);
//             gc.CartaUtilizadaSetActive(false);
//             gc.CartaUtilizadaEfectosSetActive(false);
//             TextoAtaqueSetActive(false);
            
//             gc.ImagenCartaEfecto7SetActive(true);

            
//             Sprite imagen = Resources.Load<Sprite>(cartaEfecto7[0].nombreImagen);
//             gc.imagenCartaEfecto7[0].sprite = imagen;
            
//             imagen = Resources.Load<Sprite>(cartaEfecto7[1].nombreImagen);
//             gc.imagenCartaEfecto7[1].sprite = imagen;
//         }else{
//             if(PlayerPrefs.GetInt("DificultadIA") == 0){
//                 if(!cartaEfecto7[1].esHechizo || cartaEfecto7[0].ataque > cartaEfecto7[1].ataque){
//                     mano.Add(cartaEfecto7[0]);
//                 }else
//                     mano.Add(cartaEfecto7[1]);
//             }else if(PlayerPrefs.GetInt("DificultadIA") == 1){
//                 List<Carta> manoEfecto = new List<Carta>{cartaEfecto7[0], cartaEfecto7[1]};
//                 mano.Add(cartaEfecto7[ia.MonteCarloEfecto1y7(manoEfecto, mano)]);
//             }

//             mano[mano.Count-1].SetJugador(id);
//             gc.GuardarMano(id, mano);
//         }
//     }

//     public void Efecto7(int id){
//         Carta carta;

//         if(id > 8){
//             if(id == 9)
//                 carta = cartaEfecto7[0];
//             else 
//                 carta = cartaEfecto7[1];

//             //gc.manoDelJugador.RemoveAt(gc.idManoCartaSeleccionada);
//             gc.manoDelJugador.Add(carta);

//             for(int i = 0; i < gc.ImagenCartasMano.Length; i++){
//                 if(i < gc.manoDelJugador.Count){
//                     Sprite imagen = Resources.Load<Sprite>(gc.manoDelJugador[i].nombreImagen);
//                     gc.ImagenCartasMano[i].sprite = imagen;
//                     gc.ImagenCartasMano[i].gameObject.SetActive(true);
//                 }
//             }

//             gc.ImagenCartaEfecto7SetActive(false);
//             gc.IconosSetActive(true);
//             gc.CartaUtilizadaSetActive(true);
//             gc.CartaUtilizadaEfectosSetActive(false);    //todo
//             ActualizarAtaques();

//             esperandoAccion = false;
//             gc.textoGuia.text = "Esperando al resto de jugadores";
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha escogido la carta " + gc.manoDelJugador[4].nombreImagen.Substring(7);
//             gc.AumentarLeyenda();
//         }
//     }


//     public void Efecto8(int id){
//         id = id - 5;
//         if(jugadorVivo[id]){
//             efecto8Jugadores.Add(id);
//             Debug.Log("Efecto 8");
            
//             if(efecto8Jugadores.Count == 2 && efecto8Jugadores[0] != efecto8Jugadores[1]){
//                 jugadaEnCurso = false;
//                 Carta carta = cartasRonda[efecto8Jugadores[0]];
//                 cartasRonda[efecto8Jugadores[0]] = cartasRonda[efecto8Jugadores[1]];
//                 cartasRonda[efecto8Jugadores[1]] = carta;
//                 cartasRonda[efecto8Jugadores[0]].SetJugador(efecto8Jugadores[0]);
//                 cartasRonda[efecto8Jugadores[1]].SetJugador(efecto8Jugadores[1]);

//                 Sprite imagen = Resources.Load<Sprite>(cartasRonda[efecto8Jugadores[0]].nombreImagen);
//                 gc.cartaUtilizada[efecto8Jugadores[0]].sprite = imagen;

//                 imagen = Resources.Load<Sprite>(cartasRonda[efecto8Jugadores[1]].nombreImagen);
//                 gc.cartaUtilizada[efecto8Jugadores[1]].sprite = imagen;

                
//                 esperandoAccion = false;
//                 if(efecto8Jugadores[0] == 0)
//                     gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[efecto8Jugadores[1]];
//                 else if(efecto8Jugadores[1] == 0)
//                     gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[efecto8Jugadores[0]];
//                 else
//                     gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado las cartas de " +
//                      gc.nombreDinosaurios[efecto8Jugadores[0]] + " con la de " + gc.nombreDinosaurios[efecto8Jugadores[1]];
//                 gc.AumentarLeyenda();
//                 efecto8Jugadores.Clear();
//                 ActualizarAtaques();
//                 EmpezarJugada();
//             }else if(efecto8Jugadores.Count == 2 && efecto8Jugadores[0] == efecto8Jugadores[1])
//                 efecto8Jugadores.RemoveAt(1);
//         }

//     }


//     public void Efecto8IA(int id){
//         int max = 0, maxId = -1;
//         int min = 20, minId = -1;
//         int j1, j2;
//         for(int j = 0; j < cartasRonda.Length; j++){
//             if(jugadorVivo[j] && cartasRonda[j].ataque > max){
//                 max = cartasRonda[j].ataque;
//                 maxId = j;
//             }
//             if(jugadorVivo[j] && cartasRonda[j].ataque < min){
//                 min = cartasRonda[j].ataque;
//                 minId = j;
//             }
//         }

        
//         if((cartasRonda[id].ataque == max && !efecto5Activado) || (cartasRonda[id].ataque == min && efecto5Activado)){
//             do{
//                 j1 = UnityEngine.Random.Range(0, 4);
//             }while(j1 != id && jugadorVivo[j1]);

//             if(jugadorVivo.Count(b => b) == 2)
//                 j2 = id;
//             else{
//                 do{
//                     j2 = UnityEngine.Random.Range(0, 4);
//                 }while(j1 != id && j1 != j2 && jugadorVivo[j2]);
//             }

//             posicion1 = j1;
//             posicion2 = j2;
            
//             tiempoEspera += 2;
//             Invoke("CambiarSpriteCartasUtilizadas", 2.0f);
//         }else{
//             if(!efecto5Activado)
//                 posicion2 = maxId;
//             else
//                 posicion2 = minId;
//             posicion1 = id;
//             tiempoEspera += 2;
//             Invoke("CambiarSpriteCartasUtilizadas", 2.0f);
//         }

//         if(posicion1 == id)
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[posicion1] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion2];
//         else if(posicion2 == id)
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[posicion2] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion1];
//         else
//             gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha intercambiado las cartas de " +
//                 gc.nombreDinosaurios[posicion1] + " con la de " + gc.nombreDinosaurios[posicion2];
//         gc.AumentarLeyenda();
//     }

//     public IEnumerator AgregarCartaEfectos(int id, List<Carta> mano, int idCarta){
//         yield return new WaitForSeconds(1);

//         gc.cartaUtilizadaEfectos[id].gameObject.SetActive(true);
//         Sprite imagen = Resources.Load<Sprite>(mano[idCarta].nombreImagen);
//         gc.cartaUtilizadaEfectos[id].sprite = imagen;
//         bonusAtaque[id] += mano[idCarta].ataque;
//         gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha descartado la carta " + mano[idCarta].nombreImagen.Substring(7) +
//          " para sumarse " + mano[idCarta].ataque + " de ataque";
//         gc.AumentarLeyenda();
//         mano.RemoveAt(idCarta);
//         gc.GuardarMano(id, mano);

//         ActualizarAtaques();
//     }

//     public void CambiarSpriteCartasUtilizadas(){
//         Debug.Log("CambiarSprite");
//         Carta carta = cartasRonda[posicion1];
//         cartasRonda[posicion1] = cartasRonda[posicion2];
//         cartasRonda[posicion2] = carta;

        
//         Sprite imagen = Resources.Load<Sprite>(cartasRonda[posicion1].nombreImagen);
//         gc.cartaUtilizada[posicion1].sprite = imagen;
        
//         imagen = Resources.Load<Sprite>(cartasRonda[posicion2].nombreImagen);
//         gc.cartaUtilizada[posicion2].sprite = imagen;
//         ActualizarAtaques();
//     }



    // Update is called once per frame
    void Update(){
    }
}


//FALTA POR HACER
//Fondo
 