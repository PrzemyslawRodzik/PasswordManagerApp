

/*
 *  
 * Wylogowanie po 5 min nieaktywności
 * 
 */

var inactivityTime = function () {
    var time;
    window.onload = resetTimer;

    // DOM Events
    window.onmousemove = resetTimer;
    window.onmousedown = resetTimer;  // catches touchscreen presses as well      
    window.ontouchstart = resetTimer; // catches touchscreen swipes as well 
    window.onclick = resetTimer;      // catches touchpad clicks as well
    window.onkeypress = resetTimer;

    function logout() {
        alert("You are now logged out.")
        window.location = '/auth/login';
    }

    function resetTimer() {
        clearTimeout(time);
        time = setTimeout(logout, 1000 * 60 * 5 )
        // 1000 milliseconds = 1 second
    }
};
window.onload = function () {

    inactivityTime();
}