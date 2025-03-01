import numpy as np

import rlcard
from rlcard.models.model import Model


class HDLRuleAgentV1(object):
    ''' UNO Rule agent version 1
    '''

    def __init__(self):
        self.use_raw = False

    def step(self, state):
        ''' Predict the action given raw state. A naive rule. Choose the color
            that appears least in the hand from legal actions. Try to keep wild
            cards as long as it can.

        Args:
            state (dict): Raw state from the game

        Returns:
            action (str): Predicted action
        '''

        legal_actions = state['raw_legal_actions']
        state = state['raw_obs']

        action = np.random.choice(legal_actions)
        return action

    def eval_step(self, state):
        ''' Step for evaluation. The same to step
        '''
        return self.step(state), []
    

class Model(object):
    ''' The base model class
    '''

    def __init__(self):
        env = rlcard.make('HDL')

        rule_agent = HDLRuleAgentV1()
        self.rule_agents = [rule_agent for _ in range(env.num_players)]

    def eval_step(self, state):
        ''' Step for evaluation. The same to step
        '''
        return self.step(state), []

    @property
    def agents(self):
        ''' Get a list of agents for each position in a the game

        Returns:
            agents (list): A list of agents

        Note: Each agent should be just like RL agent with step and eval_step
              functioning well.
        '''
        return self.rule_agents