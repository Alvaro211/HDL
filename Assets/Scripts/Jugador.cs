using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Jugador
{
    public const int TRICERATOPS = 0;
    public const int REX = 1;
    public const int BRONTO = 2;
    public const int STEGO = 3;

    public int dinosaurio;
    public int puntuacion;
    public bool vivo;

    public List<string> cartasDesastres;

    //Primera posicion Rojo, Segunda Verde, Tercera Azul y Cuarta
    public int[] colorBonus = new int[4];   

    public Jugador(int _dinosaurio){
        dinosaurio = _dinosaurio;
        puntuacion = 0;
        cartasDesastres = new List<string>();
        vivo = true;

        if(dinosaurio == TRICERATOPS){
            colorBonus[2] = 1;
            colorBonus[1] = -1;
        }else if(dinosaurio == REX){
            colorBonus[0] = 1;
            colorBonus[2] = -1;
        }else if(dinosaurio == BRONTO){
            colorBonus[1] = 1;
            colorBonus[0] = -1;
        }else if(dinosaurio == STEGO){
            colorBonus[3] = 1;
        }
    }

    public bool SigoVivo(){
        int azul = 0, rojo = 0, verde = 0, comodin = 0;
        foreach(string carta in cartasDesastres){
            if(carta[8] == 'A')
                azul++;
            if(carta[8] == 'R')
                rojo++;
            if(carta[8] == 'V')
                verde++;
            if(carta[8] == 'M'){
                comodin++;
            }
        }
        if(azul > 2 || rojo > 2 || verde > 2 || (azul > 0 && rojo > 0 && verde > 0) ||
            ((azul > 1 || rojo > 1 || verde > 1) && comodin > 0) || comodin > 2 ||
            ((azul > 0 || rojo > 0 || verde > 0) && comodin > 1) || 
            (azul > 0 && rojo > 0 && comodin > 0) || (azul > 0 && verde > 0 && comodin > 0) ||
            (rojo > 0 && verde > 0 && comodin > 0)){
                vivo = false;
                return false;
            }
        return true;
    }
}
