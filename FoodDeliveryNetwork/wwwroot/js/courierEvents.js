var connection = new signalR.HubConnectionBuilder().withUrl("/livedelivery").build();

connection.on("UpdateCourierView", function (restaurantName, restaurantAddress, deliveryAddress) {
    console.log('new order');
    toastr.info(`A new order is ready to be picked up from ${restaurantName} (${restaurantAddress}) with delivery address: ${deliveryAddress}.`);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});