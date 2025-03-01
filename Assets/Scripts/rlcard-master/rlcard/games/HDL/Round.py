import random
class HDLRound:
    def __init__(self, game_instance):
        self.game_instance = game_instance

        for i in range(len(self.game_instance.jugadorVivo)):
            self.game_instance.jugadorVivo[i] = self.game_instance.jugador[i].SigoVivo()
            if self.game_instance.jugador[i].vivo and self.game_instance.jugador[i].puntuacion >= 50:
                self.game_instance.ganadorId.append(i)
        
        #TODO: QUE HACER AL FINAL DE LA PARTIDA
        self.AgregarCartasALaMano()

    
    def BuscarMano(self, id):
        if id == 0:
            return self.manoDelJugador
        elif id == 1:
            return self.manoRival1
        elif id == 2:
            return self.manoRival2
        elif id == 3:
            return self.manoRival3

    def GuardarMano(self, id, mano):
        if id == 0:
            self.manoDelJugador = mano
        elif id == 1:
            self.manoRival1 = mano
        elif id == 2:
            self.manoRival2 = mano
        elif id == 3:
            self.manoRival3 = mano

    def AgregarCartasALaMano(self):
        if not self.game_instance.efectosFinRonda:
            if self.game_instance.jugadorVivo[0]:
                self.AgregarCartas(self.manoDelJugador, 0)
            if self.game_instance.jugadorVivo[1]:
                self.AgregarCartas(self.manoRival1, 1)
            if self.game_instance.jugadorVivo[2]:
                self.AgregarCartas(self.manoRival2, 2)
            if self.game_instance.jugadorVivo[3]:
                self.AgregarCartas(self.manoRival3, 3)
        self.EmpezarJugada()

    def RellenarMazo(self, cartas):
        if cartas > len(self.game_instance.Mazo):
            random.shuffle(self.game_instance.Descartes)
            for carta in self.game_instance.Descartes:
                self.game_instance.Mazo.append(carta)
            self.Descartes.clear()

    def TieneHechizosDisaster(self, mano):
        for i in range(len(mano)):
           if mano[i].nombreImagen == "ObjetoDisasterInsurnce" or mano[i].nombreImagen == "ObjetoDisasterRedirect":
               return i
        return -1

    def AgregarCartas(self, ListMano, id):
        cartasFaltantes = 5 - len(ListMano)
        self.RellenarMazo(cartasFaltantes)
        for _ in range(cartasFaltantes):
            indiceAleatorio = random.randint(0, len(self.Mazo) - 1)
            cartaAleatoria = self.game_instance.Mazo[indiceAleatorio]
            cartaAleatoria.SetJugador(id)
            ListMano.append(cartaAleatoria)
            self.game_instance.Mazo.pop(indiceAleatorio)
        #self.manoDelJugador[1] = self.PRUEBA2
        return cartasFaltantes
    
    def DeshabilitarCartaEfecto0(self):
        if len(self.idManoCartaSeleccionadaEfectos) > 0:
            self.game_instance.bonusAtaque[0] -= self.manoDelJugador[self.idManoCartaSeleccionadaEfectos[0]].ataque
            self.idManoCartaSeleccionadaEfectos.pop()


    def AcabarRonda(self, cartaRonda):
        self.game_instance.efecto6Activado = False
        perdedor = -1
        if self.game_instance.numPerdedores == 1:
            perdedor = self.game_instance.idPerdedores.index(True)
            self.game_instance.jugador[perdedor].cartasDesastres.append(self.game_instance.mazoDesastres[0].nombreImagen)
            for i in range(4):
                ataque = self.game_instance.cartasRonda[i].ataque + self.game_instance.bonusAtaque[i]
                if self.game_instance.ganadores[i] and not self.game_instance.empate:
                    self.game_instance.jugador[i].puntuacion += ataque
                self.game_instance.jugador[i].puntuacion += len(self.game_instance.jugador[i].cartasDesastres)
        for i in range(len(cartaRonda)):
            if i < len(cartaRonda) and self.game_instance.jugadorVivo[i]:
                self.game_instance.Descartes.append(cartaRonda[i])
        self.game_instance.efectos = False
        if self.game_instance.numPerdedores == 1:
            mano = self.BuscarMano(perdedor)
            posicion = self.TieneHechizosDisaster(mano)
            ultimaCarta = len(self.game_instance.jugador[perdedor].cartasDesastres) - 1
            if posicion != -1 and mano[posicion].nombreImagen == "ObjetoDisasterInsurnce":
                self.game_instance.jugador[perdedor].cartasDesastres.pop(ultimaCarta)
                self.game_instance.jugador[perdedor].puntuacion -= 1
            elif posicion != -1 and mano[posicion].nombreImagen == "ObjetoDisasterRedirect":
                self.DarCartaDesastre(perdedor, -1)
                self.game_instance.jugador[perdedor].puntuacion -= 1
            self.Descartar_carta(perdedor)

        for i in range(len(self.game_instance.efectoJugado)):
            self.game_instance.efectoJugado[i] = False
        
        self.game_instance.descartes = 3
        self.game_instance.menorIdCartaUtilizada = 0
        self.game_instance.efecto5Activado = False
        self.AcabarRondaEfectos()
        self.SiguienteRonda()

    def Descartar_carta(self, perdedor):
        mano = self.buscar_mano(perdedor)
        min_ataque = 100
        id_carta = -1

        for i, carta in enumerate(mano):
            if carta.ataque < min_ataque:
                min_ataque = carta.ataque
                id_carta = i

        if id_carta != -1:
            mano.pop(id_carta)
            self.GuardarMano(perdedor, mano)

    def DarCartaDesastre(self, id, objetivo):
        
        ultima_carta = len(self.game_instance.jugador[id].cartas_desastres) - 1

        # Cambiar el objetivo si el id no es cero
        if id != 0:
            objetivo = random.randint(0, 1)
            if objetivo >= id:
                objetivo += 1

        # Mover la última carta desastre del jugador a otro jugador
        self.game_instance.jugador[objetivo].cartas_desastres.append(self.game_instance.jugador[id].cartas_desastres[ultima_carta])
        del self.game_instance.jugador[id].cartas_desastres[ultima_carta]

        # Incrementar la puntuación del jugador objetivo
        self.game_instance.jugador[objetivo].puntuacion += 1


    def SiguienteRonda(self):
        perdedor = -1
        if self.game_instance.numPerdedores == 1:
            perdedor = self.game_instance.idPerdedores.index(True)
        if self.game_instance.idEfecto == [] and (self.game_instance.numPerdedores == 1 or self.game_instance.numPerdedores == 4):
            self.game_instance.mazoDesastres.pop(0)
            self.game_instance.idManoCartaSeleccionadaEfectos.clear()
            self.game_instance.etapaDescarte = True

    def SiguienteRonda(self):
        if len(self.game_instance.id_efecto) == 0 and (self.game_instance.num_perdedores == 1 or self.game_instance.num_perdedores == 4):
            self.game_instance.mazo_desastres.pop(0)
            self.game_instance.idManoCartaSeleccionadaEfectos.clear()
        elif len(self.game_instance.id_efecto) == 0 and self.game_instance.num_perdedores > 1:
            self.game_instance.empate = True  
            self.EmpezarJugada()

    def AcabarRondaEfectos(self):
        while self.game_instance.idEfecto != []:
            i = len(self.game_instance.idJugadorEfecto) - 1
            if self.game_instance.idEfecto[i] == 1:
                self.Efecto1IA()
                self.game_instance.idJugadorEfecto.pop(i)
                self.game_instance.idEfecto.pop(i)

    def Efecto1IA(self):
        idCarta = -1
        jugadorId = -1
        while True:
            jugadorId = random.randint(0, 2)
            if jugadorId >= self.game_instance.idJugadorEfecto[0]:
                jugadorId += 1
            if self.game_instance.jugadorVivo[jugadorId]:
                break
        mano = self.BuscarMano(self.game_instance.idJugador)
        self.game_instance.manoEfecto = self.BuscarMano(jugadorId)
        idCarta = random.randint(0, len(self.game_instance.manoEfecto))
        carta = self.game_instance.manoEfecto[idCarta]
        carta.SetJugador(jugadorId)
        mano.append(carta)
        self.game_instance.manoEfecto.pop(idCarta)
        self.GuardarMano(jugadorId, self.game_instance.manoEfecto)
        self.GuardarMano(self.game_instance.idJugador, mano)
        self.game_instance.efectosFinRonda = False
            

    def ObtenerGanadores(self):
        ataqueRonda = [0, 0, 0, 0]
        maxAtaque = self.game_instance.cartasRonda[0].ataque
        minAtaque = 100
        self.game_instance.numPerdedores = 0
        for i in range(len(self.game_instance.cartasRonda)):
            if self.game_instance.jugadorVivo[i] and (not self.game_instance.empate or self.game_instance.idPerdedores[i]):
                ataque = self.game_instance.cartasRonda[i].ataque + self.game_instance.bonusAtaque[i]
                ataqueRonda[i] = ataque
                if ataque > maxAtaque:
                    maxAtaque = ataque
                if ataque < minAtaque:
                    minAtaque = ataque
        if (self.game_instance.efecto5Activado == True and not self.game_instance.scoreInversion or
            self.game_instance.efecto5Activado == False and self.game_instance.scoreInversion):
            for i in range(len(ataqueRonda)):
                if self.game_instance.jugadorVivo[i]:
                    if minAtaque == ataqueRonda[i]:
                        self.game_instance.textoAtaque[i] = str(maxAtaque)   
                    elif maxAtaque == ataqueRonda[i]:
                        self.game_instance.textoAtaque[i] = str(minAtaque)
        for i in range(len(self.ganadores)):
            if self.game_instance.jugadorVivo[i] and (not self.game_instance.empate or self.game_instance.idPerdedores[i]):
                ataque = int(self.textoAtaque[i])
                self.game_instance.ganadores[i] = (ataque == maxAtaque)
                if minAtaque == ataque:
                    self.game_instance.numPerdedores += 1
                    self.game_instance.idPerdedores[i] = True
                else:
                    self.game_instance.idPerdedores[i] = False


    def ActualizarAtaques(self):
        for i in range(len(self.game_instance.textoAtaque)):
            if self.game_instance.jugadorVivo[i]:
                if not self.game_instance.empate:
                    self.game_instance.textoAtaque[i] = self.game_instance.cartasRonda[i].ataque + self.game_instance.bonusAtaque[i]
                elif self.idPerdedores[i]:
                    self.game_instance.textoAtaque[i] = self.game_instance.cartasRonda[i].ataque


    def EmpezarJugada(self):
        if self.game_instance.etapaDescarte:
            #TODO: revisar que hacer aqui
            return
        if self.game_instance.hechizos:
            self.Hechizos(1)
            return
        if (self.game_instance.empate and not self.game_instance.idPerdedores[0] and not self.game_instance.efectosFinRonda):
            if not self.game_instance.efectos:
                if not self.game_instance.empate:
                    idCarta = 0
                    
                    if self.game_instance.jugadorVivo[0]:
                        #state['actions'] = self.game_instance.manoDelJugador 
                        idCarta = self.mejor_carta(self.game_instance.manoDelJugador)
                        self.game_instance.cartasRonda[0] = self.game_instance.manoDelJugador[idCarta]
                        self.game_instance.manoDelJugador.pop(idCarta)
                    else:
                        self.game_instance.cartasRonda[0] = self.game_instance.PERDER
                    
                    
                    if self.jugadorVivo[1]:
                        idCarta = self.mejor_carta(self.game_instance.manoRival1)
                        self.game_instance.cartasRonda[1] = self.game_instance.manoRival1[idCarta]
                        self.game_instance.manoRival1.pop(idCarta)
                    else:
                        self.game_instance.cartasRonda[1] = self.game_instance.PERDER
                    if self.jugadorVivo[2]:
                        idCarta = self.mejor_carta(self.game_instance.manoRival2)
                        self.game_instance.cartasRonda[2] = self.game_instance.manoRival2[idCarta]
                        self.game_instance.manoRival2.pop(idCarta)
                    else:
                        self.game_instance.cartasRonda[2] = self.game_instance.PERDER
                    if self.jugadorVivo[3]:
                        idCarta = self.mejor_carta(self.game_instance.manoRival3)
                        self.game_instance.cartasRonda[3] = self.game_instance.manoRival3[idCarta]
                        self.game_instance.manoRival3.pop(idCarta)
                    else:
                        self.game_instance.cartasRonda[3] = self.game_instance.PERDER
                    for i in range(len(self.game_instance.jugador)):
                        if self.game_instance.jugadorVivo[i]:
                            self.game_instance.bonusAtaque[i] += self.game_instance.jugador[i].colorBonus[self.game_instance.mazoDesastres[0].color]
                    for i in range(len(self.efectoJugado)):
                        self.game_instance.efectoJugado[i] = False
                else:
                    self.JugadaEmpate()
                    return
            else:
                if self.game_instance.efecto0Activo and len(self.game_instance.idManoCartaSeleccionadaEfectos) > 0:
                    self.game_instance.manoDelJugador.pop(self.game_instance.idManoCartaSeleccionadaEfectos[0])
            self.ActualizarAtaques()
            self.Efectos()
            self.game_instance.hechizos = True
            if self.game_instance.jugador[0].TieneHechizosScore(self.game_instance.manoDelJugador):
                return self.Hechizos(0)
            else:
                return self.Hechizos(1)
        return
    
    def mejor_carta(self, list_mano):
        id = 0
        max_ataque = 0
        for i, carta in enumerate(list_mano):
            if not carta.es_hechizo and carta.ataque > max_ataque:
                id = i
                max_ataque = carta.ataque
        return id


    
    def Hechizos(self, id):
        hehcizoJugado = False
        mano = self.game_instance.BuscarMano(id)
        if self.game_instance.jugadorVivo[id] and self.game_instance.jugador[i].TieneHechizosScore(mano):
            booster = 0
            sapper = 0
            inversion = 0
            posicionId = []
            posicionAtaque = []
            self.ObtenerPosiciones(posicionId, posicionAtaque)
            for i in range(len(mano)):
                if mano[i].nombreImagen == "ObjetoScoreBooster":
                    booster += 1
                elif mano[i].nombreImagen == "ObjetoScoreSapper":
                    sapper += 1
                elif mano[i].nombreImagen == "ObjetoScoreInversion":
                    inversion += 1
            intercambio = self.game_instance.efecto5Activado != self.game_instance.scoreInversion
            if inversion > 0 and ((posicionId[0] == id and not self.game_instance.efecto5Activado) or posicionId[3] == id and self.game_instance.efecto5Activado):
                self.ScoreInversion(id, mano)
                hehcizoJugado = True
            elif (booster > 0 or sapper > 0) and ((posicionId[len(posicionId)-1] != id and not intercambio) or (posicionId[0] != id and intercambio)):
                diferenciaNoPerder = -1
                diferenciaGanar = -1
                noPerder = -1
                ganar = -1
                posicion = posicionId.index(id)
                distancia = 3 + 2 * (1 if self.game_instance.efecto6Activado else 0)
                if posicion == 0 and not intercambio and posicionAtaque[0] != posicionAtaque[1]:
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[1]
                    noPerder = 1
                elif posicion == 0 and not intercambio and posicionAtaque[0] != posicionAtaque[2]:
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[2]
                    noPerder = 2
                elif posicion == 0 and not intercambio and posicionAtaque[0] != posicionAtaque[3]:
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[3]
                    noPerder = 3
                elif posicion == 3 and intercambio and posicionAtaque[3] != posicionAtaque[2]:
                    diferenciaNoPerder = posicionAtaque[2] - posicionAtaque[3]
                    noPerder = 2
                elif posicion == 3 and intercambio and posicionAtaque[3] != posicionAtaque[1]:
                    diferenciaNoPerder = posicionAtaque[1] - posicionAtaque[3]
                    noPerder = 1
                elif posicion == 3 and intercambio and posicionAtaque[3] != posicionAtaque[0]:
                    diferenciaNoPerder = posicionAtaque[0] - posicionAtaque[3]
                    noPerder = 0
                if posicion != 3 and not intercambio:
                    diferenciaGanar = posicionAtaque[posicion] - posicionAtaque[3]
                    ganar = 3
                elif posicion != 0 and intercambio:
                    diferenciaGanar = posicionAtaque[0] - posicionAtaque[posicion]
                    ganar = 0
                if (diferenciaNoPerder > -1 and diferenciaNoPerder < distancia) or (diferenciaGanar > -1 and diferenciaGanar < distancia):
                    if booster > 0:
                        if not intercambio:
                            self.ScoreBooster(id, id, mano)
                        elif posicion == 3:
                            self.ScoreBooster(id, noPerder, mano)
                        else:
                            self.ScoreBooster(id, ganar, mano)
                        hehcizoJugado = True
                    elif sapper > 0:
                        if intercambio:
                            self.ScoreSapper(id, id, mano)
                        elif posicion == 3:
                            self.ScoreSapper(id, noPerder, mano)
                        elif diferenciaGanar > -1:
                            self.ScoreSapper(id, ganar, mano)
                        hehcizoJugado = True
        if hehcizoJugado:
            self.Hechizos(0)
            return
        
        for i in range(id, 3):
            if self.game_instance.jugadorVivo[i+1]:
                self.Hechizos(i+1)
                return
        self.hechizos = False
        self.ObtenerGanadores()
        self.AcabarRonda() #todo
    
    def ScoreBooster(self, id, idObjetivo, mano):
        self.bonusAtaque[idObjetivo] += 2 + 2 * (1 if self.game_instance.efecto6Activado else 0)
        self.ActualizarAtaques()
        idCartaMano = self.ObtenerIdCartaManoPorNombre("ObjetoScoreBooster", mano)
        mano.pop(idCartaMano)
        self.GuardarMano(id, mano)

    def ScoreSapper(self, id, idObjetivo, mano):
        self.bonusAtaque[idObjetivo] -= 2 + 2 * (1 if self.game_instance.efecto6Activado else 0)
        self.ActualizarAtaques()
        idCartaMano = self.ObtenerIdCartaManoPorNombre("ObjetoScoreSapper", mano)
        mano.pop(idCartaMano)
        self.GuardarMano(id, mano)

    def ScoreInversion(self, id, mano):
        self.game_instance.scoreInversion = not self.game_instance.scoreInversion
        self.ActualizarAtaques()
        idCartaMano = self.ObtenerIdCartaManoPorNombre("ObjetoScoreInversion", mano)
        mano.pop(idCartaMano)
        self.GuardarMano(id, mano)

    def ObtenerPosiciones(self, posiciones, enterosOrdenados):
        listaEnteros = []
        for i in range(len(self.game_instance.textoAtaque)):
            if self.game_instance.jugadorVivo[i]:
                ataque = self.game_instance.textoAtaque[i]
                listaEnteros.append([ataque, i])
        listaEnteros.sort(key=lambda x: x[0])
        posiciones = [0] * len(listaEnteros)
        enterosOrdenados = [0] * len(listaEnteros)
        for i in range(len(listaEnteros)):
            posiciones[i] = listaEnteros[i][1]
            enterosOrdenados[i] = listaEnteros[i][0]

    def ObtenerIdCartaManoPorNombre(self, nombre, mano):
        for i in range(len(mano)):
            if mano[i].nombreImagen == nombre:
                return i
        return -1
    
    def JugadaEmpate(self):
        for i in range(4):
            if self.game_instance.idPerdedores[i]:
                    mano = self.BuscarMano(i)
                    idCarta = self.mejor_carta(mano)
                    self.game_instance.cartasRonda[i] = mano[idCarta]
                    mano.pop(idCarta)
                    self.GuardarMano(i, mano)
            else:
                self.game_instance.cartasRonda[i] = self.game_instance.PERDER
        self.ActualizarAtaques()
        self.ObtenerGanadores()
        self.AcabarRonda()

    def Efectos(self):
        cartasArray = sorted(self.game_instance.cartasRonda, key=lambda carta: carta.ataque)
        cartasOrdenadas = list(cartasArray)
        jugado = 0
        count = 0
        motosierra = False
        self.game_instance.efectos = True
        for i in range(len(self.game_instance.efectoJugado)):
            if self.game_instance.efectoJugado[i]:
                jugado += 1
        for i in range(len(self.game_instance.cartasRonda)):
            if self.game_instance.cartasRonda[i].nombreImagen == "Objeto9Motosierra":
                jugado = 4
                motosierra = True
        if not motosierra:
            for i in range(len(self.game_instance.efectoJugado)):
                if self.game_instance.cartasRonda[i].nombreImagen == "Objeto5Trampa":
                    self.game_instance.efecto5Activado = True
        for i in range(jugado, len(cartasOrdenadas)):
            self.game_instance.efectoJugado[i] = True
            id = cartasOrdenadas[i].idJugador
            mano = self.BuscarMano(id)
            if cartasOrdenadas[i].nombreImagen == "Objeto0":
                count = 0
                for carta in mano:
                    if not carta.esHechizo:
                        count += 1
                if count > 0:
                    self.Efecto0y3IA(id, mano, 0)
            elif cartasOrdenadas[i].nombreImagen == "Objeto1Dinno":
                self.game_instance.efectosFinRonda = True
                self.game_instance.idJugador = id
                self.game_instance.idJugadorEfecto.append(self.game_instance.idJugador)
                self.game_instance.idEfecto.append(1)
            elif cartasOrdenadas[i].nombreImagen == "Objeto2Serpiente":
                    self.Efecto2IA(id)
            elif cartasOrdenadas[i].nombreImagen == "Objeto3Batidora":
                count = 0
                for carta in mano:
                    if carta.efecto:
                        count += 1
                if count > 0:
                    self.Efecto0y3IA(id, mano, 3)
            elif cartasOrdenadas[i].nombreImagen == "Objeto4Fuego":
                descartes = 3
                for j in range(len(mano)-1, -1, -1):
                    if descartes > 0 and mano[j].ataque < 6:
                        mano.pop(j)
                        descartes -= 1
                self.GuardarMano(id, mano)
            elif cartasOrdenadas[i].nombreImagen == "Objeto6Estrella":
                self.game_instance.efecto6Activado = True
            elif cartasOrdenadas[i].nombreImagen == "Objeto7Prismaticos":
                self.PrepararEfecto7(id, mano)
            elif cartasOrdenadas[i].nombreImagen == "Objeto8Planta":
                self.Efecto8IA(id)
            else:
                pass

    def Efecto0y3IA(self, id, mano, efecto):
        if(not self.game_instance.efecto5Activado):
            idCarta = self.ObtenerIdCartaEfecto0y3IA(id, mano, efecto) 
            self.game_instance.bonusAtaque[id] += mano[idCarta].ataque
            mano.pop(idCarta)
            self.GuardarMano(id, mano)
            self.ActualizarAtaques()

    def ObtenerIdCartaEfecto0y3IA(self, id, mano, efecto):
        ataqueMin = 100
        idCarta = -1
        ataqueMax = 0
        for j in range(4):
            if j != id:
                ataque = int(self.game_instance.textoAtaque[j])
                if ataqueMin > ataque:
                    ataqueMin = ataque
                if ataqueMax < ataque:
                    ataqueMax = ataque

        if(self.game_instance.efecto5Activado):
            minimo = 100
            for j in range(len(mano)):
                if(efecto == 0 or (efecto == 3 and mano[j].efecto)) and minimo > mano[j].ataque:
                    idCarta = j
                    minimo = mano[j].ataque
        else:
            for j in range(len(mano)):
                if (efecto == 0 or (efecto == 3 and mano[j].efecto)) and ((self.game_instance.cartasRonda[id].ataque + mano[j].ataque + self.game_instance.bonusAtaque[id]) >= ataqueMax and not self.game_instance.efecto5Activado):
                    idCarta = j
                    break
            if idCarta == -1:
                for j in range(len(mano)):
                    if (efecto == 0 or (efecto == 3 and mano[j].efecto)) and not mano[j].esHechizo and (self.game_instance.cartasRonda[id].ataque + mano[j].ataque + self.game_instance.bonusAtaque[id]) > ataqueMin:
                        idCarta = j
                        break
                if idCarta == -1:
                    idCarta =  random.randint(0, len(mano))
                    while mano[idCarta].esHechizo and (efecto == 0 or efecto == 3 and mano[idCarta].efecto):
                        idCarta =  random.randint(0, len(mano))
        return idCarta
    
    def Efecto2IA(self, id):
        robo1 = id - 1
        robo2 = id + 1
        if id == 3:
            robo2 = 0
        elif id == 0:
            robo1 = 3
        if self.game_instance.cartasRonda[robo1].efecto or self.game_instance.cartasRonda[robo2].efecto:
            datos = [
                (self.game_instance.cartasRonda[robo1].ataque, robo1, self.game_instance.cartasRonda[robo1].efecto),
                (self.game_instance.cartasRonda[robo2].ataque, robo2, self.game_instance.cartasRonda[robo2].efecto),
                (self.game_instance.cartasRonda[id].ataque, id, True)
            ]
            
            datosOrdenados = sorted(filter(lambda d: d[2], datos), key=lambda d: d[0], reverse=True)
            idOrdenado = [d[1] for d in datosOrdenados]

            if ((idOrdenado[0] == robo1 or idOrdenado[0] == robo2) and not self.game_instance.efecto5Activado) or ((idOrdenado[len(idOrdenado)-1] == robo1 or idOrdenado[len(idOrdenado)-1] == robo2) and not self.game_instance.efecto5Activado):
                posicion2 = idOrdenado[0] if self.game_instance.efecto5Activado else idOrdenado[-1]
                posicion1 = id

                carta = self.game_instance.cartasRonda[posicion1]
                self.game_instance.cartasRonda[posicion1] = self.game_instance.cartasRonda[posicion2]
                self.game_instance.cartasRonda[posicion2] = carta
                self.ActualizarAtaques()
                
    def PrepararEfecto7(self, id, mano):
        self.RellenarMazo(2)
        cartaEfecto7 = []
        cartaEfecto7.append(self.game_instance.Mazo[0])
        cartaEfecto7.append(self.game_instance.Mazo[1])
        self.game_instance.Mazo.pop(0)
        self.game_instance.Mazo.pop(0)
        
        if not cartaEfecto7[1].esHechizo or cartaEfecto7[0].ataque > cartaEfecto7[1].ataque:
            mano.append(cartaEfecto7[0])
        else:
            mano.append(cartaEfecto7[1])
        mano[len(mano)-1].SetJugador(id)
        self.GuardarMano(id, mano)


    def Efecto8IA(self, id):
        max = 0
        maxId = -1
        min = 20
        minId = -1
        j1 = 0
        j2 = 0
        for j in range(4):
            if self.game_instance.jugadorVivo[j] and self.game_instance.cartasRonda[j].ataque > max:
                max = self.game_instance.cartasRonda[j].ataque
                maxId = j
            if self.game_instance.jugadorVivo[j] and self.game_instance.cartasRonda[j].ataque < min:
                min = self.game_instance.cartasRonda[j].ataque
                minId = j
        if (self.game_instance.cartasRonda[id].ataque == max and not self.game_instance.efecto5Activado) or (self.game_instance.cartasRonda[id].ataque == min and self.game_instance.efecto5Activado):
            j1 = random.randint(0, 4)
            while j1 != id and self.game_instance.jugadorVivo[j1]:
                j1 = random.randint(0, 4)
            if self.game_instance.jugadorVivo.count(True) == 2:
                j2 = id
            else:
                j2 = random.randint(0, 4)
                while j1 != id and j1 != j2 and self.game_instance.jugadorVivo[j2]:
                    j2 = random.randint(0, 4)
            posicion1 = j1
            posicion2 = j2
            carta = self.game_instance.cartasRonda[posicion1]
            self.game_instance.cartasRonda[posicion1] = self.game_instance.cartasRonda[posicion2]
            self.game_instance.cartasRonda[posicion2] = carta
        else:
            if not self.game_instance.efecto5Activado:
                posicion2 = maxId
            else:
                posicion2 = minId
            posicion1 = id
            carta = self.game_instance.cartasRonda[posicion1]
            self.game_instance.cartasRonda[posicion1] = self.game_instance.cartasRonda[posicion2]
            self.game_instance.cartasRonda[posicion2] = carta