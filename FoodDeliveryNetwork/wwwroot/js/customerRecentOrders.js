let statuses = {
    Cooking: {
        Verbatim: "Cooking",
    },
    ReadyForPickup: {
        Verbatim: "Ready for pickup",
    },
    OnTheWay: {
        Verbatim: "On the way",
    },
    Delivered: {
        Verbatim: "Delivered",
    },
    CancelledByCustomer: {
        Verbatim: "Cancelled by you",
    },
    CancelledByRestaurant: {
        Verbatim: "Cancelled by restaurant",
    },
    ReturnedToRestaurant: {
        Verbatim: "Returned to restaurant",
    },
}

var connection = new signalR.HubConnectionBuilder().withUrl("/livedelivery").build();

connection.on("UpdateCustomerView", function (orderId, newStatus, restaurantName) {
    let el1 = document.getElementById(orderId + "-status-wrapper");
    let el2 = document.getElementById(orderId + "-status");

    el1.classList.remove(el2.classList[1]);
    el1.classList.add("order-status-" + newStatus.toLowerCase());

    el2.textContent = statuses[newStatus].Verbatim;

    toastr.info(`Order from ${restaurantName} has a new status: ${statuses[newStatus].Verbatim.toLowerCase()}.`);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});