function fullscreen(el) {
    var a = el || document.body;
    a.requestFullScreen ? a.requestFullScreen() : a.mozRequestFullScreen ? a.mozRequestFullScreen() : a.webkitRequestFullScreen && a.webkitRequestFullScreen();
};

function goDownTo(nr, vm) {
    return new Promise(function (resolve, reject) {
        var iv = setInterval(() => {
            if (vm.score < nr) {
                vm.score++;
            } else if (vm.score > nr) {
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


function setupPing(conn) {

    var error = document.querySelector("error") || document.createElement("div");

    var start = () => {
        setTimeout(() => {
            conn.start().then(() => {
                error.style.opacity = 0;
            });
        },1000);
    };

    conn.onclose(e => {
        error.style.opacity = 1;
    });

    setInterval(() => {
        conn.invoke("Ping").then(i => {
            error.style.opacity = 0;
        }).catch(e => {
            error.style.opacity = 1;
            start();
        });
    }, 10000);

    conn.on("Restart", () => {
        window.location.reload(true);
    });
}