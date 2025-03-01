using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EscenaPuntuacion : MonoBehaviour
{
    public Dropdown seleccionIA;
    public Dropdown resolucionPantalla;
    public Toggle toggle;
    public bool pantallaCompleta = false;

    public Slider barraDeSonido;
    public TextMeshProUGUI numVolumen;
    public AudioSource audio;
    public Image tablero;
    public Image icono;
    public Image[] imagenCartasDesastres;
    public Jugador[] jugador = new Jugador[4];
    int idJugador = 0;
    bool ajustes = false;

    List<string> ImagenesTablero = new List<string>{"TableroPuntuacionTriceratops", "TableroPuntuacionRex", "TableroPuntuacionBronto", "TableroPuntuacionStego"};
    List<string> ImagenesIcono = new List<string>{"Triceratops", "Rex", "Bronto", "Stego"};                                                                                             //5                                                                                                                       //10                                                                                                              //15                                                                                                                //20                                                                                                        //25                                                                                                            //30                                                                                                            //35                                                                                                //40                                                                                                                //45
    
    List<Vector2> posicionPuntuacionTri = new List<Vector2>{new Vector2(-190, -154), new Vector2(-190, -123), new Vector2(-190, -92), new Vector2(-190, -56), new Vector2(-145, -56), new Vector2(-145, -89), new Vector2(-145, -123), new Vector2(-100, -123), new Vector2(-62, -123), new Vector2(-19, -123), new Vector2(19, -123), new Vector2(58, -123), new Vector2(58, -84), new Vector2(19, -84), new Vector2(-22, -84), new Vector2(-60, -84), new Vector2(-100, -84), new Vector2(-100, -42), new Vector2(-60, -40), new Vector2(-23, -40), new Vector2(19, -40), new Vector2(57, -38), new Vector2(57, 0), new Vector2(19, 0), new Vector2(-19, 0), new Vector2(-61, 0), new Vector2(-102, 0), new Vector2(-143, 0), new Vector2(-184, 5), new Vector2(-184, 42), new Vector2(-144, 42), new Vector2(-102, 42), new Vector2(-62, 44), new Vector2(-23, 44), new Vector2(19, 44), new Vector2(57, 46), new Vector2(56, 82), new Vector2(54, 122), new Vector2(14, 123), new Vector2(13, 87), new Vector2(-25, 86), new Vector2(-66, 85), new Vector2(-106, 85), new Vector2(-146, 85), new Vector2(-185, 90), new Vector2(-187, 126), new Vector2(-146, 128), new Vector2(-105, 128), new Vector2(-65, 128), new Vector2(-27, 130), new Vector2(-27, 164)};
    
    List<Vector2> posicionPuntuacionRex = new List<Vector2>{new Vector2(-190, -154), new Vector2(-190, -123), new Vector2(-190, -92), new Vector2(-190, -56), new Vector2(-148, -56), new Vector2(-148, -89), new Vector2(-145, -123), new Vector2(-109, -127), new Vector2(-66, -127), new Vector2(-26, -127), new Vector2(14, -127), new Vector2(51, -125), new Vector2(51, -85), new Vector2(12, -84), new Vector2(-27, -84), new Vector2(-67, -84), new Vector2(-106, -82), new Vector2(-107, -44), new Vector2(-67, -40), new Vector2(-28, -40), new Vector2(14, -40), new Vector2(51, -40), new Vector2(50, 0), new Vector2(12, 0), new Vector2(-26, 0), new Vector2(-68, 0), new Vector2(-110, 0), new Vector2(-147, 0), new Vector2(-187, 5), new Vector2(-187, 41), new Vector2(-149, 43), new Vector2(-108, 43), new Vector2(-68, 44), new Vector2(-29, 43), new Vector2(13, 43), new Vector2(51, 44), new Vector2(51, 84), new Vector2(47, 123), new Vector2(10, 122), new Vector2(8, 87), new Vector2(-32, 85), new Vector2(-71, 85), new Vector2(-110, 85), new Vector2(-150, 85), new Vector2(-190, 87), new Vector2(-191, 125), new Vector2(-152, 125), new Vector2(-108, 128), new Vector2(-69, 127), new Vector2(-31, 127), new Vector2(-32, 164)};
    
    List<Vector2> posicionPuntuacionBronto = new List<Vector2>{new Vector2(-190, -154), new Vector2(-190, -123), new Vector2(-190, -92), new Vector2(-190, -56), new Vector2(-145, -56), new Vector2(-145, -89), new Vector2(-145, -123), new Vector2(-100, -123), new Vector2(-62, -123), new Vector2(-19, -123), new Vector2(19, -123), new Vector2(58, -123), new Vector2(58, -84), new Vector2(19, -84), new Vector2(-22, -84), new Vector2(-60, -84), new Vector2(-100, -84), new Vector2(-100, -42), new Vector2(-60, -40), new Vector2(-23, -40), new Vector2(19, -40), new Vector2(57, -38), new Vector2(57, 0), new Vector2(19, 0), new Vector2(-19, 0), new Vector2(-61, 0), new Vector2(-102, 0), new Vector2(-143, 0), new Vector2(-184, 5), new Vector2(-184, 42), new Vector2(-144, 42), new Vector2(-102, 42), new Vector2(-62, 44), new Vector2(-23, 44), new Vector2(19, 44), new Vector2(57, 46), new Vector2(56, 82), new Vector2(54, 122), new Vector2(14, 123), new Vector2(13, 87), new Vector2(-25, 86), new Vector2(-66, 85), new Vector2(-106, 85), new Vector2(-146, 85), new Vector2(-185, 90), new Vector2(-187, 126), new Vector2(-146, 128), new Vector2(-105, 128), new Vector2(-65, 128), new Vector2(-27, 130), new Vector2(-27, 164)};
    
    List<Vector2> posicionPuntuacionStego = new List<Vector2>{new Vector2(-193, -154), new Vector2(-193, -117), new Vector2(-193, -86), new Vector2(-191, -49), new Vector2(-151, -51), new Vector2(-151, -85), new Vector2(-149, -121), new Vector2(-109, -124), new Vector2(-70, -125), new Vector2(-28, -125), new Vector2(10, -125), new Vector2(48, -122), new Vector2(46, -85), new Vector2(10, -83), new Vector2(-28, -83), new Vector2(-69, -82), new Vector2(-107, -82), new Vector2(-110, -41), new Vector2(-71, -40), new Vector2(-29, -40), new Vector2(10, -40), new Vector2(46, -40), new Vector2(46, 0), new Vector2(9, 0), new Vector2(-30, 0), new Vector2(-71, 0), new Vector2(-111, 3), new Vector2(-152, 3), new Vector2(-190, 4), new Vector2(-192, 44), new Vector2(-152, 46), new Vector2(-110, 45), new Vector2(-69, 44), new Vector2(-30, 43), new Vector2(9, 43), new Vector2(46, 44), new Vector2(46, 81), new Vector2(46, 121), new Vector2(7, 123), new Vector2(5, 86), new Vector2(-32, 86), new Vector2(-71, 85), new Vector2(-115, 86), new Vector2(-156, 87), new Vector2(-193, 90), new Vector2(-194, 128), new Vector2(-156, 130), new Vector2(-115, 129), new Vector2(-72, 126), new Vector2(-35, 129), new Vector2(-35, 164)};
 
    // Start is called before the first frame update
    void Start(){
        seleccionIA.value = PlayerPrefs.GetInt("DificultadIA");
        audio.time = PlayerPrefs.GetFloat("SegundoMusica");
        audio.volume = PlayerPrefs.GetFloat("VolumenMusica");
        barraDeSonido.value = audio.volume;
        ActualizarVolumen(audio.volume);
        audio.Play();

        pantallaCompleta = PlayerPrefs.GetInt("PantallaCompleta") == 1;
        toggle.isOn = pantallaCompleta;

        resolucionPantalla.value = PlayerPrefs.GetInt("Resolucion");

        resolucionPantalla.onValueChanged.AddListener(HandleDropdownValueChanged);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        barraDeSonido.onValueChanged.AddListener(ActualizarVolumen);

        jugador = SelecciónJugador.instancia.jugador;
        idJugador = 0;
        CerrarAjustes();

        tablero.sprite = Resources.Load<Sprite>(ImagenesTablero[jugador[0].dinosaurio]);
        tablero.preserveAspect = true;

        icono.sprite = Resources.Load<Sprite>(ImagenesIcono[jugador[0].dinosaurio]);
        icono.preserveAspect = true;

        RectTransform posicion = icono.GetComponent<RectTransform>();
        List<Vector2> listPosiciones = ObtenerPosiciones(jugador[0].dinosaurio);
        if(jugador[0].puntuacion <= 50)
            posicion.anchoredPosition = listPosiciones[jugador[0].puntuacion];
        else
            posicion.anchoredPosition = listPosiciones[50];

        for(int i = 0; i < imagenCartasDesastres.Length; i++){
            if(i < jugador[0].cartasDesastres.Count){
                imagenCartasDesastres[i].gameObject.SetActive(true);
                imagenCartasDesastres[i].sprite = Resources.Load<Sprite>(jugador[0].cartasDesastres[i]);
            }else{
                imagenCartasDesastres[i].gameObject.SetActive(false);
            }
        }

        //StartCoroutine(EmpezarAutomatico());
    }

    public IEnumerator EmpezarAutomatico(){
        yield return new WaitForSeconds(0.5f);
        PasarEscena();
    }

    public List<Vector2> ObtenerPosiciones(int id){
        List<Vector2> list = null;
        switch(id){
            case 0:
            list = posicionPuntuacionTri;
            break;

            case 1:
            list = posicionPuntuacionRex;
            break;

            case 2:
            list = posicionPuntuacionBronto;
            break;

            case 3:
            list = posicionPuntuacionStego;
            break;
        }

        return list;
    }

    public void PasarEscena(){
        PlayerPrefs.SetFloat("SegundoMusica", audio.time);
        PlayerPrefs.SetFloat("VolumenMusica", audio.volume);
        PlayerPrefs.SetInt("PantallaCompleta", pantallaCompleta ? 1 : 0);
        PlayerPrefs.SetInt("Resolucion", resolucionPantalla.value);
        PlayerPrefs.SetInt("DificultadIA", seleccionIA.value);
        SceneManager.LoadScene("MainScene");
    }

    public void PasarJugadorIzq(){
        if(idJugador != 0)
            idJugador--;
        else
            idJugador = 3;

        CambiarJugador();
    }


    public void PasarJugadorDer(){
        if(idJugador != 3)
            idJugador++;
        else
            idJugador = 0;

        CambiarJugador();
    }

    public void CambiarJugador(){
        tablero.sprite = Resources.Load<Sprite>(ImagenesTablero[jugador[idJugador].dinosaurio]);
        icono.sprite = Resources.Load<Sprite>(ImagenesIcono[jugador[idJugador].dinosaurio]);

        RectTransform posicion = icono.GetComponent<RectTransform>();
        List<Vector2> listPosiciones = ObtenerPosiciones(idJugador);
        posicion.anchoredPosition = listPosiciones[jugador[idJugador].puntuacion];

        for(int i = 0; i < imagenCartasDesastres.Length; i++){
            if(i < jugador[idJugador].cartasDesastres.Count){
                imagenCartasDesastres[i].gameObject.SetActive(true);
                imagenCartasDesastres[i].sprite = Resources.Load<Sprite>(jugador[idJugador].cartasDesastres[i]);
            }else{
                imagenCartasDesastres[i].gameObject.SetActive(false);
            }
        }
    }

    public void AbrirCerrarAjustes(){
        ajustes = !ajustes;
        if(ajustes)
            AbrirAjustes();
        else
            CerrarAjustes();
    }


    void AbrirAjustes(){
        tablero.gameObject.SetActive(false);
        icono.gameObject.SetActive(false);
        foreach(Image imagen in imagenCartasDesastres){
            imagen.gameObject.SetActive(false);
        }

        seleccionIA.gameObject.SetActive(true);
        resolucionPantalla.gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        barraDeSonido.gameObject.SetActive(true);
    }


    void CerrarAjustes(){
        tablero.gameObject.SetActive(true);
        icono.gameObject.SetActive(true);
        for(int i = 0; i < jugador[idJugador].cartasDesastres.Count; i++){
            imagenCartasDesastres[i].gameObject.SetActive(true);
        }

        seleccionIA.gameObject.SetActive(false);
        resolucionPantalla.gameObject.SetActive(false);
        toggle.gameObject.SetActive(false);
        barraDeSonido.gameObject.SetActive(false);
    }



    void HandleDropdownValueChanged(int index){
        // Obtiene el valor seleccionado del Dropdown
        string seleccion = resolucionPantalla.options[index].text;

        // Lógica para ajustar el tamaño de la ventana según la selección
        if (seleccion == "480x320")
            Screen.SetResolution(480, 320, pantallaCompleta);
        else if(seleccion == "640x480")
            Screen.SetResolution(640, 480, pantallaCompleta);
        else if (seleccion == "HD")
            Screen.SetResolution(1280, 720, pantallaCompleta);
        else if (seleccion == "WXGA")
            Screen.SetResolution(1366, 768, pantallaCompleta);
        else if (seleccion == "Full HD")
            Screen.SetResolution(1920, 1080, pantallaCompleta);
        else if (seleccion == "QHD")
            Screen.SetResolution(2560, 1440, pantallaCompleta);
        else if (seleccion == "4K")
            Screen.SetResolution(3840, 2160, pantallaCompleta);
    }

    void OnToggleValueChanged(bool isOn){
        pantallaCompleta = !pantallaCompleta;
        Screen.SetResolution(Screen.width, Screen.height, pantallaCompleta);
    }

    void ActualizarVolumen(float volumen){
        audio.volume = volumen;
        numVolumen.text = (volumen * 100f).ToString("F0");
    }

    // Update is called once per frame
    void Update()
  {
        
    }
}
