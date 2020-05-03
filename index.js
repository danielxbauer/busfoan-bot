const eris = require('eris');

var bot = new eris.Client("NzA2NDk3MjMwMDA1MjA3MDQx.Xq7M5Q.c7b3KkWWJI2wj_9cZC2hoqap1pU");

var bus = {
    isStarted: false,
    members: [],
    active: null,
    rightChoice: null
};

bot.on("ready", () => {
    console.log("Ready!");
});
bot.on("messageCreate", (msg) => {

    if (msg.bot) return;

    console.log("* \t" + msg.content)
    console.log("  \t" + JSON.stringify(msg) + "\n");
    console.log(bus)

    if (msg.content === "!goodbot") {
        bot.createMessage(msg.channel.id, "I am a good bot and you're a good human @" + msg.member.username + "#" + msg.member.discriminator);
    }

    if (msg.content === "!badbot") {
        bot.createMessage(msg.channel.id, "I try my best ...");
    }

    if (msg.content === '!busfoan') {
        if (!bus.isStarted) {
            bot.createMessage(msg.channel.id, "Gscheid busfoan! Wer will einsteigen? (Tippe !einsteigen)");
            bus.isStarted = true;
        } else {
            bot.createMessage(msg.channel.id, "Da bus is scho augfoan. (!endstation zum beenden da rundn)");
        }
    }

    if (msg.content === '!einsteigen') {
        bus.members.push(msg.member.username);
        if (bus.members.length === 1) {
            bot.createMessage(msg.channel.id, "Du bist alleine im bus");
        } else {
            bot.createMessage(msg.channel.id, "Leiwaund. Im bus san scho " + bus.members.length + " leit");
        }
    }

    if (msg.content === '!abfoat') {
        bot.createMessage(msg.channel.id, "Da bus startet mit " + bus.members.length + " leiwaunde leit.");

        bus.active = bus.members[0];
        bus.rightChoice = 1;
        bot.createMessage(msg.channel.id, "Rot [1] oder schwarz [2]? @" + bus.active);
        return;
    }

    if (msg.author.username === bus.active && parseInt(msg.content) !== Number.NaN && bus.rightChoice != null) {
        const choice = parseInt(msg.content);

        if (choice === bus.rightChoice) {
            bot.createMessage(msg.channel.id, "Richtig. Ein Schluck verteilen!");
            bus.rightChoice = null;
        }
        else {
            bot.createMessage(msg.channel.id, "Foisch. Trink ans @" + bus.active);
            bus.rightChoice = null;
        }
    }

    if (msg.content === "!endstation") {
        bus.isStarted = false;
        bus.members = [];
        bot.createMessage(msg.channel.id, "Woa a coole foaht. Bis boid");
    }
});

bot.connect();