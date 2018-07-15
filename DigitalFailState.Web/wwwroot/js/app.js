var conn = new signalR.HubConnectionBuilder().withUrl("/appHub").build();
var animating = false;
var vm = new Vue({
    el: 'app',
    data: {
        question: '',
        conclusion: '',
        score: 0
    },
    methods: {
        wrong: (d) => {
            if (animating) return;
            animating = true;

            vm.conclusion = d === "y" ? vm.yesConclusion : vm.noConclusion;
            var qyn = vm.$el.querySelectorAll('question, yes, no');
            var con = vm.$el.querySelectorAll('conclusion');

            a({
                targets: qyn, opacity: 0
            }).then(() => {
                return conn.invoke("WrongAnswer");
            }).then((score) => {
                return a({ targets: con, opacity: 1 }).then(() => {
                    return goDownTo(score);
                });
            }).then(() => {
                return wait(3000);
            }).then(() => {
                return a({ targets: con, opacity: 0 });
            }).then(() => {
                return nextQuestion();
            }).then(() => {
                return a({ targets: qyn, opacity: 1 });
            }).catch((e) => {
                alert("error: " + e);
                location.reload(true);
            }).finally(() => {
                animating = false;
            });

        },
        fullscreen: () => {
            fullscreen(vm.$el);
        }
    }
});

conn.on("SetScore", score => {
    goDownTo(score);
});

function nextQuestion() {
    return conn.invoke("GetNextQuestion").then(q => {
        vm.question = q.question;
        vm.yesConclusion = q.yesConclusion;
        vm.noConclusion = q.noConclusion;
        return Promise.resolve();
    });
}

function a(opt) {
    opt.easing = opt.easing || 'linear';
    opt.duration = opt.duration || 2000;
    return anime(opt).finished;
}

function goDownTo(nr) {
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

function fullscreen(a) { a.requestFullScreen ? a.requestFullScreen() : a.mozRequestFullScreen ? a.mozRequestFullScreen() : a.webkitRequestFullScreen && a.webkitRequestFullScreen() };

conn.start().then(() => {
    return nextQuestion();
})