import { createMachine, interpret } from 'xstate';
import { assign } from 'xstate/lib/actionTypes';

const initialContext = {
    questions: [
        { text: 'Rot oder schwarz?' }
    ],
    activeQuestion: -1,
    players: [],
    activePlayer: -1,
    round: 1
};

const areEnoughPlayers = context => context.players.length >= 2;
const isQuestionLeft = context => context.activeQuestion < (context.questions.length - 1);
const noQuestionsLeft = context => !isQuestionLeft(context);
const isPlayerLeft = context => context.activePlayer < (context.players.length - 1);

const addPlayer = assign({
    players: (context, event) => [...context.players, 'test']
});

const selectQuestion = context => callback => {
    const result = isQuestionLeft(context) ? 'SUCCESS' : 'NO_MORE_QUESTIONS';
    context.activePlayer = -1;
    context.activeQuestion++;

    callback(result);
};

const selectPlayer = context => callback => {
    const result = isPlayerLeft(context) ?  'SUCCESS' : 'NO_PLAYER_LEFT';
    context.activePlayer++;
    callback(result);
};

const gameMachine = createMachine({
    id: 'Busfoan',
    initial: 'idle',
    context: initialContext,
    states: {
        idle: {
            on: {
                START: 'wait-for-players',
                // END: 'end'
            }
        },
        'wait-for-players': {
            on: {
                JOIN: {
                    target: 'wait-for-players',
                    actions: [addPlayer]
                },
                START: {
                    target: 'question',
                    cond: areEnoughPlayers
                },
                // END: 'end'
            }
        },
        'question': {
            initial: 'question-selection',
            on: {
                NO_MORE_QUESTIONS: 'end'
            },
            states: {
                'question-selection': {
                    invoke: {
                        id: 'select-question',
                        src: selectQuestion
                    },
                    on: {
                        SUCCESS: 'player-selection'
                    }
                },
                'player-selection': {
                    invoke: {
                        id: 'select-player',
                        src: selectPlayer
                    },
                    on: {
                        SUCCESS: {
                            target: 'wait-for-answer',
                            actions: ['ask-question'],
                        },
                        NO_PLAYER_LEFT: {
                            target: 'question-selection',
                            actions: ['reset-active-player']
                        }
                    }
                },
                'wait-for-answer': {
                    on: {
                        CHECK: 'check-answer'
                    }
                },
                'check-answer': {
                    invoke: {
                        id: 'check-answer',
                        src: context => callback => {
                            // TODO
                            callback('NEXT');
                        }
                    },
                    on: {
                        NEXT: 'player-selection'
                    }
                }
            }
        },
        end: {
            type: 'final'
        }
    }
});

export default interpret(gameMachine)
    .onTransition((state) => console.log(state.value))
    .start();