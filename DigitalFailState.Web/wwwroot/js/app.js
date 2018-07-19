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
                    return goDownTo(score, vm);
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
                alert(e);
                location.reload(true);
            }).finally(() => {
                animating = false;
            });

        },
        fullscreen: () => {
            fullscreen();
        }
    }
});

conn.on("SetScore", score => {
    if (animating) return;
    animating = true;
    var qyn = vm.$el.querySelectorAll('question, yes, no');
    var sco = vm.$el.querySelectorAll('score');
    a({ targets: qyn, opacity: 0, duration: 1000 }).then(() => {
        return a({ targets: sco, opacity: 1, duration: 1000 });
    }).then(() => {
        return goDownTo(score, vm);
    }).then(() => {
        return wait(3000);
    }).then(() => {
        return a({ targets: sco, opacity: 0 });
    }).then(() => {
        return a({ targets: qyn, opacity: 1 });
    }).finally(() => {
        animating = false;
    });
});

function nextQuestion() {
    return conn.invoke("GetNextQuestion").then(q => {
        vm.id = q.id;
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

function flick() {
    if (animating) return;
    animating = true;

    var app = vm.$el;
    var body = document.body;

    var oldBodybackgroundColor = body.style.backgroundColor;
    var oldBodyColor = body.style.color;

    body.style.backgroundColor = "black";
    body.style.color = "lime";

    a({ targets: app, opacity: 0, direction: 'rtl', duration: 100 }).then(() => {
        return a({ targets: app, opacity: 1, direction: 'ltr', duration: 100 });
    }).finally(() => {
        body.style.backgroundColor = oldBodybackgroundColor;
        body.style.color = oldBodyColor;
        animating = false;
    });
}

conn.on("Flick", flick);

conn.start().then(() => {
    return nextQuestion();
}).then(() => {
    return conn.invoke("GetScore");
}).then((score) => {
    vm.score = score;
});

setInterval(() => {
    conn.invoke("Ping").catch(e => {
        alert(e);
        location.reload(true);
    });
}, 10000);

setInterval(() => {
    var chance = anime.random(0, 1000);
    if (chance > 5) return;
    flick();
},1500);