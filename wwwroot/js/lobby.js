// CONSTANTS
const BLUE_TEAM = 0;
const RED_TEAM  = 1;

// DOM
var btn_joinBlue        = document.getElementById('btn-blueJoin');
var btn_leaveBlue       = document.getElementById('btn-blueLeave');
var btn_joinRed         = document.getElementById('btn-redJoin');
var btn_leaveRed        = document.getElementById('btn-redLeave');
var btn_startGame       = document.getElementById('btn-startGame');
var input_playerName    = document.getElementById('input-playerName');
var inputGrp_playerName = document.getElementById('inputGrp-playerName');
var ul_redTeam          = document.getElementById('ul-redteam');
var ul_blueTeam         = document.getElementById('ul-blueteam');

// LOCAL PLAYER
var localPlayer = {};

// SIGNAL R
var connection = new signalR.HubConnectionBuilder().withUrl("/lobbyHub").build();

connection.start().then(function () {
    connection.invoke('GetPlayers');
});

connection.on('GetPlayers', function (playersJson) {
    cleanTeams();
    resetButtons();

    let players = JSON.parse(playersJson);

    players.forEach(function (player) {

        if (player.IsManager) {
            player.Name += " (Admin)";
        }

        addPlayerToTeam(player.Name, player.ConnectionId, player.Team);


        if (player.ConnectionId === connection.connectionId) {
            localPlayer.id = player.ConnectionId;
            joinTeam(player.Team);

            if (player.IsManager) {
                btn_startGame.classList.remove('hidden');
            }
            else {
                btn_startGame.classList.add('hidden');
            }
        }
    });
});

connection.on('StartGame', function () {
    if (!isNullOrWhitespace(localPlayer.id)) {
        window.location.href = "/Game?id=" + localPlayer.id;
    }
});

// EVENTS 
btn_joinBlue.addEventListener('click', function () {
    let playerName = input_playerName.value;
    if (!isNullOrWhitespace(playerName)) {
        connection.invoke('JoinTeam', BLUE_TEAM, playerName);
    }
});

btn_joinRed.addEventListener('click', function () {
    let playerName = input_playerName.value;
    if (!isNullOrWhitespace(playerName)) {
        connection.invoke('JoinTeam', RED_TEAM, playerName);
    }
});

btn_leaveBlue.addEventListener('click', function () {
    leaveTeam();
});

btn_leaveRed.addEventListener('click', function () {
    leaveTeam();
});

btn_startGame.addEventListener('click', function () {
    connection.invoke('StartGame');
});

// UTILITIES
function leaveTeam() {
    connection.invoke('LeaveTeam');
    showControl(inputGrp_playerName);
    hideControl(btn_startGame);
}

function cleanTeams() {
    ul_blueTeam.innerHTML = "";
    ul_redTeam.innerHTML  = "";
}

function resetButtons() {
    showControl(btn_joinBlue);
    showControl(btn_joinRed);
    hideControl(btn_leaveBlue);
    hideControl(btn_leaveRed);
}

function isNullOrWhitespace(input) {
    return !input || !input.trim();
}

function addPlayerToTeam(nickname, id, team) {
    let player = document.createElement('li');
    player.innerText = nickname;
    player.id = 'p-' + id;

    let pTeam = team == BLUE_TEAM ? "blue" : "red";

    document.getElementById('ul-' + pTeam + 'team').appendChild(player);
}

function joinTeam(team) {
    switch (team) {
        case BLUE_TEAM: hideControl(btn_joinBlue);
            hideControl(btn_joinRed);
            hideControl(btn_leaveRed);
            showControl(btn_leaveBlue);
            break;
        case RED_TEAM: hideControl(btn_joinRed);
            hideControl(btn_joinBlue);
            hideControl(btn_leaveBlue);
            showControl(btn_leaveRed);
            break;
    }
    hideControl(inputGrp_playerName);
}

function hideControl(ctrl) {
    if (!ctrl.classList.contains('hidden')) {
        ctrl.classList.add('hidden');
    }
}

function showControl(ctrl) {
    ctrl.classList.remove('hidden');
}