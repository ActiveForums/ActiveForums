function amaf_handleDebug(d) {
    var div = document.getElementById('amafdebug');
    if (div == null) {
        div = document.createElement('div');
        div.id = 'amafdebug';
        div.style.backgroundColor = '#fff';
        div.style.position = 'absolute';
        div.style.top = '0px';
        div.style.left = '0px';
        div.style.width = '100%';
    };
    var ul = document.createElement('ul');
    var li = document.createElement('li');
    li.appendChild(document.createTextNode('time: ' + amaf_runtime));
    ul.appendChild(li);
    for (prop in d) {
        li = document.createElement('li');
        li.appendChild(document.createTextNode(prop + ': ' + d[prop]));
        ul.appendChild(li);
    };
    div.appendChild(ul);
    document.body.appendChild(div);
};