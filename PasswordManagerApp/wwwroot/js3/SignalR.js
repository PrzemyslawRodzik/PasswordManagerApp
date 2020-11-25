"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationhub")
    .build();

  
(async () => {
    try {
        await connection.start();
    }
    catch (e) {
        console.error(e.toString());
    }
})();



connection.on("ReceiveAlert", function (message, user) {
    HandleNotification(message);
    console.log(user + " says " + message);
    
});





function HandleNotification(message) {
    var div = document.querySelector("#ring > div");
    var diva = document.querySelector("#ring > a");
    $('#ring > a > i').fadeOut('slow');
    $('#ring > a > i').fadeIn('slow');
    $('#sidebar-menu > li:nth-child(7) > a > i').fadeOut('slow');
    $('#sidebar-menu > li:nth-child(7) > a > i').fadeIn('slow');
    $('#ring > div > ul > li > a > div.body-col').html("<p>" + message + "</p>");
    $("#ring").addClass("show");
    $("#ring > div").addClass("show");
    diva.setAttribute("aria-expanded", "true");
    
    div.setAttribute("x-placement", "bottom-start");
    div.setAttribute("style", "position: absolute; transform: translate3d(-101px, 24px, 0px); top: 0px; left: 0px; will-change: transform;");
    
    setTimeout(() => {
        $("#ring").removeClass("show");
        $("#ring > div").removeClass("show");
        diva.setAttribute("aria-expanded", "false");
    },4000)


}




