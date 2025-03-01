from copy import deepcopy
import numpy as np
import random

from rlcard.games.HDL import Dealer
from rlcard.games.HDL import Player
from rlcard.games.HDL import Judger
from rlcard.games.HDL import Player
from rlcard.games.HDL import Round
from rlcard.games.HDL import Carta 
from rlcard.games.HDL import CartaDesastre

class HDLGame:

    def __init__(self, allow_step_back=False):
        ''' Initialize the class Blackjack Game
        '''
        self.allow_step_back = allow_step_back
        self.np_random = np.random.RandomState()
        self.PERDER = Carta("", 0, -1, False, False)

        self.jugador = []  # Aquí se almacenarán los jugadores
        self.Mazo = []
        self.Descartes = []
        self.mazoDesastres = []
        self.manoDelJugador = []
        self.manoRival1 = []
        self.manoRival2 = []
        self.manoRival3 = []
        self.jugadorVivo = [self.jugador[i].SigoVivo() for i in range(4)]
        self.ganadorId = []
        self.numPerdedores = 0
        self.idPerdedores = []
        self.ganadores = []
        self.descartes = 3
        self.menorIdCartaUtilizada = 0
        self.etapaDescarte = False
        self.textoAtaque = [0, 0, 0, 0]
        self.idManoCartaSeleccionadaEfectos = []

        self.cartasRonda = [self.PERDER, self.PERDER, self.PERDER, self.PERDER]
        self.efecto0Activo = False
        self.efecto5Activado = False
        self.efecto6Activado = False
        self.efectosFinRonda = False

        self.hechizos = False
        self.jugador0Sapper = False
        self.jugador0Booster = False

        self.scoreInversion = False

        self.manoEfecto = []

        self.bonusAtaque = [0, 0, 0, 0]

        self.idManoCartaSeleccionada = 0
        self.empate = False
        self.efectos = False
        self.efectoJugado = [False, False, False, False]
        self.idEfecto = []
        self.idJugadorEfecto = []
        self.idJugador = -1
        self.ganadores = []

        # Creamos los 4 jugadores con sus respectivos dinosaurios
        self.jugador.append(Player(Player.TRICERATOPS))
        self.jugador.append(Player(Player.REX))
        self.jugador.append(Player(Player.BRONTO))
        self.jugador.append(Player(Player.TRICERATOPS))



        AtaqueCartas = [0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, -1, -1, -1, -1, -1]
        CantidadCartas = [4, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 3, 2, 3, 1, 3, 0, 5, 5, 2, 3, 1]
        ImagenesCartas = ["Objeto0", "Objeto1Palo", "Objeto1Dinno", "Objeto2Sarten", "Objeto2Serpiente", "Objeto3Pala", "Objeto3Batidora", "Objeto4Pico", "Objeto4Fuego", "Objeto5Bate", "Objeto5Trampa", "Objeto6Arco", "Objeto6Estrella", "Objeto7Bicho", "Objeto7Prismaticos", "Objeto8Picos", "Objeto8Planta", "Objeto9Bazuka", "Objeto9Motosierra", "ObjetoScoreBooster", "ObjetoScoreSapper", "ObjetoScoreInversion", "ObjetoDisasterInsurnce", "ObjetoDisasterRedirect"]
        ImagenesDesastres = ["DesastreR1", "DesastreR2", "DesastreR3", "DesastreR4", "DesastreR5", "DesastreR6", "DesastreR7", "DesastreR8", "DesastreV1", "DesastreV2", "DesastreV3", "DesastreV4", "DesastreV5", "DesastreV6", "DesastreV7", "DesastreV8", "DesastreA1", "DesastreA2", "DesastreA3", "DesastreA4", "DesastreA5", "DesastreA6", "DesastreA7", "DesastreA8", "DesastreMeteorito"]
        id = 0
        color = 0
        efecto = True
        hechizo = False
        for i in range(len(AtaqueCartas)):
            for j in range(CantidadCartas[i]):
                carta = Carta(ImagenesCartas[i], AtaqueCartas[i], id, efecto, hechizo)
                self.Mazo.append(carta)
                id += 1
            if i == 18:
                efecto = False
                hechizo = True
            elif i < 18:
                efecto = not efecto
        for i in range(len(ImagenesDesastres)):
            carta = CartaDesastre(ImagenesDesastres[i], color)
            self.mazoDesastres.append(carta)
            if i == len(ImagenesDesastres) - 1:
                self.mazoDesastres.append(carta)
                self.mazoDesastres.append(carta)
            if i == 7 or i == 15 or i == 23:
                color += 1
        random.shuffle(self.Mazo)
        random.shuffle(self.mazoDesastres)

    def configure(self, game_config):
        ''' Specifiy some game specific parameters, such as number of players
        '''
        self.num_players = game_config['game_num_players']
        self.num_decks = game_config['game_num_decks']

    def init_game(self):
        ''' Initialilze the game

        Returns:
            state (dict): the first state of the game
            player_id (int): current player's id
        '''
        self.dealer = Dealer(self.np_random, self.num_decks)

        self.players = self.jugador

        self.judger = Judger(self.np_random)

        for i in range(2):
            for j in range(self.num_players):
                self.dealer.deal_card(self.players[j])
            self.dealer.deal_card(self.dealer)

        for i in range(self.num_players):
            self.players[i].status, self.players[i].score = self.judger.judge_round(self.players[i])

        self.dealer.status, self.dealer.score = self.judger.judge_round(self.dealer)

        self.winner = {'dealer': 0}
        for i in range(self.num_players):
            self.winner['player' + str(i)] = 0

        self.history = []
        self.game_pointer = 0

        return self.get_state(self.game_pointer), self.game_pointer

    def get_legal_action(self, posicion):
        legal_action = []
        mano = []
        if posicion == 0:
            mano = self.manoDelJugador
        if posicion == 1:
            mano = self.manoRival1
        if posicion == 2:
            mano = self.manoRival2
        if posicion == 3:
            mano = self.manoRival3
        if(self.jugadorVivo[posicion]):
            for i in range(mano):
                if(not mano[i].hechizo):
                    legal_action.append(mano[i])        
        return legal_action

    def step(self, action):
       self.Round.EmpezarJugada()

    def get_num_players(self):
        ''' Return the number of players in blackjack

        Returns:
            number_of_player (int): blackjack only have 1 player
        '''
        return self.num_players
    
    def get_score(self, id_player):
        return self.jugador(id_player).score

    @staticmethod
    def get_num_actions():
        ''' Return the number of applicable actions

        Returns:
            number_of_actions (int): there are only two actions (hit and stand)
        '''
        return 2

    def get_player_id(self):
        ''' Return the current player's id

        Returns:
            player_id (int): current player's id
        '''
        return self.game_pointer

    def get_state(self, player_id):
        ''' Return player's state

        Args:
            player_id (int): player id

        Returns:
            state (dict): corresponding player's state
        '''
        '''
                before change state only have two keys (action, state)
                but now have more than 4 keys (action, state, player0 hand, player1 hand, ... , dealer hand)
                Although key 'state' have duplicated information with key 'player hand' and 'dealer hand', I couldn't remove it because of other codes
                To remove it, we need to change dqn agent too in my opinion
                '''
        state = {}
        state['actions'] = ('hit', 'stand')
        hand = [card.get_index() for card in self.players[player_id].hand]
        if self.is_over():
            dealer_hand = [card.get_index() for card in self.dealer.hand]
        else:
            dealer_hand = [card.get_index() for card in self.dealer.hand[1:]]

        for i in range(self.num_players):
            state['player' + str(i) + ' hand'] = [card.get_index() for card in self.players[i].hand]
        state['dealer hand'] = dealer_hand
        state['state'] = (hand, dealer_hand)

        return state

    def is_over(self):
        ''' Check if the game is over

        Returns:
            status (bool): True/False
        '''
        '''
                I should change here because judger and self.winner is changed too
                '''
        for i in range(self.num_players):
            if self.winner['player' + str(i)] == 0:
                return False

        return True
