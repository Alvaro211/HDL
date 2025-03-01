using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

using TMPro;

public class SelecciónJugador : MonoBehaviour
{
    // Start is called before the first frame update
    private SelecciónJugador instace;
    public SelecciónJugador Instance{
        get{return instace;}
    }

    public List<Image> iconosDinosaurios = new List<Image>();
    public Jugador[] jugador = new Jugador[4];
    public static SelecciónJugador instancia;

    public Dropdown seleccionIA;
    public Dropdown resolucionPantalla;
    public Toggle toggle;
    public bool pantallaCompleta = false;

    public Slider barraDeSonido;
    public TextMeshProUGUI numVolumen;
    public AudioSource audio;
    
    string[] ImagenesCartas = new string[]{"Triceratops", "Rex", "Bronto", "Stego"};

    void Start(){
        if(PlayerPrefs.HasKey("Resolucion")){
            resolucionPantalla.value = PlayerPrefs.GetInt("Resolucion");
            HandleDropdownValueChanged(resolucionPantalla.value);
            PlayerPrefs.DeleteAll();
        }else{
            resolucionPantalla.value = 2;
            Screen.SetResolution(1280, 720, false);
        }

        instancia = this;
        for(int i = 0; i < iconosDinosaurios.Count; i++){
            iconosDinosaurios[i].sprite = Resources.Load<Sprite>(ImagenesCartas[i]);
            iconosDinosaurios[i].gameObject.SetActive(true);
            iconosDinosaurios[i].preserveAspect = true;
        }
        resolucionPantalla.onValueChanged.AddListener(HandleDropdownValueChanged);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        barraDeSonido.onValueChanged.AddListener(ActualizarVolumen);

        ActualizarVolumen(barraDeSonido.value);
    } 


    public void NuevoJugador(int id){
        jugador[0] = new Jugador(id);

        for(int i = 1; i < jugador.Length;i++){
            if(jugador[0].dinosaurio >= i)
                jugador[i] = new Jugador(i-1);
            else
                jugador[i] = new Jugador(i);
        }

        PlayerPrefs.SetFloat("SegundoMusica", audio.time);
        PlayerPrefs.SetFloat("VolumenMusica", audio.volume);
        PlayerPrefs.SetInt("PantallaCompleta", pantallaCompleta ? 1 : 0);
        PlayerPrefs.SetInt("Resolucion", resolucionPantalla.value);
        PlayerPrefs.SetInt("DificultadIA", seleccionIA.value);
        SceneManager.LoadScene("Puntuaciones");
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
}
