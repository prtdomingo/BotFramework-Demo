'use strict';

const builder = require('botbuilder');

const ReceiptCard = 'Receipt card';
const AnimationCard = 'Animation card';
const VideoCard = 'Video card';
const ChoiceOptions = [ReceiptCard, AnimationCard, VideoCard];

module.exports = [
    (session) => {
        builder.Prompts.choice(session,
            'Select any of the available standard cards',
            ChoiceOptions, {
                maxRetries: 3,
                retryPrompt: 'The option you\'ve selected is not valid, try again!',
                listStyle: builder.ListStyle.button
            });
    },
    (session, results, next) => {
        const userChoice = results.response.entity;
        const card = createStandardCard(userChoice, session);
        const message = new builder.Message(session).addAttachment(card);
        session.send(message);
    }
];

function createStandardCard(selectedCard, session) {
    switch(selectedCard) {
        case ReceiptCard:
            return createReceiptCard(session);
        case AnimationCard:
            return createAnimationCard(session);
        case VideoCard:
            return createVideoCard(session);
        default:
            return createAnimationCard(session);
    }
}

function createReceiptCard(session) {
    return new builder.ReceiptCard(session)
        .title('Foo Bar')
        .facts([
            builder.Fact.create(session, '555', 'Order Number'),
            builder.Fact.create(session, 'VISA 5555-****', 'Payment Method')
        ])
        .items([
            builder.ReceiptItem.create(session, 'PHP 38.45', 'Data Transfer')
                .quantity(368)
                .image(builder.CardImage.create(session, 'https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png')),
            builder.ReceiptItem.create(session, 'PHP 45.00', 'App Service')
                .quantity(720)
                .image(builder.CardImage.create(session, 'https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png'))
        ])
        .tax('PHP 7.50')
        .total('PHP 90.95')
        .buttons([
            builder.CardAction.openUrl(session, 'https://azure.microsoft.com/en-us/pricing/', 'More Information')
                .image('https://raw.githubusercontent.com/amido/azure-vector-icons/master/renders/microsoft-azure.png')
        ]);
}

function createAnimationCard(session) {
    return new builder.AnimationCard(session)
        .title('Microsoft Bot Framework')
        .subtitle('Animation Card')
        .image(builder.CardImage.create(session, 'https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png'))
        .media([
            { url: 'http://i.giphy.com/Ki55RUbOV5njy.gif' }
        ]);
}

function createVideoCard(session) {
    return new builder.VideoCard(session)
        .title('Big Buck Bunny')
        .subtitle('by the Blender Institute')
        .text('Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute, part of the Blender Foundation. Like the foundation\'s previous film Elephants Dream, the film was made using Blender, a free software application for animation made by the same foundation. It was released as an open-source film under Creative Commons License Attribution 3.0.')
        .image(builder.CardImage.create(session, 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg'))
        .media([
            { url: 'http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4' }
        ]);
}
