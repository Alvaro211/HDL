using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class CartaInteractiva : MonoBehaviour
{
    public GestorCartas gc; 
    public GestorJugada gj;
    public JugadaEfectos je;
    public JugadaHechizos jh;
    public int id;

    private void OnMouseDown(){
        gc.FinExplicarCarta();
        // Cuando se hace clic en la carta, llama a la función para desactivarla
        if(jh.hechizos && id < 5 && !jh.jugador0Sapper && !jh.jugador0Booster && jh.jugador0PuedeHechizos){
            jh.HechizosJugador(this.gameObject, id);
        }else if(jh.hechizos && id >= 5 && jh.jugador0Booster){
            jh.ScoreBooster(0, id, gc.manoDelJugador);
        }else if(jh.hechizos && id >= 5 && jh.jugador0Sapper){
            jh.ScoreSapper(0, id, gc.manoDelJugador);
        }else if(je.idEfectos == 8 && id > 4 && id < 9){
            je.Efecto8(id);
        }else if(!je.efectosFinRonda && (id < 6 || id >= 9)){
            if(gj.empate && !gj.idPerdedores[0]){
            }else{
                gc.DeshabilitarCarta(this.gameObject, id);
            }
        }else if(je.idEfectos == 1 && id > 4){
            je.Efecto1ObtenerCarta(id);
        }else if(je.idEfectos == 2 && id != 7 && je.efecto2Activo){
            je.Efecto2(id);
        }
    }

    public void RatonEncima(){
        gc.ExplicarCarta(id);
    }

    public void RatonFuera(){
        gc.FinExplicarCarta();
    }

}
