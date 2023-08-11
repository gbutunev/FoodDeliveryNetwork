var connection = new signalR.HubConnectionBuilder().withUrl("/livedelivery").build();

connection.on("UpdateDispatcherView", function (totalPrice) {
    console.log('new order');
    toastr.info(`A new order for $${totalPrice} has been placed. Please refresh.`);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});