const eris = require('eris');
const config =  require('./config.json');
const stateMaschine = require('./state.js');

var bot = new eris.Client(config.token);

bot.on("ready", () => {
    console.log("Ready!");
});
bot.on("messageCreate", (msg) => {

    if (msg.bot) return;

    // console.log("* \t" + msg.content)
    // console.log("  \t" + JSON.stringify(msg) + "\n");
    // console.log(bus)

    if (msg.content === "!goodbot") {
        bot.createMessage(msg.channel.id, "I am a good bot and you're a good human @" + msg.member.username + "#" + msg.member.discriminator);
    }

    if (msg.content === "!badbot") {
        bot.createMessage(msg.channel.id, "I try my best ...");
    }

    if (msg.content === '!busfoan') {
        var r = stateMaschine.send('START', { bot, msg });
    }

    if (msg.content === '!einsteigen') {
        stateMaschine.send('JOIN', { bot, msg });
    }

    if (msg.content === '!abfoat') {
        stateMaschine.send('START', { bot, msg });
    }

    if (msg.content === "!endstation") {
        // bus.isStarted = false;
        // bus.members = [];
        // bot.createMessage(msg.channel.id, "Woa a coole foaht. Bis boid");
    }
});

bot.connect();