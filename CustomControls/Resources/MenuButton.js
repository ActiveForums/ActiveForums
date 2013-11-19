ActiveMenuButton=function(id,w,h,s,d,t,l,a){
    this.id=id;
    this.element=document.getElementById(id);
    this.div=document.getElementById(id+'_div');
    this.width=w;
    this.height=h;
    this.interval=null;
    this.display=false;
    this.step=1;
    this.steps=s;
    this.delay=d;
    this.Top=t;
    this.Left=l;
    this.GrowLeft;
    this.GrowTop;
    this.Direction=a;
    this.divPos;
};
ActiveMenuButton.prototype.Toggle=function(){
    var pos=this.GetPosition(this.element);
    if(!this.display){
        this.GrowLeft=this.width/this.steps;
        this.GrowTop=this.height/this.steps;
        this.step=1;
        this.div.style.left=(pos.x+this.Left)+'px';
        this.div.style.top=(pos.y+this.Top)+'px';
        //this.div.style.left = (this.Left) + 'px';
        //this.div.style.top = (this.Top) + 'px';
        this.divPos={x:(pos.x+this.Left),y:(pos.y+this.Top)};
        //this.divPos = { x: (this.Left), y: (this.Top) };
        this.interval=setInterval(this.id+'.Animate("'+this.id+'");',this.delay);
    }else{
        this.div.style.display='none';
        this.display=false;
    };
};
ActiveMenuButton.prototype.Animate=function(id){
    var obj=window[id];
    switch(obj.step){
        case obj.steps:
            obj.display=true;
            obj.div.style.width=parseInt(obj.GrowLeft*obj.step)+'px';
            obj.div.style.height=parseInt(obj.GrowTop*obj.step)+'px';
            clearInterval(obj.interval);
            break;
        case 1:
            obj.div.style.display='';
            obj.div.style.width=parseInt(obj.GrowLeft)+'px';
            obj.div.style.height=parseInt(obj.GrowTop)+'px';
            obj.step+=1;
            break;
        default:
            obj.div.style.width=parseInt(obj.GrowLeft*obj.step)+'px';
            obj.div.style.height=parseInt(obj.GrowTop*obj.step)+'px';
            obj.step+=1;
            break;
    };
    switch(obj.Direction){
        case 1:
            obj.div.style.left=(obj.divPos.x-parseInt(obj.div.style.width))+'px';
            break;
        case 2:
            obj.div.style.top=(obj.divPos.y-parseInt(obj.div.style.height))+'px';
            break;
        case 3:
            obj.div.style.top=(obj.divPos.y-parseInt(obj.div.style.height))+'px';
            obj.div.style.left=(obj.divPos.x-parseInt(obj.div.style.width))+'px';
            break;
    };
};
ActiveMenuButton.prototype.GetPosition = function(obj) {
    var left = 0, top = 0;
      left = ActiveMenuButton.prototype.findPosX(obj)
      top = ActiveMenuButton.prototype.findPosY(obj)
      return {x:left,y:top};
//    while (obj != document.getElementsByTagName('body')[0]) {
//        if (obj.offsetParent) {
//            if (obj.style.position != 'relative') {
//                if (obj.style.position == 'absolute') {
//                    return { x: left, y: top };
//                } else {
//                    left += obj.offsetLeft;
//                    top += obj.offsetTop;
//                };
//                obj = obj.offsetParent
//            } else {
//                obj = obj.parentNode;
//            };
//        };
//    };
//    if (obj != null) {
//        if (obj.offsetParent) { left += obj.offsetLeft; top += obj.offsetTop; };
//    }; return { x: left, y: top };

};
ActiveMenuButton.prototype.findPosX = function(obj) {
    var curleft = 0;
    if (obj.offsetParent)

        while (1) {
//        if (obj.offsetLeft == 0) {
//            return curleft;
//        };
        curleft += obj.offsetLeft;
        if (!obj.offsetParent)
            break;
        obj = obj.offsetParent;
    }
    else if (obj.x)
        curleft += obj.x;
    return curleft;
};

ActiveMenuButton.prototype.findPosY = function(obj) {
    var curtop = 0;
    if (obj.offsetParent)
        while (1) {
        curtop += obj.offsetTop;
        if (!obj.offsetParent)
            break;
        obj = obj.offsetParent;
    }
    else if (obj.y)
        curtop += obj.y;
    return curtop;
};
