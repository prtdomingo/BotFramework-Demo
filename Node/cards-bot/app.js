'use strict';

require('dotenv-extended').load();

const builder = require('botbuilder');
const restify = require('restify');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, () => {
    console.log(`${server.name} listening to ${server.url}`);
});

server.post('/api/messages', connector.listen());

const AdaptiveCard = 'Adaptive Card';
const StandardCard = 'Standard Card';

const bot = new builder.UniversalBot(connector, [
    (session) => {
        var card = createGreetingsCard(session);
        var message = new builder.Message(session).addAttachment(card);
        session.send(message);
    }
]);

bot.dialog('standardCard', require('./dialogs/standardCardDialog'))
    .triggerAction({ matches: /^Standard Card$/i });

function createGreetingsCard(session) {
    return new builder.HeroCard(session)
        .title('I\'m a Visual Card Bot')
        .subtitle('What\'s up? What do you want to do?')
        .images([
            builder.CardImage.create(session, 'http://adaptivecards.io/api/cat')
        ])
        .buttons([
            builder.CardAction.imBack(session, StandardCard, StandardCard),
            builder.CardAction.imBack(session, AdaptiveCard, AdaptiveCard)
        ]);
}