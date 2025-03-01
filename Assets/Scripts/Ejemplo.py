import rlcard
from rlcard import models
from rlcard.agents.random_agent import RandomAgent
from rlcard.utils.utils import set_global_seed
from rlcard.utils.logger import Logger

# Configuración global
set_global_seed(0)

# Configuración del entorno
env = rlcard.make('whist', config={'seed': 0, 'allow_raw_data': True})
env.set_agents([RandomAgent(env.action_space) for _ in range(4)])

# Configuración del registro
logger = Logger(log_dir='./experiments/whist_random_result/')

# Parámetros de entrenamiento
num_episodes = 1000

for episode in range(num_episodes):
    trajectories, payoffs = env.run()
    logger.log('\n########## Episode {} ##########'.format(episode))
    for i in range(env.num_agents):
        logger.log('Player {} : {}'.format(i, payoffs[i]))

logger.close_files()

# Imprimir resultados finales
for i in range(env.num_agents):
    print('Player {} average payoff: {}'.format(i, sum(logger.payoffs[i]) / num_episodes))  