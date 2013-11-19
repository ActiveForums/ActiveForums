ActiveGrid=function(id){
    this.id=id;
    this.element=document.getElementById(id);
    this.table=this.element.firstChild;
    if(this.table.nodeType==3){this.table=this.table.nextSibling;};
    this.ShowPager=true;
    this.Results=0;
    this.PageIndex=0;
    this.Sort='DESC';
    this.DefaultSort='DESC';
    this.SortImages=[];
    this.Params="";
    this.ImagePath="";
};
ActiveGrid.prototype.SetColumns=function(){
    var headerCells=this.table.rows[0].cells;
    var id=this.id;
    for(bgi=0;bgi<headerCells.length;bgi++){
        if(headerCells[bgi].getAttribute("AllowSorting")=="true"){
	        var img=document.createElement('img');
	        img.src=this.SpacerImage;
	        img.id=id+'_img_'+bgi;
	        img.width=10;
	        img.height=10;
	        headerCells[bgi].setAttribute("ImageID",img.id)
	        headerCells[bgi].onclick=function(){window[id].SortColumn(this.getAttribute("ColumnName"),this.getAttribute("ImageID"));};
	        var div=document.createElement('div');
            if(document.all){div.style.styleFloat='right';}else{div.style.cssFloat='right'};
            div.style.textAlign='right';
            div.style.height='10px';
            div.style.width='10px';
	        this.SortImages[this.SortImages.length]=img.id;
	        div.appendChild(img);
	        headerCells[bgi].appendChild(div);
	    };
    };
};
ActiveGrid.prototype.Build=function(){
    var titleWidth=30;
    if(this.Width==""){
        this.Width=this.table.width;
        if(this.Width==""){
            this.Width=this.table.style.width;
            if(this.Width.indexOf('px')>-1){
                this.Width=parseInt(this.Width);
            };
        };
    };
    if(isNaN(this.Width)&&this.Width.indexOf('%')>-1){this.Width=this.table.offsetWidth;};
    var tmpWidth=this.Width,id=this.id;
    var usedCellWidth=0;
    var headerCells=this.table.rows[0].cells;
    for(bgi=1;bgi<headerCells.length;bgi++){
	    if(this.table.rows[1].cells[bgi].getAttribute("resize")){
	        continue;
	    };
	    if(!isNaN(parseInt(headerCells[bgi].style.width))){
	        usedCellWidth+=parseInt(headerCells[bgi].style.width);
	    };
	};
	tmpWidth=tmpWidth-(headerCells.length*8);
	if((usedCellWidth+30)>tmpWidth){
	    titleWidth=30;
	}else{
	    titleWidth=tmpWidth-usedCellWidth;
	};
	for(bgh=0;bgh<headerCells.length;bgh++){
	    if(this.table.rows[1].cells[bgh].getAttribute("resize")){
	        this.table.rows[0].cells[bgh].style.width=titleWidth+'px';
	        break;
	    };
	};
	var rowSplit=this.data.split(this.RowDelimiter);
	for(bgi=0;bgi<(rowSplit.length-1);bgi++){
		var colSplit=rowSplit[bgi].split(this.ColDelimiter);
		var row=this.table.insertRow(this.table.rows.length);
		row.setAttribute("SelectedStyle",this.SelectedStyle);
		if((bgi%2)==0){
		    row.setAttribute("ItemStyle",this.ItemStyle);
    		row.onmouseout=function(){this.className=this.getAttribute("ItemStyle");};
			row.onmouseover=function(){this.className=this.getAttribute("SelectedStyle");};
			row.className=this.ItemStyle;
		}else{
		    row.setAttribute("ItemStyle",this.AltItemStyle);
			row.onmouseout=function(){this.className=this.getAttribute("ItemStyle");};
			row.onmouseover=function(){this.className=this.getAttribute("SelectedStyle");};
			row.className=this.AltItemStyle;
		};
		var dataCount=0;
		for(bgx=0;bgx<(colSplit.length);bgx++){
            var cell=this.table.rows[1].cells[bgx].cloneNode(true);
            var tmpStr=cell.innerHTML;
            while(cell.innerHTML.indexOf("##DataItem('")>-1){
                var start=tmpStr.indexOf("##DataItem('");
                var end=tmpStr.indexOf("')##")+4;
                var replace=cell.innerHTML.substring(start,end);
                cell.innerHTML=cell.innerHTML.replace(replace,colSplit[dataCount]);
                if (cell.innerHTML == ''){
                    cell.innerHTML = '&nbsp;';
                };
                dataCount+=1;
            };
    		row.appendChild(cell);
		};
	};
	this.element.style.height=this.table.offsetHeight+'px';
	if(this.ShowPager){this.BuildPager();};
};
ActiveGrid.prototype.SortColumn=function(column,id){
    var img=document.getElementById(id);
    for(var srt=0;srt<this.SortImages.length;srt++){
        document.getElementById(this.SortImages[srt]).src=this.SpacerImage;
    };
    if(column==this.Column){
        if(this.Sort=='ASC'){
            this.Sort='DESC';
            img.src=this.DescImage;
        }else{
            this.Sort='ASC';
            img.src=this.AscImage;
        };
    }else{
        this.Column=column;
        this.Sort=this.DefaultSort;
        if(this.Sort=='ASC'){
            img.src=this.AscImage;
        }else{
            img.src=this.DescImage;
        };
    };
    this.Callback();
};
ActiveGrid.prototype.Callback=function(){
    this.DeleteRows();
    window[this.CB].Callback(this.PageIndex,this.PageSize,this.Column,this.Sort,this.Params);
};
ActiveGrid.prototype.DeleteRows=function(){
	currentRows=this.table.rows.length-2;
	for(dri=0;dri<currentRows;dri++){
		this.table.deleteRow(2);
	};
};
ActiveGrid.prototype.BuildPager=function(){
    var amPager;
    if(this.PageCount<=1){
        this.Pager.innerHTML='';
        return false;
    }else{
        var id=this.id;
        amPager='<table cellpadding=0 cellspacing=0 border=0 align=right><tr>';
        amPager+='<td width=7><img src='+this.ImagePath+'pgLeft.gif></td>';
        amPager+='<td class='+this.CssPagerInfo+' width=100>'+this.PagerText.replace('{0}',(this.PageIndex+1)).replace('{1}',this.PageCount)+'</td>';
        var lastCell='',cellWidth=Math.floor(95/(this.PagerPages+4));
        if(this.PageIndex>0){
            amPager+='<td class='+this.CssPagerItem+' onclick='+id+'.GoPage(0)><img src='+this.ImagePath+'pgHome.gif></td>';
            amPager+='<td class='+this.CssPagerItem2+' onclick='+id+'.GoPage('+(this.PageIndex-1)+')><img src='+this.ImagePath+'pgPrev.gif></td>';
        };
        var iPager=0;
        if(this.PageIndex>=(this.PagerPages-1)){
            iPager=this.PageIndex-1;
        };
        if(this.PageIndex==(this.PageCount-1)){
            if(this.PageCount<this.PagerPages){
                iPager=0;
            }else{
                iPager=this.PageCount-this.PagerPages;
            };
        };
        var loopEnd=iPager+this.PagerPages;
        if(this.PageCount<loopEnd){
            loopEnd=this.PageCount;
        };
        for(var xPager=iPager;xPager<loopEnd;xPager++){
            amPager+='<td ';
            if(xPager==this.PageIndex){
                amPager+=' class='+this.CssPagerCurrentNumber;
            }else{
                amPager+=' class='+this.CssPagerNumber;
            };
            amPager+=' onclick='+id+'.GoPage('+xPager+')';
            amPager+='>'+(xPager+1)+'</td>';
        };
        if(this.PageCount-1!=this.PageIndex){
            amPager+='<td class='+this.CssPagerItem+' onclick='+id+'.GoPage('+(this.PageIndex+1)+')><img src='+this.ImagePath+'pgNext.gif></td>';
            amPager+='<td class='+this.CssPagerItem+' onclick='+id+'.GoPage('+(this.PageCount-1)+')><img src='+this.ImagePath+'pgLast.gif></td>';
        };
        amPager+='<td width=9><img src='+this.ImagePath+'pgRight.gif></td>';
        amPager+='</tr></table>';
        this.Pager.innerHTML=amPager;
    };
};
ActiveGrid.prototype.GoPage=function(index){
    this.PageIndex=index;
    this.Callback();
};
