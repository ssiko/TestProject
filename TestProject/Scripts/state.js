let state = {};

async function updateUrlHash() {
    var hashString = Object.keys(state).map(key => key + '=' + encodeURIComponent(state[key])).join('&');
    window.location.hash = hashString;
}

async function getStateFromHash() {
    var pairs = location.hash.slice(1).split('&');
    pairs.forEach(function (pair) {
        pair = pair.split('=');
        if (pair[0]) {              
            state[pair[0]] = decodeURIComponent(pair[1] || '');
        }
    });
}