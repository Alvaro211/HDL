
class HDLCarta:
    def __init__(self, nombreImagen, ataque, id, efecto, hechizo):
        self.nombreImagen = nombreImagen
        self.ataque = ataque
        self.id = id
        self.efecto = efecto
        self.hechizo = hechizo

    def SetJugador(self, id):
        self.id = id  