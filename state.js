const { createMachine, interpret } = require('xstate');
const { assign } = require('xstate/lib');

const initialContext = {
    questions: [
        { text: 'Rot [1] oder Schwarz [2]?' }
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

const addPlayer = (context, { bot, msg }) => {
    context.players.push({ name: msg.member.username });

    if (context.players.length === 1) {
        bot.createMessage(msg.channel.id, "Du bist alleine im bus");
    } else {
        bot.createMessage(msg.channel.id, "Leiwaund. Im bus san scho " + context.players.length + " leit");
    }
};

const selectQuestion = context => callback => {
    const result = isQuestionLeft(context) ? 'SUCCESS' : 'NO_MORE_QUESTIONS';
    context.activePlayer = -1;
    context.activeQuestion++;

    callback(result);
};

const welcomeMessage = (context, { bot, msg }) => {
    bot.createMessage(msg.channel.id, "Gscheid busfoan! Wer will einsteigen? (Tippe !einsteigen)");
};

const selectPlayer = context => callback => {
    const result = isPlayerLeft(context) ?  'SUCCESS' : 'NO_PLAYER_LEFT';
    context.activePlayer++;
    callback(result);
};

const startQuestions = (context, { bot, msg }) => {
    bot.createMessage(msg.channel.id, "Da bus startet mit " + context.players.length + " leiwaunde leit.");
};

const askQuestion = (context, { bot, msg }) => {
    console.log("ASK QUESTION");
    const question = context.questions[context.activeQuestion];
    bot.createMachine(msg.channel.id, question.text);
};

const gameMachine = createMachine({
    id: 'Busfoan',
    initial: 'idle',
    context: initialContext,
    states: {
        idle: {
            on: {
                START: {
                    target: 'wait-for-players',
                    actions: [welcomeMessage]
                }
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
                    actions: [startQuestions],
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
                            actions: [askQuestion],
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

module.exports = interpret(gameMachine)
    // .onTransition((state) => console.log(state.value))
    .start();