// CONSTANTS
const CARD_APPARITION_TIMEOUT = 30;

const BLUE_CARD = 1;
const RED_CARD = 2;
const BLACK_CARD = 3;
const WHITE_CARD = 4;

const BLUE_TEAM = 0;
const RED_TEAM = 1;

// DOM
var h2_redRound     = document.getElementById('h2-redRound');
var h2_blueRound    = document.getElementById('h2-blueRound');
var h1_gameOver     = document.getElementById('h1-gameOver');
var btn_nextRound   = document.getElementById('btn-nextRound');
var red_wordsLeft   = document.getElementById('red-wordsLeft');
var blue_wordsLeft  = document.getElementById('blue-wordsLeft');
var btn_newGame     = document.getElementById('btn-newGame');
var ul_redTeam      = document.getElementById('ul-redTeam');
var ul_blueTeam     = document.getElementById('ul-blueTeam');

// INIT
(function init() {
    btn_nextRound.addEventListener('click', function () {
        connection.invoke('NextRound');
    });
    btn_newGame.addEventListener('click', function () {
        connection.invoke('NewGame');
    });
})();

// LOCAL PLAYER
var localPlayer = {};

// SIGNALR
var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.start().then(function () {
    let queryString = window.location.search;
    let urlParams = new URLSearchParams(queryString);
    let connectionId = urlParams.get('id');
    connection.invoke('AssociateGameId', connectionId);
});

connection.on('GetPlayers', function (playersJson) {
    let players = JSON.parse(playersJson);

    clearTeams();

    players.forEach(function (player) {
        if (player.IsLeader) {
            player.Name += " (chef d'équipe)";
        }
        addPlayerToTeam(player);
    });
});

connection.on('NewGame', function () {

    h1_gameOver.innerText = "";

    let cardsDiv = document.getElementsByClassName('c-card');
    Array.prototype.forEach.call(cardsDiv, function (card) {
        card.className = 'c-card codename-card-hidden';
    });
});

connection.on('SetupLocalPlayer', function (playersJson) {
    let players = JSON.parse(playersJson);
    players.forEach(function (player) {
        if (player.GameId == connection.connectionId) {
            localPlayer = player;
        }
    });

    if (localPlayer.IsLeader) {
        btn_nextRound.classList.remove('hidden');
    }
    else {
        btn_nextRound.classList.add('hidden');
    }

    if (localPlayer.IsManager) {
        btn_newGame.classList.remove('hidden');
    }
    else {
        btn_newGame.classList.add('hidden');
    }
});

connection.on('GameOver', function (winner) {

    h2_blueRound.classList.add('hidden');
    h2_redRound.classList.add('hidden');

    switch (winner) {
        case BLUE_TEAM:
            h1_gameOver.innerText = "Les bleus ont gagnés !";
            h1_gameOver.classList.add('blue-text');
            break;
        case RED_TEAM:
            h1_gameOver.innerText = "Les rouges ont gagnés !";
            h1_gameOver.classList.add('red-text');
            break;
    }
});

connection.on('AnimateCard', function (index, newclass) {
    animateCard(index, newclass);
});

connection.on('ShowMessage', function (message) {
    alert(message);
});

connection.on('DisplayCards', function (cardsJson) {
    let cards = JSON.parse(cardsJson);
    displayCards(cards);
});

connection.on('SyncGameStatus', function (gameStatusJson) {
    let gameStatus = JSON.parse(gameStatusJson);

    switch (gameStatus.PlayingTeam) {
        case BLUE_TEAM:
            h2_redRound.classList.add('hidden');
            h2_blueRound.classList.remove('hidden');
            break;
        case RED_TEAM:
            h2_blueRound.classList.add('hidden');
            h2_redRound.classList.remove('hidden');
            break;
    }

    blue_wordsLeft.innerText = "Il reste " + gameStatus.BlueWordsLeft + " mots à trouver";
    red_wordsLeft.innerText = "Il reste " + gameStatus.RedWordsLeft + " mots à trouver";

});

// UTILITIES
function clearTeams() {
    ul_redTeam.innerHTML = "";
    ul_blueTeam.innerHTML = "";
}

function addPlayerToTeam(player) {
    let pTeam = player.Team == BLUE_TEAM ? 'blue' : 'red';
    let ul = document.getElementById('ul-' + pTeam + 'Team');
    let p = document.createElement('li');
    p.innerText = player.Name;
    ul.appendChild(p);
}

function animateCard(index, newclass) {
    let card = document.getElementById('card-' + index);
    card.classList.add('codename-card-animate');
    setTimeout(function () {
        card.classList.remove('codename-card-animate');
        card.classList.add(newclass);
    }, 500);
}

function displayCards(cards) {

    cards.forEach(function (card) {
        let cardDiv = document.getElementById('card-text-' + card.Id);
        cardDiv.innerText = card.Text;

        if (localPlayer.IsLeader) {
            switch (card.Color) {
                case BLUE_CARD:
                    cardDiv.parentElement.classList.add('blue'); break;
                case RED_CARD:
                    cardDiv.parentElement.classList.add('red'); break;
                case BLACK_CARD:
                    cardDiv.parentElement.classList.add('black'); break;
            }
        }
    });

    let cardsDiv = document.getElementsByClassName('codename-card-hidden');

    let timeout = 0;
    Array.prototype.forEach.call(cardsDiv, function (card) {
        setTimeout(function () {
            card.classList.remove('codename-card-hidden');
            card.classList.add('codename-card');
        }, timeout++ * CARD_APPARITION_TIMEOUT);
    });

    if (localPlayer.IsLeader) {
        for (let i = 0; i < 25; ++i) {
            document.getElementById('card-' + i).addEventListener('click', function () {
                connection.invoke('PlayCard', i);
            });
        }
    }
}