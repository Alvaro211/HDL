using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;
using System;

public class IA : MonoBehaviour{

    public void AgentePhyton(List<Carta> mano){
        string pythonInterpreter = "C:\\Python27\\python.exe"; // Puedes cambiar esto según la instalación de Python en tu sistema

        // Ruta al script de Python que deseas ejecutar
        string pythonScript = "C:\\Users\\Lenovo\\Desktop\\Unity\\TutorialUnity2D\\Assets\\Scripts\\Ejemplo.py"; // Cambia esto con la ruta completa a tu script de Python

        var cartasEnMano = new string[] { mano[0].nombreImagen, mano[1].nombreImagen, mano[2].nombreImagen, mano[3].nombreImagen, mano[4].nombreImagen };
        var handInfo = new { cartas = cartasEnMano };
        string cartasInfo = JsonConvert.SerializeObject(handInfo);

        // Crear el proceso de inicio
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonInterpreter,   
            Arguments = pythonScript,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            var cartas = cartasInfo;
            // Leer la salida estándar y el error estándar
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Esperar a que el proceso termine
            process.WaitForExit();

            // Imprimir la salida y el error
            UnityEngine.Debug.Log("Salida del script: " + output);
            UnityEngine.Debug.Log("Error del script: " + error);
        }
    }

    public int SeleccionAlgoritmoIA(List<Carta> mano, List<Carta> descartes){
        int algoritmo = PlayerPrefs.GetInt("DificultadIA");
        

        if(algoritmo == 0)
            return MejorCarta(mano);
        else if(algoritmo == 1)
            return MonteCarlo(mano);
        else
            //return ReinforcementLearning(mano, descartes);
            AgentePhyton(mano);
        return -1;
    }

    public int SeleccionAlgoritmoIAEmpate(List<Carta> mano){
        int algoritmo = PlayerPrefs.GetInt("DificultadIA");

        if(algoritmo == 0)
            return MejorCarta(mano);
        else if(algoritmo == 1)
            return MonteCarloEmpate(mano);
        return -1;
    }   


    //Obtiene la posicion de la carta con mayor ataque de la lista
    public int MejorCarta(List<Carta> listMano){
        int id = 0, max = 0;
        for(int i = 0; i < listMano.Count; i++){
            if(!listMano[i].esHechizo && max < listMano[i].ataque){
                id = i;
                max = listMano[i].ataque;
            }
        }

        return id;
    }



    public int MonteCarlo(List<Carta> mano){
        int[] pesos = new int[mano.Count];
        int mejorPeso = 0, maxPeso = 0;

        for(int i = 0; i < mano.Count; i++){
            if(!mano[i].efecto){
                if(!mano[i].esHechizo)
                    pesos[i] = mano[i].ataque;
                else    
                    pesos[i] = -1;
            }else{
                switch(mano[i].nombreImagen){
                    case "Objeto0Roca":
                        int maxAtaque = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(mano[j].ataque > maxAtaque)
                                maxAtaque += mano[j].ataque;
                        }
                        pesos[i] = maxAtaque + 1;
                        break;

                    case "Objeto1Dinno":
                        pesos[i] = 4;
                        break;

                    case "Objeto2Serpiente":
                        pesos[i] = 6;
                        break;

                    case "Objeto3Batidora":
                        maxAtaque = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(mano[j].efecto && mano[j].ataque > maxAtaque)
                                maxAtaque += mano[j].ataque;
                        }
                        pesos[i] = maxAtaque + 4;
                        break;
                    
                    case "Objeto4Fuego":
                        int media = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(j != i)
                                media += mano[j].ataque;
                        }
                        media /= mano.Count;

                        if(media < 3)
                            pesos[i] = 8;
                        else if(media < 4)
                            pesos[i] = 7;
                        else if(media < 5)
                            pesos[i] = 6;
                        else
                            pesos[i] = 4;
                        break;            
                    
                    case "Objeto5Trampa":
                        pesos[i] = 7;
                        break;            
                    
                    case "Objeto6Estrella":
                        int numHechizos = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(mano[j].nombreImagen == "ObjetoScoreSapper" || mano[j].nombreImagen == "ObjetoScoreBooster")
                                numHechizos++;
                        }

                        if(numHechizos == 0)
                            pesos[i] = 4;
                        else if(numHechizos == 1)
                            pesos[i] = 6;
                        else
                            pesos[i] = 8;
                        break;            
                    
                    case "Objeto7Prismaticos":
                        pesos[i] = 8;
                        break;            
                    
                    case "Objeto8Planta":
                        pesos[i] = 9;
                        break;            
                    
                    case "Objeto9Motosierra":
                        pesos[i] = 10;
                        break;            
                }
            }
        }


        for(int i = 0; i < pesos.Length; i++){
            if(pesos[i] > maxPeso){
                maxPeso = pesos[i];
                mejorPeso = i;
            }
        }

        return mejorPeso;
    }


    public int MonteCarloEmpate(List<Carta> mano){
        int[] pesos = new int[mano.Count];
        int mejorPeso = 0, maxPeso = 0;

        for(int i = 0; i < mano.Count; i++){
            pesos[i] = mano[i].ataque;

            if(mano[i].efecto)
                pesos[i] -= 1;
        }

        for(int i = 0; i < pesos.Length; i++){
            if(pesos[i] > maxPeso){
                maxPeso = pesos[i];
                mejorPeso = i;
            }
        }

        return mejorPeso;
    }


    public int MonteCarloEfecto1y7(List<Carta> manoObjetivo, List<Carta> mano){
        int[] pesos = new int[manoObjetivo.Count];
        int mejorPeso = 0, maxPeso = 0;
        bool estrella = false;

        for(int i = 0; i < mano.Count; i++){
            if(mano[i].nombreImagen == "Objeto6Estrella")
                estrella = true;
        }

        for(int i = 0; i < manoObjetivo.Count; i++){
            if(!manoObjetivo[i].efecto && !manoObjetivo[i].esHechizo){
                pesos[i] = manoObjetivo[i].ataque;    
            }else{
                switch(manoObjetivo[i].nombreImagen){
                    case "Objeto0Roca":
                        pesos[i] = 4;
                        break;

                    case "Objeto1Dinno":
                        pesos[i] = 4;
                        break;

                    case "Objeto2Serpiente":
                        pesos[i] = 6;
                        break;

                    case "Objeto3Batidora":
                        pesos[i] = 6;
                        break;
                    
                    case "Objeto4Fuego":
                        int media = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(j != i)
                                media += mano[j].ataque;
                        }
                        media /= mano.Count;

                        if(media < 3)
                            pesos[i] = 8;
                        else if(media < 4)
                            pesos[i] = 7;
                        else if(media < 5)
                            pesos[i] = 6;
                        else
                            pesos[i] = 4;
                        break;            
                    
                    case "Objeto5Trampa":
                        pesos[i] = 7;
                        break;            
                    
                    case "Objeto6Estrella":
                        int numHechizos = 0;
                        for(int j = 0; j < mano.Count; j++){
                            if(mano[j].nombreImagen == "ObjetoScoreSapper" || mano[j].nombreImagen == "ObjetoScoreBooster")
                                numHechizos++;
                        }

                        if(numHechizos == 0)
                            pesos[i] = 4;
                        else if(numHechizos == 1)
                            pesos[i] = 6;
                        else
                            pesos[i] = 8;
                        break;            
                    
                    case "Objeto7Prismaticos":
                        pesos[i] = 8;
                        break;            
                    
                    case "Objeto8Planta":
                        pesos[i] = 9;
                        break;            
                    
                    case "Objeto9Motosierra":
                        pesos[i] = 10;
                        break;   
                    case "ObjetoScoreBooster":
                        if(estrella)
                            pesos[i] = 7;
                        else
                            pesos[i] = 5;
                        break;            
                    
                    case "ObjetoScoreSapper":
                        if(estrella)
                            pesos[i] = 6;
                        else
                            pesos[i] = 4;
                        break;            
                    
                    case "ObjetoScoreInversion":
                        pesos[i] = 7;
                        break;            
                    
                    case "ObjetoDisasterInsurnce":
                        pesos[i] = 7;
                        break;            
                    
                    case "ObjetoDisasterRedirect":
                        pesos[i] = 9;
                        break;
                }
            }
        }


        for(int i = 0; i < pesos.Length; i++){
            if(pesos[i] > maxPeso){
                maxPeso = pesos[i];
                mejorPeso = i;
            }
        }

        return mejorPeso;
    }


    public int ReinforcementLearning(List<Carta> mano, List<Carta> descartes){
        int[] pesos = new int[mano.Count];
        int mejorPeso = 0, maxPeso = 0;

        for(int i = 0; i < mano.Count; i++){
            if(!mano[i].efecto){
                if(!mano[i].esHechizo)
                    pesos[i] = mano[i].ataque;
                else    
                    pesos[i] = -1;
            }
        }

        for(int i = 0; i < pesos.Length; i++){
            if(pesos[i] > maxPeso){
                maxPeso = pesos[i];
                mejorPeso = i;
            }
        }

        return mejorPeso;
    }

}
