function fullscreen(el) {
    var a = el || document.body;
    a.requestFullScreen ? a.requestFullScreen() : a.mozRequestFullScreen ? a.mozRequestFullScreen() : a.webkitRequestFullScreen && a.webkitRequestFullScreen();
};

function goDownTo(nr, vm) {
    return new Promise(function (resolve, reject) {
        var iv = setInterval(() => {
            if (vm.score > nr) {
                vm.score--;
            } else {
                clearInterval(iv);
                resolve();
            }
        }, 15);
    });
}

function wait(delay) {
    return new Promise((resolve, reject) => {
        setTimeout(() => resolve(), delay);
    });
}