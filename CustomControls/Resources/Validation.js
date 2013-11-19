AMValidator=function(){
    this.Controls=new Array();
};
AMValidator.prototype.IsValid=function(){
    var bool=true;
    for(val=0;val<this.Controls.length;val++){
        if(this.CheckControl(val)){
            document.getElementById(this.Controls[val][0]).innerHTML=this.Controls[val][6];
            bool=false;
        }else{
            document.getElementById(this.Controls[val][0]).innerHTML="";
        };
    };
    return bool;
};
AMValidator.prototype.IsGroupValid=function(name){
    var bool=true;
    for(val=0;val<this.Controls.length;val++){
        if(this.Controls[val][2]==name){
            
            //if (document.getElementById(this.Controls[val][0])!=null){
                if(this.CheckControl(val)){
                    document.getElementById(this.Controls[val][0]).innerHTML=this.Controls[val][6];
                    bool=false;
                }else{
                    if (document.getElementById(this.Controls[val][0])){
                        document.getElementById(this.Controls[val][0]).innerHTML="";
                    };
                };
            //};
        };
    };
    return bool;
};
AMValidator.prototype.CheckControl=function(index){
    var Control=this.Controls[index],obj=document.getElementById(Control[1]),showError=false;
    if(obj!=null){
    if(Control[7]!=null){
        //ReqField
        
        switch(obj.tagName.toUpperCase()){
            case "TEXTAREA":
                if(obj.value==""||obj.value==Control[7]){
                    showError=true;
                };
                break;
            case "INPUT":
                switch(obj.type.toUpperCase()){
                    case "CHECKBOX":
                        if(!obj.checked){
                            showError=true;
                        };
                        break;
                    case "TEXT":
                        if(obj.value==""||obj.value==Control[7]){
                            showError=true;
                        };
                        break;
                    case "PASSWORD":
                        if(obj.value==""||obj.value==Control[7]){
                            showError=true;
                        };
                        break;
                };
                break;
            case "SELECT":
                if(obj.options.length>0){
                    if(obj.options[obj.selectedIndex].value==Control[7]){
                        showError=true;
                    };
                }else{
                    if(!obj.disabled){
                        showError=true;
                    };
                };
                break;
        };
    }else if(Control[4]!=null){
        //Range
        if(parseInt(obj.value)<Control[4]||parseInt(obj.value)>Control[5]){
            showError=true;
        };
    }else{
        //Regex
        var regex=new RegExp(Control[3]);
        if(!regex.test(obj.value)){
            showError=true;
        };
    };
    };
    return showError;
};
AMValidator.prototype.Add=function(id,idtovalidate,group,regex,rangelow,rangehigh,text,initial){
    this.Controls[this.Controls.length]=new Array(id,idtovalidate,group,regex,rangelow,rangehigh,text,initial);
};