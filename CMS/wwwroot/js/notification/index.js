window.socket = new signalR.HubConnectionBuilder()
    .withUrl("/_notification")
    .withAutomaticReconnect([0, 0, 10000])
    .build();

window.socket.start().then(function () {
    window.socket.invoke("JoinGroup", $("#userId").val())
        .catch(err => {
            console.log(err);
        });
});

