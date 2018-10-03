function fullscreen(el) {
    var a = el || document.body;
    a.requestFullScreen ? a.requestFullScreen() : a.mozRequestFullScreen ? a.mozRequestFullScreen() : a.webkitRequestFullScreen && a.webkitRequestFullScreen();
};

function goDownTo(nr, vm) {
    return new Promise(function (resolve, reject) {
        var diff = Math.abs(vm.score - nr);
        if (diff === 0) {
            resolve();
            return;
        }
        var delay = 3000 / diff;
        var iv = setInterval(() => {
            if (vm.score < nr) {
                vm.score++;
            } else if (vm.score > nr) {
                vm.score--;
            } else {
                clearInterval(iv);
                resolve();
            }
        }, delay);
    });
}

function typeLetters(text, vm, property) {
    return new Promise(function (resolve, reject) {
        if (text === null || text === undefined || text.length < 1) {
            resolve();
            return;
        }
        var letters = text.split("");
        var state = "";
        var delay = 2000 / letters.length;
        var iv = setInterval(() => {
            if (letters.length <= 1) {
                vm[property] = text;
                clearInterval(iv);
                resolve();
            } else {
                var nextLetter = letters.shift();
                state += nextLetter;
                vm[property] = state + "|";
            }
        }, delay);
    });
}

function wait(delay) {
    return new Promise((resolve, reject) => {
        setTimeout(() => resolve(), delay);
    });
}

function setupPing(conn, vm) {

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
        var questionId = -1;
        if (vm && vm.id) questionId = vm.id;
        conn.invoke("Ping", questionId).then(i => {
            error.style.opacity = 0;
        }).catch(e => {
            error.style.opacity = 1;
            start();
        });
    }, 2000);

    conn.on("Restart", () => {
        window.location.reload(true);
    });
}
