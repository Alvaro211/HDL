
class HDLPlayer:

    TRICERATOPS = 0
    REX = 1
    BRONTO = 2
    STEGO = 3

    def __init__(self, dinosaurio):
        self.dinosaurio = dinosaurio
        self.puntuacion = 0
        self.cartasDesastres = [0, 0, 0, 0, 0]
        self.vivo = True

        self.colorBonus = [0, 0, 0, 0]
        if self.dinosaurio == HDLPlayer.TRICERATOPS:
            self.colorBonus[2] = 1
            self.colorBonus[1] = -1
        elif self.dinosaurio == HDLPlayer.REX:
            self.colorBonus[0] = 1
            self.colorBonus[2] = -1
        elif self.dinosaurio == HDLPlayer.BRONTO:
            self.colorBonus[1] = 1
            self.colorBonus[0] = -1
        elif self.dinosaurio == HDLPlayer.STEGO:
            self.colorBonus[3] = 1

    def SigoVivo(self):
        azul = 0
        rojo = 0
        verde = 0
        comodin = 0
        for carta in self.cartasDesastres:
            if carta[8] == 'A':
                azul += 1
            if carta[8] == 'R':
                rojo += 1
            if carta[8] == 'V':
                verde += 1
            if carta[8] == 'M':
                comodin += 1
        if (azul > 2 or rojo > 2 or verde > 2 or
                (azul > 0 and rojo > 0 and verde > 0) or
                ((azul > 1 or rojo > 1 or verde > 1) and comodin > 0) or
                comodin > 2 or
                ((azul > 0 or rojo > 0 or verde > 0) and comodin > 1) or
                (azul > 0 and rojo > 0 and comodin > 0) or
                (azul > 0 and verde > 0 and comodin > 0) or
                (rojo > 0 and verde > 0 and comodin > 0)):
            self.vivo = False
            return False
        return True

    def TieneHechizosScore(self, mano):
        for carta in mano:
            if carta.nombreImagen == "ObjetoScoreBooster" or carta.nombreImagen == "ObjetoScoreSapper" or carta.nombreImagen == "ObjetoScoreInversion":
                return True
        return False