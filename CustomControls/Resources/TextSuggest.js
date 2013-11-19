objTS=function(id){this.id=id;this.element=this.DomElement=document.getElementById(id);this.ItemsArray=[];this.value='';this.data='';this.ItemSelected=-1;this.MoveCaret=false;this.MouseDown=false;this.div=document.getElementById("div_"+this.id);this.InDiv=false;return this;};
objItemTS=function(id,count){var item=document.createElement('div');item.id=id+"_Item_"+count;item.onclick=this.setValue;item.onmouseover=function(){this.className=window[id].itemSelectedCss;window[id].SelectedItem=this.id;};item.onmouseout=function(){this.className=window[id].itemCss;window[id].SelectedItem='';};item.value="";return item;};
objTS.prototype.TextSuggest_MouseDown=function(event){this.MouseDown=true;};
objTS.prototype.TextSuggest_MouseMove=function(event){if(this.MouseDown){return false;};};
objTS.prototype.TextSuggest_Click=function(event){this.MouseDown=false;var position=caretPosTS(this.element);if(!position){return false;};if(position==this.element.value.length){var end=document.selection?0:this.element.value.length;this.SetCaret(this.element.value.length,end);this.ItemSelected=-1;return true;};for(tsu=0;tsu<this.ItemsArray.length;tsu++){if(position<this.ItemsArray[tsu][3]+this.Delimiter.length&&position>=this.ItemsArray[tsu][2]){if(this.ItemSelected==tsu&&!document.selection){return false;};var end=document.selection?(this.ItemsArray[tsu][3]-this.element.value.length+this.Delimiter.length):(this.ItemsArray[tsu][3]+this.Delimiter.length);this.SetCaret(this.ItemsArray[tsu][2],end);this.ItemSelected=tsu;};};};
objTS.prototype.SetCaret=function(start,end){if(document.selection){ var range=this.element.createTextRange();range.moveStart("character",start);range.moveEnd("character",end);range.select();}else if(this.element.selectionStart){this.element.select();this.element.focus();this.element.setSelectionRange(start,end);};};
objTS.prototype.TextSuggest_Down=function(event){
    var key=document.all?event.keyCode:event.which,end;
    if(this.ItemSelected!=-1){
        if(key==8||key==46){
            this.MoveCaret=true;
            var startPos=this.ItemsArray[this.ItemSelected][2];
            for(tsq=this.ItemSelected+1;tsq<this.ItemsArray.length;tsq++){
                var tmpCount=this.ItemsArray[tsq][3]-this.ItemsArray[tsq][2];
                this.ItemsArray[tsq][2]=startPos;
                this.ItemsArray[tsq][3]=startPos+tmpCount;
                startPos=this.ItemsArray[tsq][3]+1;
            };
            var strStart=this.element.value.substring(0,this.ItemsArray[this.ItemSelected][2]);
            var strEnd=this.element.value.substring(this.ItemsArray[this.ItemSelected][3]+this.Delimiter.length);
            this.element.value=strStart+strEnd;
            this.DelimiterIndex=this.element.value.lastIndexOf(this.Delimiter);
            this.ItemsArray.splice(this.ItemSelected,1);
            this.SetObjects();
            this.ItemSelected=-1;
            if(this.OnDeleted){
                this.OnDeleted();
            };
            return false;
        }else{return true;};
    }else{
        if(key==8){
            if(this.element.value.charAt(this.element.value.length-1)!=32&&this.value==''){
                if(this.ItemsArray.length>0){
                    var lastItem=this.ItemsArray[this.ItemsArray.length-1][0];
                    if(this.element.value.lastIndexOf(lastItem)+1+lastItem.length+this.Delimiter.length!=this.element.value.length){
                        this.element.value=this.element.value.substring(0,this.ItemsArray[this.ItemsArray.length-1][2]);
                        this.ItemsArray.splice(this.ItemsArray.length-1,1);
                        this.SetObjects();
                        if(this.OnDeleted){
                            this.OnDeleted();
                        };
                        return false;
                    };
                };
            };
        };
    };
    if(key==13||key==9){
        if(this.SelectedItem==''){
            return true;
        }else{
            var hid=document.getElementById("hid_"+this.id);
            if(this.Delimiter!=""&&this.element.value.lastIndexOf(this.Delimiter)>0){
                this.element.value=this.element.value.substring(0,this.element.value.lastIndexOf(this.Delimiter)+this.Delimiter.length)+document.getElementById(this.SelectedItem).innerHTML;
            }else{
                this.element.value=document.getElementById(this.SelectedItem).innerHTML;
            };
            if(!document.all){
                var value=document.getElementById(this.SelectedItem).value;
            }else{
                var value=window[this.SelectedItem].value;
            };
            var start=this.element.value.lastIndexOf(document.getElementById(this.SelectedItem).innerHTML);
            var end=start+document.getElementById(this.SelectedItem).innerHTML.length;
            this.ItemsArray[this.ItemsArray.length]=new Array(document.getElementById(this.SelectedItem).innerHTML,value,start,end);
            this.div.style.display='none';
            this.SelectedItem='';
            this.value='';
            this.SetObjects();
            if(this.OnSelected){
                this.OnSelected(this.SelectedText,this.SelectedValue);
            };
            return false;
        };
    };
};
objTS.prototype.TextSuggest_Press=function(event){
    var key=document.all?event.keyCode:event.which,end;
    if(this.ItemSelected!=-1){return;};
    if(this.Delimiter==""&&this.ItemsArray.length==1){
        this.value=this.ItemsArray[0][0];
        this.ItemsArray.splice(0,1);
    };
    if(key==32){if(this.value==''){return;};};
    if(key!=0){
        this.value+=String.fromCharCode(key);
        if(this.value.length==this.CharsToLoad){
            positionTSDiv(this.div.id,this.id);
            this.Callback(this.value);
        }else if(this.value.length>this.CharsToLoad){
            this.UpdateDiv();
        }else{
            this.div.style.display='none';
        };
    };
};
objTS.prototype.TextSuggest_Up=function(event){
    var key=document.all?event.keyCode:event.which,end;
    if(this.Delimiter!=''&&this.value.indexOf(this.Delimiter)>-1){
        var nText=this.value;this.value='';
        this.DelimiterIndex=this.element.value.lastIndexOf(this.Delimiter);
        nText=nText.substring(0,nText.indexOf(this.Delimiter));
        if(nText!=''){
            var itemValue="";
            for(aco=0;aco<this.data.childNodes.length;aco++){
                var nValue=this.data.childNodes[aco].childNodes[0].childNodes[0].nodeValue;
                if(nValue.toUpperCase()==nText.toUpperCase()){
                    itemValue=this.data.childNodes[aco].childNodes[1].childNodes[0].nodeValue;
                };
            };
            var start=this.element.value.lastIndexOf(nText);
            if(itemValue==""&&this.ValidEntry){
                this.element.value=this.element.value.substring(0,start);
            }else{
                this.ItemsArray[this.ItemsArray.length]=new Array(nText,itemValue,start,(start+nText.length));
                this.SetObjects();
                if(this.OnSelected){
                    this.OnSelected(this.SelectedText,this.SelectedValue);
                };
            };
        };
    };
    if(this.MoveCaret){
        this.MoveCaret=false;
        end=document.selection?0:this.element.value.length;
        this.SetCaret(this.element.value.length,end);
    };
    if(key==38){
        if(this.div.childNodes.length>0&&this.div.style.display=='block'){
            if(this.SelectedItem!=''){
                var current=document.getElementById(this.SelectedItem);
                current.className=this.itemCss;
                if(current.previousSibling){
                    current.previousSibling.className=this.itemSelectedCss;
                    this.SelectedItem=current.previousSibling.id;
                };
            };
        };
        return false;
    };
    if(key==40){
        if(this.div.childNodes.length>0&&this.div.style.display=='block'){
            if(this.SelectedItem==''){
                if(this.div.innerHTML!="No results found."){
                    this.SelectedItem=this.div.firstChild.id;
                    if(this.div.firstChild){
                        this.div.firstChild.className=this.itemSelectedCss;
                    };
                };
            }else{
                var current=document.getElementById(this.SelectedItem);
                if(current.nextSibling){
                    current.className=this.itemCss;
                    current.nextSibling.className=this.itemSelectedCss;
                    this.SelectedItem=current.nextSibling.id;
                };
            };
        };
        return false;
    };
    if(key==37){
        var position=caretPosTS(this.element);
        if(this.ItemSelected>0){
            this.ItemSelected-=1;
            end=document.selection?(this.ItemsArray[this.ItemSelected][3]-this.element.value.length+this.Delimiter.length):(this.ItemsArray[this.ItemSelected][3]+this.Delimiter.length);
            this.SetCaret(this.ItemsArray[this.ItemSelected][2],end);
            return false;
        };
        for(tsu=0;tsu<this.ItemsArray.length;tsu++){
            if(position<this.ItemsArray[tsu][3]+this.Delimiter.length&&position>this.ItemsArray[tsu][2]){
                end=document.selection?(this.ItemsArray[tsu][3]-this.element.value.length+this.Delimiter.length):(this.ItemsArray[tsu][3]+this.Delimiter.length);
                this.SetCaret(this.ItemsArray[tsu][2],end);
                this.ItemSelected=tsu;
            };
        };
        return false;
    };
    if(key==39){
        if(this.ItemSelected+1<this.ItemsArray.length){
            this.ItemSelected+=1;
            end=document.selection?(this.ItemsArray[this.ItemSelected][3]-this.element.value.length+this.Delimiter.length):(this.ItemsArray[this.ItemSelected][3]+this.Delimiter.length);
            this.SetCaret(this.ItemsArray[this.ItemSelected][2],end);
        }else{
            this.ItemSelected=-1;
            end=document.selection?0:this.element.value.length;
            this.SetCaret(this.element.value.length,end);
        };
        return false;
    };
    if(key==8){
        if(this.value==''){
            if(this.Delimiter!=""&&this.element.value.length==this.DelimiterIndex-1){
                this.DelimiterIndex=this.element.value.lastIndexOf(this.Delimiter);
                var start=0;
                if(this.DelimiterIndex!=-1){
                    start=this.DelimiterIndex+this.Delimiter.length;
                };
                while(this.element.value.charCodeAt(start)==8){
                    start+=1;
                };
                this.value=this.element.value.substring(start);
                positionTSDiv(this.div.id,this.id);
                this.Callback(this.value);
                this.div.style.display='block';
            };
            if(this.DelimiterIndex==this.element.value.length){
                this.div.style.display='none';
                this.SelectedItem='';
            };
            return;
        };
        this.value=this.value.substring(0,this.value.length-1);
        if(this.value.length>this.CharsToLoad){this.UpdateDiv();}else{this.div.style.display='none';};
        return;
    };
};
objTS.prototype.SelectedItem='';
objTS.prototype.SelectedValue=-1;
objTS.prototype.UpdateDiv=function(){
    this.div.innerHTML="";
    if(this.data){
        for(aco=0;aco<this.data.childNodes.length;aco++){
            var nValue=this.data.childNodes[aco].childNodes[0].childNodes[0].nodeValue;
            if(nValue.substring(0,this.value.length).toUpperCase()==this.value.toUpperCase()){
                var item=new objItemTS(this.id,aco);
                if(document.all){
                    item.innerText=this.data.childNodes[aco].childNodes[0].childNodes[0].nodeValue;
                }else{
                    item.textContent=this.data.childNodes[aco].childNodes[0].childNodes[0].nodeValue;
                };
                item.value=this.data.childNodes[aco].childNodes[1].childNodes[0].nodeValue;
                div.appendChild(item);
            };
        };
        this.div.style.display='block';
    };
    if(this.div.innerHTML==""){
        if(this.ValidEntry&&this.value.charAt(this.value.length)==this.Delimiter){
            this.div.innerHTML="No results found.";
        }else{
            this.div.style.display='none';
        };
    };
};
objItemTS.prototype.setValue=function(){
    var ids=this.id.split("_Item_"),textBox=document.getElementById(ids[0]),hid=document.getElementById("hid_"+ids[0]),elem=window[ids[0]];
    var start=textbox.value.lastIndexOf(elem.value);
    if(elem.Delimiter!=""&&textbox.value.lastIndexOf(elem.Delimiter)>0){
        textbox.value=textbox.value.substring(0,textbox.value.lastIndexOf(elem.Delimiter)+elem.Delimiter.length)+this.innerHTML;
    }else{
        textBox.value=this.innerHTML;
    };
    elem.ItemsArray[elem.ItemsArray.length]=new Array(this.innerHTML,this.value,start,(start+this.innerHTML.length));
    elem.value='';
    elem.SelectedItem='';
    elem.SetObjects();
    if(elem.OnSelected){
        elem.OnSelected(elem.SelectedText,elem.SelectedValue);
    };
    elem.Blur();
    elem.div.style.display='none';
    var end=document.selection?0:textbox.value.length;
    elem.SetCaret(textbox.value.length,end);
};
positionTSDiv=function(divID,textboxID){div=document.getElementById(divID);textbox=document.getElementById(textboxID);pos=getPositionTS(textbox);div.style.top=textbox.offsetHeight+pos.y;div.style.left=pos.x;};
getPositionTS=function(e){
    var left=0,top=0;
    while(e!=document.getElementsByTagName('body')[0]){
        if(e.style.position!='relative'){
            if(e.style.position=='absolute'){
                return{x:left,y:top};
            }else{
                left+=e.offsetLeft;
                top+=e.offsetTop;
            };
            e=e.offsetParent;
        }else{
            e=e.parentNode;
        };
    };
    if(e.offsetParent){left+=e.offsetLeft;top+=e.offsetTop;};return{x:left,y:top};};
objTS.prototype.Blur=function(){
    if(this.SelectedItem==''&&!this.InDiv){
        this.div.style.display='none';
    }else{
        if(this.div.innerHTML=="No results found."){
            this.div.style.display='none';
        }else{
            return;
        };
    };
    if(this.value!=''){
        if(this.data){
            var itemValue="";
            for(aco=0;aco<this.data.childNodes.length;aco++){
                var nValue=this.data.childNodes[aco].childNodes[0].childNodes[0].nodeValue;
                if(nValue.toUpperCase()==this.value.toUpperCase()){
                    itemValue=this.data.childNodes[aco].childNodes[1].childNodes[0].nodeValue;
                };
            };
            var start=this.element.value.lastIndexOf(this.value);
            if(itemValue==""&&this.ValidEntry){
                this.value="";
                this.element.value=this.element.value.substring(0,start);
            }else{
                this.ItemsArray[this.ItemsArray.length]=new Array(this.value,itemValue,start,(start+this.value.length));
                this.value='';
                this.SetObjects();
                if(this.OnSelected){
                    this.OnSelected(this.SelectedText,this.SelectedValue);
                };
            };
        };
    };
};
objTS.prototype.Focus=function(){
    var end=document.selection?0:this.element.value.length;
    this.SetCaret(this.element.value.length,end);
};
objTS.prototype.SetObjects=function(){
    var strText='',strValue='';
    for(tsu=0;tsu<this.ItemsArray.length;tsu++){
        strText+=this.ItemsArray[tsu][0]+this.Delimiter;
        strValue+=this.ItemsArray[tsu][1]+this.Delimiter;
    };
    if(this.Delimiter.length>0){
        strText=strText.substring(0,strText.lastIndexOf(this.Delimiter));
        strValue=strValue.substring(0,strValue.lastIndexOf(this.Delimiter));
    };
    this.SelectedText=strText;
    this.SelectedValue=strValue;
    document.getElementById("hid_"+this.id).value=strValue;
};
loadDataTS=function(id){var str=window[id].data;str=str.replace(/&lt;/g,"<");str=str.replace(/&gt;/g,">");if (window.ActiveXObject){var doc=new ActiveXObject("Microsoft.XMLDOM");doc.async="false";doc.loadXML(str);}else{var parser=new DOMParser(),doc=parser.parseFromString(str,"text/xml");};window[id].data=doc.documentElement;window[id].UpdateDiv();};
caretPosTS=function(obj){if(document.selection){obj.focus();var range=document.selection.createRange();if(range.parentElement()!=obj){return false;};var orig=obj.value;range.text="|&%";var actual=obj.value;var index=actual.indexOf('|&%');obj.value=orig;return index;}else if(obj.selectionStart){return obj.selectionEnd;};};