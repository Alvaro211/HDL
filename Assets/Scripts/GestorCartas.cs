using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI; 
using UnityEngine;
using System;
using TMPro;

public class GestorCartas : MonoBehaviour   {

    public AudioSource audio;

    public GestorJugada gestorJugada;
    public JugadaEfectos efecto;
    public JugadaHechizos hechizo;
    public Jugador[] jugador = new Jugador[4];
    public bool jugador0Bot = false;
    public List<Carta> Mazo = new List<Carta>();
    public List<Carta> Descartes = new List<Carta>();
    public List<CartaDesastre> mazoDesastres = new List<CartaDesastre>();
    public List<Carta> manoDelJugador = new List<Carta>();
    public List<Carta> manoRival1 = new List<Carta>();
    public List<Carta> manoRival2 = new List<Carta>();
    public List<Carta> manoRival3 = new List<Carta>();
    
    public Image[] ImagenCartasMano;
    public Image[] cartaUtilizada;
    public Image[] cartaUtilizadaEfectos;
    public Image[] imagenCartaEfecto7;
    public Image[] iconos;
    public Image[] iconosHechizo;
    public Image fondo; 

    public TextMeshProUGUI textoGuia;
    public TextMeshProUGUI textoAyuda;
    public ScrollRect scrollView;

    public Scrollbar scrollbar;
    public TextMeshProUGUI leyenda;
    public Button buton;
    public string[] nombreDinosaurios = new string[]{"Triceratops", "Rex", "Bronto", "Stego"};

    public int idManoCartaSeleccionada;
    
    public List<int> idManoCartaSeleccionadaEfectos = new List<int>();
    List<int> idDinosaurios = new List<int>();
    List<string> ImagenesIcono = new List<string>{"Triceratops", "Rex", "Bronto", "Stego"};
    public List<Carta> manoEfecto = new List<Carta>();

    public int efecto1JugadorSeleccionado = -1;
    public int descartes = 3;
    public int menorIdCartaUtilizada = 0;

    public Carta PERDER = new Carta("Reverso", -1, -1, false, false);
    public Carta PRUEBA = new Carta("Objeto8Planta", 8, 0, true, false);
    public Carta PRUEBA2 = new Carta("Objeto1Palo", 1, 0, false, false);

    List<int> ganadorId = new List<int>();
    public bool etapaDescarte = false;


    // Start is called before the first frame update
    void Start(){

        audio.time = PlayerPrefs.GetFloat("SegundoMusica");
        audio.volume = PlayerPrefs.GetFloat("VolumenMusica");

        string manoDelJugadorS = PlayerPrefs.GetString("manoDelJugador", "");
        if(manoDelJugadorS != ""){
            ObtenerVariables(manoDelJugadorS);
        }else{
            CrearMazos();
        }

        foreach (Image imagenCarta in ImagenCartasMano)
        {
            imagenCarta.gameObject.SetActive(false);
            imagenCarta.preserveAspect = true;
        }
        
        foreach (Image imagenCarta in cartaUtilizada){
            imagenCarta.gameObject.SetActive(false);
            imagenCarta.preserveAspect  = true;
        }
        
        foreach (Image imagenCarta in cartaUtilizadaEfectos){
            imagenCarta.gameObject.SetActive(false);
            imagenCarta.preserveAspect  = true;
        }

        cartaUtilizada[4].gameObject.SetActive(true);
        Sprite imagen = Resources.Load<Sprite>(mazoDesastres[0].nombreImagen);
        cartaUtilizada[4].sprite = imagen;

        ImagenCartaEfecto7SetActive(false);
        IconosHechizosSetActive(false);

        //Obtenemos la referencia del jugador y ponemos los sprite de los iconos
        jugador = SelecciónJugador.instancia.jugador;
        iconos[0].sprite = Resources.Load<Sprite>(ImagenesIcono[jugador[0].dinosaurio]);
        iconos[0].gameObject.SetActive(false);
        iconos[0].preserveAspect = true;

        iconos[4].sprite = Resources.Load<Sprite>("Bonificacion" + ImagenesIcono[jugador[0].dinosaurio]);
        iconos[4].gameObject.SetActive(false);
        iconos[4].preserveAspect = true;
        idDinosaurios.Add(jugador[0].dinosaurio);
        ImagenesIcono.RemoveAt(jugador[0].dinosaurio);

        for(int i = 0; i < ImagenesIcono.Count; i++){
            iconos[i+1].sprite = Resources.Load<Sprite>(ImagenesIcono[i]);
            iconos[i+1].gameObject.SetActive(false);
            iconos[i+1].preserveAspect = true;

            iconos[i+5].sprite = Resources.Load<Sprite>("Bonificacion" + ImagenesIcono[i]);
            iconos[i+5].gameObject.SetActive(false);
            iconos[i+5].preserveAspect = true;
            
            if(i >= jugador[0].dinosaurio){ 
                idDinosaurios.Add(i+1);
            }else{
                idDinosaurios.Add(i);
                
            }
        }

        for(int i = 0; i < gestorJugada.jugadorVivo.Length; i++){
            gestorJugada.jugadorVivo[i] = jugador[i].SigoVivo();

            if(jugador[i].vivo && jugador[i].puntuacion >= 50){
                ganadorId.Add(i);
            }
        }

        if(!jugador[0].vivo){
            //Quitar todas las imagenes, cambiar fondo, sleep y salir
            fondo.sprite = Resources.Load<Sprite>("FondoPerder");
            fondo.gameObject.SetActive(true);
            cartaUtilizada[4].sprite = iconos[0].sprite;
            ImagenCartasManoSetActive(false);
            textoAyuda.gameObject.SetActive(false);
            textoGuia.gameObject.SetActive(false);
            scrollView.gameObject.SetActive(false);
            buton.gameObject.SetActive(false);
            gestorJugada.TextoAtaqueSetActive(false);
            return;
        }

        if(ganadorId.Count > 0){
            //Quitar todas las imagenes, cambiar fondo, sleep y salir
            bool ganador0 = false;
            
            for(int i = 0; i < ganadorId.Count; i++){
                if(ganadorId[i] == 0)
                    ganador0 = true;
            }

            if(ganador0){
                fondo.sprite = Resources.Load<Sprite>("FondoGanar");
                fondo.gameObject.SetActive(true);
                cartaUtilizada[4].sprite = iconos[0].sprite;
                ImagenCartasManoSetActive(false);
                textoAyuda.gameObject.SetActive(false);
                textoGuia.gameObject.SetActive(false);
                scrollView.gameObject.SetActive(false);
                buton.gameObject.SetActive(false);
                gestorJugada.TextoAtaqueSetActive(false);
                return;
            }
        }

        efecto1JugadorSeleccionado = -1;
        AgregarCartasALaMano();
        textoGuia.fontSize = 24;
        textoGuia.text = "Juege una carta";
    }


    public void CrearMazos(){
        int[] AtaqueCartas = new int[]{0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, -1, -1, -1, -1, -1};
        int[] CantidadCartas = new []{4, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 3, 2, 3, 1, 3, 1, 5, 5, 2, 3, 1};
        string[] ImagenesCartas = new string[]{"Objeto0", "Objeto1Palo", "Objeto1Dinno", "Objeto2Sarten", "Objeto2Serpiente", "Objeto3Pala", "Objeto3Batidora", "Objeto4Pico", "Objeto4Fuego", "Objeto5Bate", "Objeto5Trampa", "Objeto6Arco", "Objeto6Estrella", "Objeto7Bicho", "Objeto7Prismaticos", "Objeto8Picos", "Objeto8Planta", "Objeto9Bazuka", "Objeto9Motosierra", "ObjetoScoreBooster", "ObjetoScoreSapper", "ObjetoScoreInversion", "ObjetoDisasterInsurnce", "ObjetoDisasterRedirect"};
        string[] ImagenesDesastres = new string[]{"DesastreR1", "DesastreR2", "DesastreR3", "DesastreR4", "DesastreR5", "DesastreR6", "DesastreR7", "DesastreR8", "DesastreV1", "DesastreV2", "DesastreV3", "DesastreV4", "DesastreV5", "DesastreV6", "DesastreV7", "DesastreV8", "DesastreA1", "DesastreA2", "DesastreA3", "DesastreA4", "DesastreA5", "DesastreA6", "DesastreA7", "DesastreA8", "DesastreMeteorito" };
        int id = 0, color = 0;
        bool efecto = true, hechizo = false;

        //Creamos todas las cartas y las añadimos al mazo
        for (int i = 0; i < AtaqueCartas.Length; i++){
            for (int j = 0; j < CantidadCartas[i]; j++){
                Carta carta = new Carta(ImagenesCartas[i], AtaqueCartas[i], id, efecto, hechizo);
                Mazo.Add(carta);
                id++;
            }
            if(i == 18){
                efecto = false;
                hechizo = true;
            }else if(i < 18)   
                efecto = !efecto;
        }
        
        for(int i = 0; i < ImagenesDesastres.Length; i++){
            //Sprite imagen = Resources.Load<Sprite>(ImagenesDesastres[i]);
            CartaDesastre carta = new CartaDesastre(ImagenesDesastres[i], color);
            mazoDesastres.Add(carta);
            if(i == ImagenesDesastres.Length -1){
                mazoDesastres.Add(carta);           //Añadimos la carta del meteorito 2 veces más
                mazoDesastres.Add(carta); 
            }
            if (i == 7 || i == 15 || i == 23)
                color++;
        }
    
        FuncionesAuxiliares.Shuffle(Mazo);
        FuncionesAuxiliares.Shuffle(mazoDesastres);
    }


    public void IconosSetActive(bool b){
        for(int i = 0; i < iconos.Length/2; i++){
            if(gestorJugada.jugadorVivo[i] && b){
                if(!gestorJugada.empate){
                    iconos[i].gameObject.SetActive(b);
                    iconos[i+4].gameObject.SetActive(b);
                }else{
                    if(gestorJugada.idPerdedores[i])
                        iconos[i].gameObject.SetActive(b);
                }
            }else{
                iconos[i].gameObject.SetActive(false);
                iconos[i+4].gameObject.SetActive(false);
            }
        }
    }

    public void IconosHechizosSetActive(bool b){
        foreach(Image icono in iconosHechizo){
            icono.gameObject.SetActive(b);
        }
    }

    public void CartaUtilizadaSetActive(bool b){
        for(int i = 0; i < cartaUtilizada.Length; i++){
            if(i == 4 || gestorJugada.jugadorVivo[i])
                cartaUtilizada[i].gameObject.SetActive(b);
        }
    }

    public void ImagenCartasManoSetActive(bool b){
        for(int i = 0; i < manoDelJugador.Count; i++){    
            ImagenCartasMano[i].gameObject.SetActive(b);
        }
    }

    public void CartaUtilizadaEfectosSetActive(bool b){
        for(int i = 0; i < cartaUtilizadaEfectos.Length; i++){
            if(b && cartaUtilizadaEfectos[i] != null && gestorJugada.jugadorVivo[i])
                cartaUtilizadaEfectos[i].gameObject.SetActive(false);
            else
                cartaUtilizadaEfectos[i].gameObject.SetActive(b);
        }
    }

    public void ImagenCartaEfecto7SetActive(bool b){
        for(int i = 0; i < imagenCartaEfecto7.Length; i++){

            imagenCartaEfecto7[i].gameObject.SetActive(b);
            imagenCartaEfecto7[i].preserveAspect  = true;
        }
    }



    public List<Carta> BuscarMano(int id){
        List<Carta> mano = new List<Carta>();

        switch(id){
            case 0:
                mano = manoDelJugador;
                break;
            case 1:
                mano = manoRival1;
                break;
            case 2:
                mano = manoRival2;
                break;
            case 3:
                mano = manoRival3;
                break;
        }

        return mano;
    }

    public void GuardarMano(int id, List<Carta> mano){
        switch(id){
            case 0:
                manoDelJugador = mano;
                break;
            case 1:
                manoRival1 = mano;
                break;
            case 2:
                manoRival2 = mano;
                break;
            case 3:
                manoRival3 = mano;
                break;
        }
    }

    public int ObtenerIdCartaManoPorNombre(string nombre, List<Carta> mano){
        for(int i = 0; i < mano.Count; i++){
            if(mano[i].nombreImagen == nombre){
                return i;
            }
        }
        return -1;
    }

    public bool TieneHechizosScore(List<Carta> mano){
        foreach(Carta carta in mano){
            if(carta.nombreImagen == "ObjetoScoreBooster" || carta.nombreImagen == "ObjetoScoreSapper" 
                || carta.nombreImagen == "ObjetoScoreInversion"){
                    //Debug.Log("Tengo Hechizos Score");
                return true;
            }
        }
        return false;
    }


    public int TieneHechizosDisaster(List<Carta> mano){
        for(int i = 0; i < mano.Count; i++){
            if(mano[i].nombreImagen == "ObjetoDisasterInsurnce" || mano[i].nombreImagen == "ObjetoDisasterRedirect"){
                return i;
            }
        }
        return -1;
    }


    public void OrdenarCartasMano(){
        int i;
        for(i = 0; i < manoDelJugador.Count; i++){
            Sprite imagen = Resources.Load<Sprite>(manoDelJugador[i].nombreImagen);
            ImagenCartasMano[i].sprite = imagen;
            ImagenCartasMano[i].gameObject.SetActive(true);
        }

        for(; i < cartaUtilizada.Length; i++){
            ImagenCartasMano[i].gameObject.SetActive(false);
        }
    }


    //Función para añadir cartas a la mano y pone los sprite de las cartas del jugador
    public void AgregarCartasALaMano()  {
        if(!cartaUtilizada[0].gameObject.activeSelf && !efecto.efectosFinRonda){
        
            AgregarCartas(manoDelJugador, 0);
            if(gestorJugada.jugadorVivo[1])
                AgregarCartas(manoRival1, 1);
            if(gestorJugada.jugadorVivo[2])
                AgregarCartas(manoRival2, 2);
            if(gestorJugada.jugadorVivo[3])
                AgregarCartas(manoRival3, 3);

            OrdenarCartasMano();
            ImagenCartasManoSetActive(true);
        }

        if(jugador0Bot)
            StartCoroutine(gestorJugada.EmpezarAutomatico());
    }

    //Si el mazo donde se roba las cartas no contiene suficientes para robar el mazo
    //las cartas del mazo de Descartes se añaden al mazo de robar y se baraja
    public void RellenarMazo(int cartas){
        if(cartas > Mazo.Count){
            FuncionesAuxiliares.Shuffle(Descartes);
            foreach(Carta carta in Descartes){
                Mazo.Add(carta);
            }
            Descartes.Clear();
        }
    }


    //Saca las cartas del mazo y las pone en la mano 
    public int AgregarCartas(List<Carta> ListMano, int id){
        int cartasFaltantes = 5 - ListMano.Count; // Cuántas cartas faltan para tener 5 en la mano

        RellenarMazo(cartasFaltantes);

        //Se añade las cartas a la mano 
        for (int i = 0; i < cartasFaltantes; i++){
            // Generar un índice aleatorio para seleccionar una carta aleatoria
            int indiceAleatorio = UnityEngine.Random.Range(0, Mazo.Count);
            Carta cartaAleatoria = Mazo[indiceAleatorio];
            cartaAleatoria.SetJugador(id);

            // Agregar la carta a la mano del jugador y quitarla de la lista de cartas disponibles
            if(idManoCartaSeleccionada <= ListMano.Count){
                ListMano.Add(cartaAleatoria);
            }else{
                ListMano.Add(cartaAleatoria);
            }
            Mazo.RemoveAt(indiceAleatorio);
        }
        //manoDelJugador[0] = PRUEBA;
        // manoDelJugador[1] = PRUEBA2;
        return cartasFaltantes;
    }

    


    //Funcion para quitar el sprite de las cartas al hacer click sobre ellas
    //y llevar ese sprite a la carta de la ronda o bien a la carta de la mano
    public void DeshabilitarCarta(GameObject carta, int id){
        if(id != 5){
            // Encuentra la imagen asociada al GameObject de la carta
            Image imagenCarta = carta.GetComponent<Image>();

            if(etapaDescarte){
                if(manoDelJugador[id].nombreImagen == "ObjetoDisasterInsurnce"){
                    jugador[0].cartasDesastres.RemoveAt(jugador[0].cartasDesastres.Count-1);
                    manoDelJugador.RemoveAt(id);
                    OrdenarCartasMano();
                    textoGuia.text = "Descarte otra carta";
                    // GuardarVariables();
                    // SceneManager.LoadScene("Puntuaciones");
                }else if(manoDelJugador[id].nombreImagen == "ObjetoDisasterRedirect"){
                    //jugador[0].cartasDesastres.RemoveAt(jugador[0].cartasDesastres.Count-1);
                    manoDelJugador.RemoveAt(id);
                    OrdenarCartasMano();
                    IconosSetActive(true);
                }else{
                    manoDelJugador.RemoveAt(id);
                    GuardarVariables();
                    SceneManager.LoadScene("Puntuaciones");  
                }
                return;
            }

            //Si no hay ninguna carta seleccionada o esta el efecto de la carta 0, 
            //pone el sprite de la carta como seleccionada
            if (!cartaUtilizada[0].gameObject.activeSelf && !efecto.efectos && !manoDelJugador[id].esHechizo)
                UsarCarta(id, imagenCarta);
            else if (efecto.efectos && (gestorJugada.esperandoAccion || !efecto.efecto4Acabado)){ 
                    switch(efecto.idEfectos){
                    case 0:
                        idManoCartaSeleccionadaEfectos.Add(id);
                        imagenCarta.gameObject.SetActive(false);
                        efecto.Efecto0();
                        break;
                    
                    case 3:
                        if(manoDelJugador[id].efecto){
                            idManoCartaSeleccionadaEfectos.Add(id);
                            imagenCarta.gameObject.SetActive(false);
                            efecto.Efecto0();
                        }
                        break;

                    case 4:
                        efecto.Efecto4(id, imagenCarta);
                        break;

                    case 7:
                        efecto.Efecto7(id);
                        break;

                    default:
                        break;
                    }
                //}
            }
        }else{
            //Si no esta efectos activados y no es la carta de la ronda del jugador
            if(!efecto.efectos){
                ImagenCartasManoSetActive(true);
                cartaUtilizada[0].gameObject.SetActive(false);
            }else{
                DeshabilitarCartaEfecto0();
            }
        }
    }

    public void UsarCarta(int id, Image imagenCarta){
         // Desactiva la imagen
        imagenCarta.gameObject.SetActive(false);

        idManoCartaSeleccionada  = id;

        //Sprite imagen = Resources.Load<Sprite>(ImagenCartasMano[id].nombreImagen);
        cartaUtilizada[0].sprite = ImagenCartasMano[id].sprite;
        cartaUtilizada[0].gameObject.SetActive(true);

        //Si no estamos haciendo efectos de alguna carta, ponemos el resto de cartas
        //que se juegan en la ronda como el reverso
        if(!efecto.efectos){
            for(int i = 1; i < 4; i++){
                if(!gestorJugada.empate){
                    if(gestorJugada.jugadorVivo[i]){
                        cartaUtilizada[i].sprite = Resources.Load<Sprite>("Reverso");
                        cartaUtilizada[i].gameObject.SetActive(true);
                    }
                }else{
                    if(gestorJugada.idPerdedores[i]){
                        cartaUtilizada[i].sprite = Resources.Load<Sprite>("Reverso");
                        cartaUtilizada[i].gameObject.SetActive(true);
                    }
                }
            }

            IconosSetActive(true);
        }
    }


    public void DeshabilitarCartaEfecto0(){
        if(idManoCartaSeleccionadaEfectos.Count > 0){
            ImagenCartasMano[idManoCartaSeleccionadaEfectos[0]].gameObject.SetActive(true);
            gestorJugada.esperandoAccion = true;
            gestorJugada.bonusAtaque[0] -= manoDelJugador[idManoCartaSeleccionadaEfectos[0]].ataque;
            idManoCartaSeleccionadaEfectos.RemoveAt(idManoCartaSeleccionadaEfectos.Count-1);
            cartaUtilizadaEfectos[0].gameObject.SetActive(false);
            gestorJugada.ActualizarAtaques();
        }
    }
 

    //Restablece valores y quita la carta de la mano para empezar otra vez la ronda
    public void AcabarRonda(Carta[] cartaRonda){
        Debug.Log("AcabarRonda");
        gestorJugada.jugadaEnCurso = false;
        gestorJugada.acabarAnimacion = true;
        hechizo.scoreInversion = false;
        efecto.efecto7Activo = false;
        efecto.efecto6Activado = false;
        efecto.efecto0Activo = false;
        int perdedor = -1;
        int cartasParaEmpate = -1;

        if(gestorJugada.numPerdedores > 1 && gestorJugada.numPerdedores < 4){
            for(int i = 0; i < 4; i++){
                if(gestorJugada.idPerdedores[i]){
                    var mano = BuscarMano(i);
                    int count = 0;
                    foreach(Carta carta in mano){
                        if(!carta.esHechizo)
                            count++;
                    }

                    if(count == 0){
                        cartasParaEmpate = i;
                        break;
                    }
                }
            }
            
            if(cartasParaEmpate != -1){
                for(int i = 0; i < 4; i++)
                    gestorJugada.idPerdedores[i] = false;

                gestorJugada.numPerdedores = 1;
                gestorJugada.idPerdedores[cartasParaEmpate] = true;
            }
        }

        if(gestorJugada.numPerdedores == 1){
            perdedor = Array.IndexOf(gestorJugada.idPerdedores, true);
            Debug.Log("Dar carta desastre al perdedor");
            jugador[perdedor].cartasDesastres.Add(mazoDesastres[0].nombreImagen);
            Debug.Log("El jugador " + perdedor + " tiene " + jugador[perdedor].cartasDesastres.Count + " cartasDesastre");

            for(int i = 0; i < 4; i++){
                jugador[i].puntuacion += jugador[i].cartasDesastres.Count;
            }
        }

        for(int i = 0; i < 4; i++){
            int.TryParse(gestorJugada.textoAtaque[i].text, out int ataque);
            if(gestorJugada.ganadores[i] && !gestorJugada.empate)
                jugador[i].puntuacion += ataque;
        }


        
        for(int i = 0; i < cartaUtilizada.Length; i++){ 
            cartaUtilizada[i].gameObject.SetActive(false);
            //Añadir la cartas utilizadas al mazo de descartes, cartaUtilizada contiene la carta de Desastre
            if(i < cartaRonda.Length && gestorJugada.jugadorVivo[i]){
                Descartes.Add(cartaRonda[i]);
            }
        }
        
        
        efecto.efectos = false;

        if(gestorJugada.numPerdedores == 1 && (perdedor > 0 || jugador0Bot)){
            List<Carta> mano = BuscarMano(perdedor);
            int posicion = TieneHechizosDisaster(mano);
            int ultimaCarta = jugador[perdedor].cartasDesastres.Count-1;
        
            if(posicion != -1 && mano[posicion].nombreImagen == "ObjetoDisasterInsurnce"){
                jugador[perdedor].cartasDesastres.RemoveAt(ultimaCarta);
                jugador[perdedor].puntuacion -= 1;
                Debug.Log("El jugador " + perdedor + " ha descartado la carta desastre");
            }else if(posicion != -1 && mano[posicion].nombreImagen == "ObjetoDisasterRedirect"){
                DarCartaDesastre(perdedor, -1);
                jugador[perdedor].puntuacion -= 1;
                Debug.Log("El jugador " + perdedor + " ha redirigido la carta desastre");
            }
            DescartarCarta(perdedor);
        }

        //Desactivo el textoAtaque y los iconos
        for (int i = 0; i < gestorJugada.textoAtaque.Length; i++){
            gestorJugada.textoAtaque[i].gameObject.SetActive(false);
            gestorJugada.bonusAtaque[i] = 0;
        }

        for (int i = 0; i < efecto.efectoJugado.Length; i++) {
            efecto.efectoJugado[i] = false;
        }

        CartaUtilizadaEfectosSetActive(false);
        IconosHechizosSetActive(false);
        IconosSetActive(false);
        descartes = 3;
        menorIdCartaUtilizada = 0;
        efecto.efecto5Activado = false;

        AcabarRondaEfectos();
        SiguienteRonda();
    }


    public void SiguienteRonda(){
        int perdedor = -1;

        gestorJugada.jugadaEnCurso = false;
        Debug.Log("NumPerdedores: " + gestorJugada.numPerdedores);
        if(gestorJugada.numPerdedores == 1)
            perdedor = Array.IndexOf(gestorJugada.idPerdedores, true);

        if(efecto.idEfecto.Count == 0 && (gestorJugada.numPerdedores == 1 || gestorJugada.numPerdedores == 4)){
            mazoDesastres.RemoveAt(0);
            idManoCartaSeleccionadaEfectos.Clear();
            if(perdedor > 0 || jugador0Bot){
                GuardarVariables();
                SceneManager.LoadScene("Puntuaciones");
            }else{
                etapaDescarte = true;
                OrdenarCartasMano();
                IconosSetActive(false);
                Debug.Log("Descartar Carta");
                textoGuia.text = "Descartate una carta";
            }
        }else if(efecto.idEfecto.Count == 0 && gestorJugada.numPerdedores > 1 && gestorJugada.numPerdedores < 4){
            IconosSetActive(false);
            Debug.Log("Empate perdedores");
            gestorJugada.empate = true;

            string textoEmpate = "Empate de los jugadores: ";
            for(int i = 0; i < gestorJugada.idPerdedores.Length; i++){
                if(gestorJugada.idPerdedores[i])
                    textoEmpate += i.ToString() + " ";
            }

            textoGuia.text = textoEmpate;
            OrdenarCartasMano();
        }
    }

    public void DescartarCarta(int perdedor){
        List<Carta> mano = BuscarMano(perdedor);
        int minAtaque = 100;
        int id = -1;

        for(int i = 0; i < mano.Count; i++){
            if(mano[i].ataque < minAtaque){
                minAtaque = mano[i].ataque;
                id = i;
            }
        }

        if(id != -1){
            mano.RemoveAt(id);
            GuardarMano(perdedor, mano);
        }
    }

    public void ObtenerVariables(string manoDelJugadorS){
        manoDelJugador = JsonConvert.DeserializeObject<List<Carta>>(manoDelJugadorS);
            
        string manoRival1S = PlayerPrefs.GetString("manoRival1");
        manoRival1 = JsonConvert.DeserializeObject<List<Carta>>(manoRival1S);

        string manoRival2S = PlayerPrefs.GetString("manoRival2");
        manoRival2 = JsonConvert.DeserializeObject<List<Carta>>(manoRival2S);

        string manoRival3S = PlayerPrefs.GetString("manoRival3");
        manoRival3 = JsonConvert.DeserializeObject<List<Carta>>(manoRival3S);

        string mazoS = PlayerPrefs.GetString("Mazo");
        Mazo = JsonConvert.DeserializeObject<List<Carta>>(mazoS);

        string descartesS = PlayerPrefs.GetString("Descartes");
        Descartes = JsonConvert.DeserializeObject<List<Carta>>(descartesS);

        string mazoDesastresS = PlayerPrefs.GetString("mazoDesastres");
        mazoDesastres = JsonConvert.DeserializeObject<List<CartaDesastre>>(mazoDesastresS);

        string jugadorVivoS = PlayerPrefs.GetString("jugadorVivo");
        gestorJugada.jugadorVivo = JsonConvert.DeserializeObject<bool[]>(jugadorVivoS);
    }

    public void GuardarVariables(){
        string manoDelJugadorS = JsonConvert.SerializeObject(manoDelJugador);
        PlayerPrefs.SetString("manoDelJugador", manoDelJugadorS);

        string manoRival1S = JsonConvert.SerializeObject(manoRival1);
        PlayerPrefs.SetString("manoRival1", manoRival1S);

        string manoRival2S = JsonConvert.SerializeObject(manoRival2);
        PlayerPrefs.SetString("manoRival2", manoRival2S);

        string manoRival3S = JsonConvert.SerializeObject(manoRival3);
        PlayerPrefs.SetString("manoRival3", manoRival3S);

        string MazoS = JsonConvert.SerializeObject(Mazo);
        PlayerPrefs.SetString("Mazo", MazoS);

        string DescartesS = JsonConvert.SerializeObject(Descartes);
        PlayerPrefs.SetString("Descartes", DescartesS);

        string mazoDesastresS = JsonConvert.SerializeObject(mazoDesastres);
        PlayerPrefs.SetString("mazoDesastres", mazoDesastresS);

        string jugadorVivoS = JsonConvert.SerializeObject(gestorJugada.jugadorVivo);
        PlayerPrefs.SetString("jugadorVivo", jugadorVivoS);

        PlayerPrefs.SetFloat("SegundoMusica", audio.time);
        PlayerPrefs.SetFloat("VolumenMusica", audio.volume);
    }

    public void AcabarRondaEfectos(){
        while(efecto.idEfecto.Count != 0){
            int i = gestorJugada.idJugadorEfecto.Count-1;
            if(gestorJugada.idJugadorEfecto[i] != 0 || jugador0Bot){
                switch (efecto.idEfecto[i]){
                    case 1:
                        efecto.Efecto1IA();
                        gestorJugada.idJugadorEfecto.RemoveAt(i);
                        efecto.idEfecto.RemoveAt(i);
                        break;
                    default:
                        break;
                }
            }else{
                IconosSetActive(true);
                return;
            }
        }
    }


    //Función para mostrar la mano del personaje escogido y robarle una carta
    public void Efecto1MostrarCartas(int id){
        Debug.Log("Efecto1");
        efecto1JugadorSeleccionado = id;
        gestorJugada.iconoEscogido = true;
        manoEfecto = BuscarMano(id);

        for(int i = 0; (i < manoEfecto.Count || i < 3); i++){
            Sprite imagen = Resources.Load<Sprite>(manoEfecto[i].nombreImagen);
            cartaUtilizada[i].sprite = imagen;
            cartaUtilizada[i].gameObject.SetActive(true);
        }

        textoGuia.text = "Robe una carta";
    }

    public void DarCartaDesastre(int id, int objetivo){
        // Debug.Log("cartasDesastres jugador: " + jugador[id].cartasDesastres.Count);
        // Debug.Log("cartasDesastres jugador objetivo: " + jugador[objetivo].cartasDesastres.Count);
        for(int i = 0; i < jugador.Length; i++){
            if(jugador[i].cartasDesastres.Count > 0){
                Debug.Log("El jugador " + id + ", tiene mas de una carta desastre");
            }
        }

        int ultimaCarta = jugador[id].cartasDesastres.Count-1;
        if(id != 0){
            objetivo = UnityEngine.Random.Range(0, 2);
            if(objetivo >= id){
                objetivo++;
            }
        }
        jugador[objetivo].cartasDesastres.Add(jugador[id].cartasDesastres[ultimaCarta]);
        jugador[id].cartasDesastres.RemoveAt(ultimaCarta);
        jugador[objetivo].puntuacion += 1;
        IconosSetActive(false);
    }


    public void ExplicarCarta(int id){
        textoAyuda.gameObject.SetActive(true);
        scrollView.gameObject.SetActive(false);
        Carta carta;

        if(id < 5)
            carta = manoDelJugador[id];
        else if(id == 5 && (!efecto.efectos && !hechizo.hechizos && efecto1JugadorSeleccionado == -1))
            carta = manoDelJugador[idManoCartaSeleccionada];
        else if (id > 5 && id < 9 && efecto1JugadorSeleccionado == -1)
            carta = gestorJugada.cartasRonda[id-5];
        else if (id > 5 && id < 9 && efecto1JugadorSeleccionado == -1){
            List<Carta> mano = BuscarMano(efecto1JugadorSeleccionado);
            carta = mano[id];
        }else if(id == 9)
            carta = efecto.cartaEfecto7[0];
        else
            carta = efecto.cartaEfecto7[1];

        if(carta == null)
            return;

        switch(carta.nombreImagen){
            case "Objeto0":
                textoAyuda.text = "Descarta una carta para añadir su ataque a la ronda";
                break;

            case "Objeto1Dinno":
                textoAyuda.text = "Al final de la ronda mira la mano de otro jugador y robale una carta";
                break;

            case "Objeto2Serpiente":
                textoAyuda.text = "Intercambia esta carta con otra de sus lados, solo si tiene algun efecto";
                break;

            case "Objeto3Batidora":
                textoAyuda.text = "Descarta una carta con efecto para añadir su ataque a la ronda";
                break;
            
            case "Objeto4Fuego":
                textoAyuda.text = "Descarta hasta 3 cartas";
                break;            
            
            case "Objeto5Trampa":
                textoAyuda.text = "Intercambia la mejor puntuación con la peor al final de la ronda";
                break;            
            
            case "Objeto6Estrella":
                textoAyuda.text = "Duplica el efecto de las cartas Booster y Sapper";
                break;            
            
            case "Objeto7Prismaticos":
                textoAyuda.text = "Coge las 2 primeras cartas del mazo, quedate una, descarta la otra";
                break;            
            
            case "Objeto8Planta":
                textoAyuda.text = "Intercambia 2 cartas";
                break;            
            
            case "Objeto9Motosierra":
                textoAyuda.text = "Desactiva todos los efecos";
                break;            
            
            case "ObjetoScoreBooster":
                textoAyuda.text = "Añade 2 de puntuación a cualquier jugador";
                break;            
            
            case "ObjetoScoreSapper":
                textoAyuda.text = "Resta 2 de puntuación a cualquier jugador";
                break;            
            
            case "ObjetoScoreInversion":
                textoAyuda.text = "Intercambia la mejor puntuación con la peor al final de la ronda";
                break;            
            
            case "ObjetoDisasterInsurnce":
                textoAyuda.text = "Al perder, descarta esta carta y no cogeras la carta de Desastre";
                break;            
            
            case "ObjetoDisasterRedirect":
                textoAyuda.text = "Redirige la carta de Desastre a otra persona";
                break;
            
            default:
                textoAyuda.gameObject.SetActive(false);
                scrollView.gameObject.SetActive(true);
            break;
        }
    }
    

    public void FinExplicarCarta(){
        textoAyuda.gameObject.SetActive(false);
        scrollView.gameObject.SetActive(true);
    }

    public void AumentarLeyenda(){
        int count = 0;
        float offsetY;
        string[] lineas = leyenda.text.Split("\n");
        for(int i = 0; i < lineas.Length; i++){
            if(lineas[i].Length > 29)
                count++;
            count++;
        }
        if(count > 5){
            RectTransform rect = leyenda.GetComponent<RectTransform>();
            
            if(lineas[lineas.Length-1].Length > 29)
                offsetY = rect.offsetMin.y - 15;
            offsetY = rect.offsetMin.y - 15;
            rect.offsetMin = new Vector2(rect.offsetMin.x, offsetY);
        }
    }

    // Update is called once per frame
    void Update(){
        if(ganadorId.Count > 0){
            ImagenCartasManoSetActive(false);
        }
    }
}
