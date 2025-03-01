using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI; 
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class JugadaEfectos : MonoBehaviour
{
    public GestorJugada gj;
    public GestorCartas gc;
    public IA ia;

    public bool efectos = false;    
    public bool efectosFinRonda = false;
    public int idEfectos = 0;

    public List<int> idEfecto = new List<int>();
    public bool[] efectoJugado = new bool[4];

    public bool efecto0Activo = false; 
    public bool efecto2Activo = false;
    public bool efecto4Acabado;
    public bool efecto5Activado = false;
    public bool efecto6Activado = false;
    public bool efecto7Activo = false;
    public List<int> efecto8Jugadores = new List<int>();

    public Carta[] cartaEfecto7 = new Carta[2];
    int posicion1;
    int posicion2;


     //Función para añadir los efectos de las cartas
    public void Efectos(){
        Debug.Log("Efectos");
        Carta[] cartasArray = gj.cartasRonda.OrderBy(carta => carta.ataque).ToArray();
        List<Carta> cartasOrdenadas = new List<Carta>(cartasArray);
        int  jugado = 0, count = 0;
        bool motosierra = false;
        efectos = true;

        for (int i = 0; i < efectoJugado.Length; i++) {
            if(efectoJugado[i]) 
                jugado++;
        }

        for(int i = 0; i < gj.cartasRonda.Length; i++){
            if(gj.cartasRonda[i].nombreImagen == "Objeto9Motosierra"){
                jugado = 4;
                motosierra = true;
            }
        }

        if(!motosierra){
            for (int i = 0; i < efectoJugado.Length; i++) {
                if(gj.cartasRonda[i].nombreImagen == "Objeto5Trampa") 
                    efecto5Activado = true;
            }
        }

        for(int i = jugado; i < cartasOrdenadas.Count; i++){
            efectoJugado[i] = true;
            Debug.Log("Efectos " + i + ", jugador " + cartasOrdenadas[i].idJugador + " " + cartasOrdenadas[i].nombreImagen + " " + cartasOrdenadas[i].efecto);
            List<Carta> mano = null;
            int id = cartasOrdenadas[i].idJugador;
            mano = gc.BuscarMano(id);
            
            switch(cartasOrdenadas[i].nombreImagen){
            case "Objeto0":
                count = 0;
                //Comprobamos que tiene alguna carta de ataque más
                foreach (Carta carta in mano){
                    if (!carta.esHechizo){
                        count++;
                    }
                }

                if (count > 0){
                    if (id == 0 && !gc.jugador0Bot){
                        gj.jugadaEnCurso = false;
                        gj.esperandoAccion = true;
                        idEfectos = 0;
                        gc.textoGuia.text = "Descarte una carta para sumar su ataque";
                        return;
                    }else{
                    // Resto de las acciones para otros jugadores
                        Efecto0y3IA(id, mano, 0);
                    }
                }
                    break;

            case "Objeto1Dinno":
                efectosFinRonda = true;
                idEfectos = 1;
                gj.idJugador = id;
                gj.idJugadorEfecto.Add(gj.idJugador);
                idEfecto.Add(1);
                gj.iconoEscogido = false;
                //Debug.Log("Objeto1Dinno");
                break;

            case "Objeto2Serpiente":
                if(id == 0 && !gc.jugador0Bot){
                    if(gj.cartasRonda[1].efecto || gj.cartasRonda[3].efecto){
                        gj.esperandoAccion = true;
                        idEfectos = 2;
                        gj.idJugador = id;
                        efecto2Activo = true;
                        gc.textoGuia.text = "Intercambie su carta la de su lado, con efecto";
                        return;
                    }   
                }else{
                    Efecto2IA(id);
                }
                break;
            case "Objeto3Batidora":
                count = 0;
                //Comprobamos que tiene alguna carta de ataque más
                foreach (Carta carta in mano){
                    if (carta.efecto)
                        count = count + 1;
                }

                //Debug.Log(id + " " + !gc.jugador0Bot + " " + count);
                if (count > 0){
                    if (id == 0 && !gc.jugador0Bot){
                        gj.jugadaEnCurso = false;
                        gj.esperandoAccion = true;
                        idEfectos = 3;
                        gc.textoGuia.text = "Descarte una carta con texto";
                        return;
                    }else{
                        Debug.Log(count);
                        Efecto0y3IA(id, mano, 3);
                    }
                }
                break;
            
            case "Objeto4Fuego":
                    if(id == 0 && !gc.jugador0Bot){
                        idEfectos = 4;
                        efecto4Acabado = false;
                        gc.textoGuia.text = "Descarte hasta 3 cartas";
                        return;
                    }else{
                        int descartes = 3;
                        for (int j = mano.Count - 1; j >= 0; j--){
                            if (descartes > 0 && mano[j].ataque < 6){
                                mano.RemoveAt(j); // Elimina la carta de la mano
                                descartes--;
                            }
                        }
                        gc.GuardarMano(id, mano);
                    }
                break;

            case "Objeto6Estrella":
                efecto6Activado = true;
                break;

            case "Objeto7Prismaticos":
                PrepararEfecto7(id, mano);
                
                if(id == 0 && !gc.jugador0Bot){
                    gc.textoGuia.text = "Obtenga una carta";
                    return;
                }
                break;

            case "Objeto8Planta":
                if(id == 0 && !gc.jugador0Bot){
                    idEfectos = 8;
                    gj.esperandoAccion = true;
                    gc.textoGuia.text = "Intercambie 2 cartas";
                    return;
                }else{
                    Efecto8IA(id);
                }
                break;

            default:
                    break;
            }
        }
        gj.jugadaEnCurso = false;
    }

    public void Efecto0(){
        gj.jugadaEnCurso = false;
        efecto0Activo = true;
        gc.cartaUtilizadaEfectos[0].gameObject.SetActive(true);
        Sprite imagen = Resources.Load<Sprite>(gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].nombreImagen);
        gc.cartaUtilizadaEfectos[0].sprite = imagen;
        gj.esperandoAccion = false;
        gj.bonusAtaque[0] += gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].ataque;
        gj.ActualizarAtaques();
        gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha descartado la carta " + gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].nombreImagen.Substring(7)
         + " para sumarse " + gc.manoDelJugador[gc.idManoCartaSeleccionadaEfectos[0]].ataque + " de ataque";
        gc.AumentarLeyenda();
    }

    public void Efecto0y3IA(int id, List<Carta> mano, int efecto){
        int idCarta;
        
        if(!efecto5Activado){
            Debug.Log(mano[0].nombreImagen + " " + mano[0].efecto);
            Debug.Log(mano[1].nombreImagen + " " + mano[1].efecto);
            Debug.Log(mano[2].nombreImagen + " " + mano[2].efecto);
            Debug.Log(mano[3].nombreImagen + " " + mano[3].efecto);
            idCarta = ObtenerIdCartaEfecto0y3IA(id, mano, efecto);
            Debug.Log("Carta: " + gc.manoRival1[idCarta].nombreImagen + ", tiene efecto: " + gc.manoRival1[idCarta].efecto);
            StartCoroutine(AgregarCartaEfectos(id, mano, idCarta));
        }
    }


    public int ObtenerIdCartaEfecto0y3IA(int id, List<Carta> mano, int efecto){
        //Obtenemos el mejor y peor ataque
        int ataqueMin = 100, idCarta = -1;
        int ataque, ataqueMax = 0;
        for(int j = 0; j < 4; j++){
            if(j != id){
                int.TryParse(gj.textoAtaque[j].text, out ataque);
                if(ataqueMin > ataque){
                    ataqueMin = ataque;
                }
                if(ataqueMax < ataque){
                    ataqueMax = ataque;
                }
            }
        }

        //Buscamos si alguna carta con la que ganemos
        for(int j = 0; j < mano.Count; j++){
            if ((efecto == 0 || (efecto == 3 && mano[j].efecto)) && !mano[j].esHechizo &&
            ((gj.cartasRonda[id].ataque + mano[j].ataque + gj.bonusAtaque[id]) >= ataqueMax)){
                idCarta = j;
                break;
            } 
        }
        //En caso de no haber encontrado una carta con la que ganemos, buscaremos una carta con la que no perdamos
        if(idCarta == -1){
            for(int j = 0; j < mano.Count; j++){
                if((efecto == 0 || (efecto == 3 && mano[j].efecto)) && !mano[j].esHechizo &&
                (gj.cartasRonda[id].ataque + mano[j].ataque + gj.bonusAtaque[id]) > ataqueMin){
                    idCarta = j;
                    break;
                } 
            }
            
            if(idCarta == -1){
                idCarta = UnityEngine.Random.Range(0, mano.Count);
                while(mano[idCarta].esHechizo && (efecto == 0 || efecto == 3 && mano[idCarta].efecto)){  
                    idCarta = UnityEngine.Random.Range(0, mano.Count);
                }
            }
        }
        
        return idCarta;
    }

    public void Efecto1ObtenerCarta(int id){
        Carta carta = gc.manoEfecto[id-5];
        carta.SetJugador(id);
        gc.manoEfecto.RemoveAt(id-5);
        gc.GuardarMano(carta.idJugador, gc.manoEfecto);

        if(gc.idManoCartaSeleccionada <= gc.manoDelJugador.Count){
            gc.manoDelJugador.Insert(gc.idManoCartaSeleccionada, carta);
        }else{
            gc.manoDelJugador.Add(carta);
        }

        Sprite imagen = Resources.Load<Sprite>(carta.nombreImagen);
        gc.ImagenCartasMano[gc.idManoCartaSeleccionada].sprite = imagen;
        gc.ImagenCartasMano[gc.idManoCartaSeleccionada].gameObject.SetActive(true);
        efectosFinRonda = false;

        gc.IconosSetActive(false);
        gc.CartaUtilizadaSetActive(false);

        gj.idJugadorEfecto.RemoveAt(gj.idJugadorEfecto.Count-1);
        idEfecto.RemoveAt(idEfecto.Count-1);
        gc.textoGuia.text = "";
        gc.efecto1JugadorSeleccionado = -1;
        gc.AcabarRondaEfectos();
        gc.SiguienteRonda();
    }



    public void Efecto1IA(){
        int idCarta = -1;
        int jugadorId;
        do{ 
            jugadorId= UnityEngine.Random.Range(0, 2);
            if(jugadorId >= gj.idJugadorEfecto[0]){
                jugadorId++;
            }
        }while(!gj.jugadorVivo[jugadorId]);

        List<Carta> mano = gc.BuscarMano(gj.idJugador);
        gc.manoEfecto = gc.BuscarMano(jugadorId);

        if(PlayerPrefs.GetInt("DificultadIA") == 0)
            idCarta = UnityEngine.Random.Range(0, gc.manoEfecto.Count);
        else if(PlayerPrefs.GetInt("DificultadIA") == 1)
            idCarta = ia.MonteCarloEfecto1y7(gc.manoEfecto, mano);
        

        Carta carta = gc.manoEfecto[idCarta];
        carta.SetJugador(jugadorId);
        mano.Add(carta);
        gc.manoEfecto.RemoveAt(idCarta);

        gc.GuardarMano(jugadorId, gc.manoEfecto);
        gc.GuardarMano(gj.idJugador, mano);
        efectosFinRonda = false;
    }

    public void Efecto2(int idCarta){
        Carta carta = gj.cartasRonda[0];

        if(gj.cartasRonda[idCarta-5].efecto){
            Debug.Log("Efecto2");
            gj.jugadaEnCurso = false;
            efecto2Activo = false;
            gj.cartasRonda[0] = gj.cartasRonda[idCarta-5];
            gj.cartasRonda[idCarta-5] = carta;
            gj.cartasRonda[idCarta-5].SetJugador(idCarta-5);

            Sprite imagen = gc.cartaUtilizada[0].sprite;
            gc.cartaUtilizada[0].sprite = gc.cartaUtilizada[idCarta-5].sprite;
            gc.cartaUtilizada[idCarta-5].sprite = imagen;
            gj.cartasRonda[0].SetJugador(0);
            //gestorJugada.efectoJugado[] = true; 
           
            gj.esperandoAccion = false;
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[gj.cartasRonda[idCarta-5].idJugador];
            gc.AumentarLeyenda();
            gj.EmpezarJugada();
        }
    }


    public void Efecto2IA(int id){
        int robo1 = id - 1;
        int robo2 = id + 1;
        if(id == 3)
            robo2 = 0;
        else if(id == 0)
            robo1 = 3;

        if(gj.cartasRonda[robo1].efecto || gj.cartasRonda[robo2].efecto){
            (int ataque, int idDato, bool efecto)[]datos = {
                (gj.cartasRonda[robo1].ataque, robo1, gj.cartasRonda[robo1].efecto),
                (gj.cartasRonda[robo2].ataque, robo2, gj.cartasRonda[robo2].efecto),
                (gj.cartasRonda[id].ataque, id, true)
            };

            var datosOrdenados = datos.Where(d => d.efecto).OrderByDescending(d => d.ataque).ToArray();
            int[] idOrdenado = datosOrdenados.Select(d => d.idDato).ToArray();

            if(((idOrdenado[0] == robo1 || idOrdenado[0] == robo2) && !efecto5Activado) ||
             ((idOrdenado[idOrdenado.Length-1] == robo1 || idOrdenado[idOrdenado.Length-1] == robo2) && efecto5Activado)){
                posicion2 = (!efecto5Activado) ? idOrdenado[0] : idOrdenado[idOrdenado.Length-1];
                posicion1 = id;
            
                gj.tiempoEspera += 2;
                Invoke("CambiarSpriteCartasUtilizadas", 2.0f);

                gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion2];
                gc.AumentarLeyenda();
            }
        }
    }

    public void Efecto4(int id, Image imagenCarta){
        if(gc.descartes > 0){
            gj.jugadaEnCurso = false;
            Debug.Log("Efecto4");
            imagenCarta.gameObject.SetActive(false);
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha descartado la carta " + gc.manoDelJugador[id].nombreImagen.Substring(7);
            gc.AumentarLeyenda();
            gc.manoDelJugador.RemoveAt(id);
            gc.OrdenarCartasMano();
            gc.descartes--;
        }
    }

    public void PrepararEfecto7(int id, List<Carta> mano){
        gc.RellenarMazo(2);
        cartaEfecto7[0] = gc.Mazo[0];
        cartaEfecto7[1] = gc.Mazo[1];
        gc.Mazo.RemoveAt(0);
        gc.Mazo.RemoveAt(0);
        if(id == 0 && !gc.jugador0Bot){
            gj.jugadaEnCurso = false;
            idEfectos = 7;
            efecto7Activo = true;
            gj.esperandoAccion = true; 
            gc.IconosSetActive(false);
            gc.CartaUtilizadaSetActive(false);
            gc.CartaUtilizadaEfectosSetActive(false);
            gj.TextoAtaqueSetActive(false);
            
            gc.ImagenCartaEfecto7SetActive(true);

            
            Sprite imagen = Resources.Load<Sprite>(cartaEfecto7[0].nombreImagen);
            gc.imagenCartaEfecto7[0].sprite = imagen;
            
            imagen = Resources.Load<Sprite>(cartaEfecto7[1].nombreImagen);
            gc.imagenCartaEfecto7[1].sprite = imagen;
        }else{
            if(PlayerPrefs.GetInt("DificultadIA") == 0){
                if(!cartaEfecto7[1].esHechizo || cartaEfecto7[0].ataque > cartaEfecto7[1].ataque){
                    mano.Add(cartaEfecto7[0]);
                }else
                    mano.Add(cartaEfecto7[1]);
            }else if(PlayerPrefs.GetInt("DificultadIA") == 1){
                List<Carta> manoEfecto = new List<Carta>{cartaEfecto7[0], cartaEfecto7[1]};
                mano.Add(cartaEfecto7[ia.MonteCarloEfecto1y7(manoEfecto, mano)]);
            }

            mano[mano.Count-1].SetJugador(id);
            gc.GuardarMano(id, mano);
        }
    }

    public void Efecto7(int id){
        Carta carta;

        if(id > 8){
            if(id == 9)
                carta = cartaEfecto7[0];
            else 
                carta = cartaEfecto7[1];

            //gc.manoDelJugador.RemoveAt(gc.idManoCartaSeleccionada);
            gc.manoDelJugador.Add(carta);

            for(int i = 0; i < gc.ImagenCartasMano.Length; i++){
                if(i < gc.manoDelJugador.Count){
                    Sprite imagen = Resources.Load<Sprite>(gc.manoDelJugador[i].nombreImagen);
                    gc.ImagenCartasMano[i].sprite = imagen;
                    gc.ImagenCartasMano[i].gameObject.SetActive(true);
                }
            }

            gc.ImagenCartaEfecto7SetActive(false);
            gc.IconosSetActive(true);
            gc.CartaUtilizadaSetActive(true);
            gc.CartaUtilizadaEfectosSetActive(false);    //todo
            gj.ActualizarAtaques();

            gj.esperandoAccion = false;
            gc.textoGuia.text = "Esperando al resto de jugadores";
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha escogido la carta " + gc.manoDelJugador[4].nombreImagen.Substring(7);
            gc.AumentarLeyenda();
        }
    }


    public void Efecto8(int id){
        id = id - 5;
        if(gj.jugadorVivo[id]){
            efecto8Jugadores.Add(id);
            Debug.Log("Efecto 8");
            
            if(efecto8Jugadores.Count == 2 && efecto8Jugadores[0] != efecto8Jugadores[1]){
                gj.jugadaEnCurso = false;
                Carta carta = gj.cartasRonda[efecto8Jugadores[0]];
                gj.cartasRonda[efecto8Jugadores[0]] = gj.cartasRonda[efecto8Jugadores[1]];
                gj.cartasRonda[efecto8Jugadores[1]] = carta;
                gj.cartasRonda[efecto8Jugadores[0]].SetJugador(efecto8Jugadores[0]);
                gj.cartasRonda[efecto8Jugadores[1]].SetJugador(efecto8Jugadores[1]);

                Sprite imagen = Resources.Load<Sprite>(gj.cartasRonda[efecto8Jugadores[0]].nombreImagen);
                gc.cartaUtilizada[efecto8Jugadores[0]].sprite = imagen;

                imagen = Resources.Load<Sprite>(gj.cartasRonda[efecto8Jugadores[1]].nombreImagen);
                gc.cartaUtilizada[efecto8Jugadores[1]].sprite = imagen;

                
                gj.esperandoAccion = false;
                if(efecto8Jugadores[0] == 0)
                    gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[efecto8Jugadores[1]];
                else if(efecto8Jugadores[1] == 0)
                    gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[efecto8Jugadores[0]];
                else
                    gc.leyenda.text += "\n" + gc.nombreDinosaurios[0] + " ha intercambiado las cartas de " +
                     gc.nombreDinosaurios[efecto8Jugadores[0]] + " con la de " + gc.nombreDinosaurios[efecto8Jugadores[1]];
                gc.AumentarLeyenda();
                efecto8Jugadores.Clear();
                gj.ActualizarAtaques();
                gj.EmpezarJugada();
            }else if(efecto8Jugadores.Count == 2 && efecto8Jugadores[0] == efecto8Jugadores[1])
                efecto8Jugadores.RemoveAt(1);
        }

    }


    public void Efecto8IA(int id){
        int max = 0, maxId = -1;
        int min = 20, minId = -1;
        int j1, j2;
        for(int j = 0; j < gj.cartasRonda.Length; j++){
            if(gj.jugadorVivo[j] && gj.cartasRonda[j].ataque > max){
                max = gj.cartasRonda[j].ataque;
                maxId = j;
            }
            if(gj.jugadorVivo[j] && gj.cartasRonda[j].ataque < min){
                min = gj.cartasRonda[j].ataque;
                minId = j;
            }
        }

        
        if((gj.cartasRonda[id].ataque == max && !efecto5Activado) || (gj.cartasRonda[id].ataque == min && efecto5Activado)){
            do{
                j1 = UnityEngine.Random.Range(0, 4);
            }while(j1 != id && gj.jugadorVivo[j1]);

            if(gj.jugadorVivo.Count(b => b) == 2)
                j2 = id;
            else{
                do{
                    j2 = UnityEngine.Random.Range(0, 4);
                }while(j1 != id && j1 != j2 && gj.jugadorVivo[j2]);
            }

            posicion1 = j1;
            posicion2 = j2;
            
            gj.tiempoEspera += 2;
            Invoke("CambiarSpriteCartasUtilizadas", 2.0f);
        }else{
            if(!efecto5Activado)
                posicion2 = maxId;
            else
                posicion2 = minId;
            posicion1 = id;
            gj.tiempoEspera += 2;
            Invoke("CambiarSpriteCartasUtilizadas", 2.0f);
        }

        if(posicion1 == id)
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[posicion1] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion2];
        else if(posicion2 == id)
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[posicion2] + " ha intercambiado su carta con la de " + gc.nombreDinosaurios[posicion1];
        else
            gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha intercambiado las cartas de " +
                gc.nombreDinosaurios[posicion1] + " con la de " + gc.nombreDinosaurios[posicion2];
        gc.AumentarLeyenda();
    }

    public IEnumerator AgregarCartaEfectos(int id, List<Carta> mano, int idCarta){
        yield return new WaitForSeconds(1);

        gc.cartaUtilizadaEfectos[id].gameObject.SetActive(true);
        Sprite imagen = Resources.Load<Sprite>(mano[idCarta].nombreImagen);
        gc.cartaUtilizadaEfectos[id].sprite = imagen;
        gj.bonusAtaque[id] += mano[idCarta].ataque;
        gc.leyenda.text += "\n" + gc.nombreDinosaurios[id] + " ha descartado la carta " + mano[idCarta].nombreImagen.Substring(7) +
         " para sumarse " + mano[idCarta].ataque + " de ataque";
        gc.AumentarLeyenda();
        mano.RemoveAt(idCarta);
        gc.GuardarMano(id, mano);

        gj.ActualizarAtaques();
    }

    public void CambiarSpriteCartasUtilizadas(){
        Debug.Log("CambiarSprite");
        Carta carta = gj.cartasRonda[posicion1];
        gj.cartasRonda[posicion1] = gj.cartasRonda[posicion2];
        gj.cartasRonda[posicion2] = carta;

        
        Sprite imagen = Resources.Load<Sprite>(gj.cartasRonda[posicion1].nombreImagen);
        gc.cartaUtilizada[posicion1].sprite = imagen;
        
        imagen = Resources.Load<Sprite>(gj.cartasRonda[posicion2].nombreImagen);
        gc.cartaUtilizada[posicion2].sprite = imagen;
        gj.ActualizarAtaques();
    }
}
