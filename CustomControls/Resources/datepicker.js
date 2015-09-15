asDatePicker = function(id) {
    this.id = id;
    this.image = document.getElementById('img_' + id);
    this.textbox = document.getElementById(id);
    this.calendar = document.getElementById('cal_' + id);
    this.hidden = document.getElementById('hid_' + id);
    this.dateSel = new Date();
    this.currMonth = this.dateSel.getMonth();
    this.currYear = this.dateSel.getFullYear();
    this.minDate = new Date(1900, 1, 1);
    this.selectedDate = new Date();
};
asDatePicker.prototype.Set = function(elem) {
    if (this.isValid(elem)) {
        this.dateSel = this.GetSelectedDate(elem);
        var d = this.dateSel;
        this.SelectedDay = d.getDate();
        this.SelectedMonth = d.getMonth();
        this.SelectedYear = d.getFullYear();
        this.Day = this.SelectedDay;
        this.Month = this.SelectedMonth;
        this.Year = this.SelectedYear;

        this.textbox.value = d.formatDP(this.DateFormat, this.id);
        this.selectedDate = d;
        this.ClearSelected()
        elem.className = this.Class.CssSelectedDayStyle
        if (this.ShowTime == true) {
            var time = document.getElementById('time_' + this.id);
            var timeValue = time.options[time.selectedIndex].value;
            if (timeValue != '') {
                var h = timeValue.split(':')[0];
                var m = timeValue.split(':')[1];
                if (timeValue.toLowerCase().indexOf('p') > 0) {
                    h = parseFloat(h);
                    if (h < 12) { h += 12; };

                };
                this.dateSel.setHours(h);
                m = m.substring(0, 2);
                this.dateSel.setMinutes(m);
            };
            this.textbox.value = d.formatDP(this.DateFormat + ' ' + this.TimeFormat, this.id);
            this.selectedDate = d;
        } else {
            this.calendar.style.display = 'none';
        };
        if (this.linkedControl != '' && this.isEndDate == false) {
            var id = this.linkedControl;
            window[id].minDate = new Date(d.addMinutes(15));
        };
    } else {
        this.textbox.value = '';
        alert('invalid date');
    };
};
asDatePicker.prototype.onblur = function(obj) {
    var tmp = null;
    try {
        tmp = new Date(obj.value);
    } catch (e) {
        alert(e.description);
        obj.value = '';
    };
    if (tmp == 'NaN') {
        this.textbox.value = '';
    };
    if (obj.value != '') {
        this.SetDate(obj.value);
    };

};
asDatePicker.prototype.isValid = function(elem) {
    var valid = true;
    var tmp = this.GetSelectedDate(elem);
    if (tmp <= this.minDate) {
        valid = false;
    };
    if (this.isEndDate == true && this.linkedControl != '') {
        if (tmp <= window[this.linkedControl].selDate) {
            valid = false;
        };
    };
    return valid;
};
asDatePicker.prototype.ClearSelected = function() {
    var e = this.calendar.getElementsByTagName("td");
    var currDay = new Date();
    for (var i = 0; i < e.length; i++) {
        if (e[i].className == this.Class.CssSelectedDayStyle) {
            e[i].className = this.Class.CssCurrentMonthDayStyle;
        };
        if ((parseFloat(e[i].getAttribute('Month')) == currDay.getMonth()) && (parseFloat(e[i].getAttribute('Year')) == currDay.getFullYear()) && (parseFloat(e[i].getAttribute('Day')) == currDay.getDate())) {
            e[i].className = this.Class.CssCurrentDayStyle;
        };

    };
};
asDatePicker.prototype.SetDate = function(datestring) {
    var date = new Date(datestring);
    this.SelectedDay = date.getDate();
    this.SelectedMonth = date.getMonth();
    this.SelectedYear = date.getFullYear();
    this.dateSel = new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), 0, 0);
    this.SetTime();
};
asDatePicker.prototype.SetTime = function(elem) {

    var time = document.getElementById('time_' + this.id);
    if (this.isValidTime(time)) {
        var date = this.GetSelectedDate();
        this.textbox.value = date.formatDP(this.DateFormat + ' ' + this.TimeFormat);
        this.selectedDate = date;
        this.selDate = date;
        if (this.linkedControl != '' && this.isEndDate == false) {
            var id = this.linkedControl;
            window[id].minDate = new Date(date.addMinutes(15));
        };
    } else {
        this.textbox.value = '';
        alert('invalid time');
    };
};
asDatePicker.prototype.GetSelectedDate = function(elem) {
    var y, m, d, h, mm;
    if (elem == null) {
        y = this.dateSel.getFullYear();
        m = this.dateSel.getMonth();
        d = this.dateSel.getDate();
        h = this.dateSel.getHours();
        mm = this.dateSel.getMinutes();
    } else {
        y = elem.getAttribute('Year');
        m = elem.getAttribute('Month');
        d = elem.getAttribute('Day');
        h = 0
        mm = 0
    };
    var tmpDate = new Date(y, m, d, h, mm);
    var ddl = document.getElementById('time_' + this.id);
    if (ddl != null) {
        var timeValue = ddl.options[ddl.selectedIndex].value;
        if (timeValue != '') {
            var h = timeValue.split(':')[0];
            var m = timeValue.split(':')[1];
            if (timeValue.toLowerCase().indexOf('p') > 0) {
                h = parseFloat(h);
                if (h < 12) { h += 12; };

            };
            if (timeValue.toLowerCase().indexOf('a') > 0) {
                h = parseFloat(h);
                if (h == 12) {
                    h = 0;
                };
            };
            tmpDate.setHours(h);
            m = m.substring(0, 2);
            tmpDate.setMinutes(m);
        };
    };
    return tmpDate;


};
asDatePicker.prototype.isValidTime = function(elem) {
    var d = this.GetSelectedDate();
    var valid = true;
    var time = document.getElementById('time_' + this.id);
    if (time != null) {
        if (time.selectedIndex == -1 && this.RequireTime == true) {
            valid = false;
        };
    };
    if (this.isEndDate == true) {
        if (d <= this.minDate) {
            valid = false;
        };
    };
    return valid;
};
asDatePicker.prototype.Show = function(m, y) {
    var id = this.id;
    var today = '';
    if (this.textbox.value != '') {
        today = this.GetSelectedDate();
        this.SetDate(today)
    };
    this.calendar.innerHTML = '';
    if (this.textbox.value == '' && this.minDate.getFullYear() > 1900) {
        m = this.minDate.getMonth();
        y = this.minDate.getFullYear();
    };
    if (m == undefined || y == undefined) {
        m = parseFloat(this.Month);
        y = parseFloat(this.Year);
    };
    var calMonth = new Date(y, m, 1);
    this.currMonth = calMonth.getMonth();
    this.currYear = calMonth.getFullYear();
    this.MonthDays[1] = (y % 4 == 0 && (y % 100 != 0 || y % 400 == 0)) ? 29 : 28;

    var startDay = calMonth.getDay();
    var table = document.createElement('table');
    table.setAttribute('id', 'calendar');
    table.className = this.Class.CssCalendarStyle;
    table.cellSpacing = 0;
    table.border = 0;
    table.width = this.Width;
    table.height = this.Height;
    var tbody = document.createElement('tbody');
    var header = document.createElement('tr');
    header.setAttribute('id', 'days');
    var monthHeader = document.createElement('tr');
    monthHeader.setAttribute('id', 'title');
    var monthHeaderTD = document.createElement('th');
    var prev = document.createElement('th');
    prev.setAttribute('id', 'prev');
    prev.onclick = function() { window[id].prevMonth(); };
    prev.style.cursor = 'pointer';
    var imgPrev = document.createElement('img');
    imgPrev.src = this.ImgPrev;
    prev.appendChild(imgPrev);
    monthHeader.appendChild(prev);
    monthHeaderTD.colSpan = 5;
    monthHeaderTD.innerHTML = this.MonthNames[m] + ' ' + this.currYear;
    for (rci = 0; rci < 7; rci++) {
        var td = document.createElement('th');
        td.setAttribute('id', this.ShortDayNames[rci].toLowerCase());
        td.innerHTML = this.ShortDayNames[rci].substring(0, 1);
        td.style.width = (100 / 7) + '%';
        header.appendChild(td);
    };
    monthHeader.appendChild(monthHeaderTD);
    var next = document.createElement('th');
    next.setAttribute('id', 'next');
    next.style.cursor = 'pointer';
    next.onclick = function() { window[id].nextMonth(); };
    var imgNext = document.createElement('img');
    imgNext.src = this.ImgNext;
    next.appendChild(imgNext);
    monthHeader.appendChild(next);
    tbody.appendChild(monthHeader);
    tbody.appendChild(header);
    table.appendChild(tbody);


    //New cal
    var preDays = calMonth.getDay();
    if (preDays == 0) {
        preDays = 7;
    };
    var m = this.Month;
    var y = this.Year;
    var days = parseFloat(preDays) + this.MonthDays[m];
    var startDate = new Date(calMonth);
    startDate = new Date(startDate.addDays(-(preDays - 1)))
    var endDate = new Date(startDate);
    days = (42 - days) + days;
    endDate = new Date(endDate.addDays(days));
    var weekdays = 0;
    var tr = null;
    var td = null;
    var tDays = 0;
    var weekCount = 0;
    for (dc = 0; dc < 42; dc++) {
        tDays += 1;
        var tmpDate = new Date(startDate);
        tmpDate = new Date(tmpDate.addDays(tDays));
        if (weekdays == 0) {
            tr = document.createElement('tr');
            if (weekCount == 0) {
                tr.setAttribute('id', 'firstweek');
            } else if (weekCount == 5) {
                tr.setAttribute('id', 'lastweek');
            };
            weekCount += 1
        };
        weekdays += 1
        var td = document.createElement('td');
        if (tmpDate.getMonth() == calMonth.getMonth()) {
            dayClass = 'currmonth';
        } else if (tmpDate.getMonth() == startDate.getMonth()) {
            dayClass = 'prevmonth';
        } else if (tmpDate.getMonth() == endDate.getMonth()) {
            dayClass = 'nextmonth';
        };
        var isToday = false;
        var mabbr = this.ShortMonthNames[tmpDate.getMonth()];
        dayClass += ' ' + this.ShortDayNames[tmpDate.getDay()].toLowerCase();
        var today = new Date();
        if (tmpDate.getMonth() == today.getMonth() && tmpDate.getDate() == today.getDate()) {
            dayClass += ' today';
            isToday = true;
        };

        td.className = dayClass.toLowerCase();
        var nDay = tmpDate.getDate().toString().pad(2, '0', 0);
        var oDate = tmpDate.getFullYear() + '' + (tmpDate.getMonth() + 1).toString().pad(2, '0', 0) + '' + nDay;
        td.setAttribute('Day', tmpDate.getDate());
        td.setAttribute('Month', tmpDate.getMonth());
        td.setAttribute('Year', tmpDate.getFullYear());
        td.setAttribute('id', oDate);
        td.onclick = function() { window[id].Set(this); }
        td.innerHTML = nDay;
        //        var dte = document.createElement('div');
        //        dte.className = 'date';
        //        dte.innerHTML = nDay;
        //        dte.onclick = function() { window[id].clickDate(this.parentNode.id, this.parentNode); };
        //        td.appendChild(dte);

        tr.appendChild(td);



        if (weekdays == 7) {
            tbody.appendChild(tr);
            tr = null;
            weekdays = 0;
        };
    };
    if (tr != null) {
        tbody.appendChild(tr);
        tr = null;
    };


    if (this.ShowTime == true) {
        var tr = document.createElement('tr');
        tr.setAttribute('id', 'time');
        var td = document.createElement('td');
        td.setAttribute('colSpan', '2');
        td.innerHTML = this.timeLabel;
        tr.appendChild(td);
        var td = document.createElement('td');
        td.setAttribute('colSpan', '5');
        td.setAttribute('id', 'timeselect');
        td.appendChild(this.buildTime());
        tr.appendChild(td)
        tbody.appendChild(tr);
    };
    this.calendar.appendChild(table);

};
asDatePicker.prototype.Toggle = function(e) {
    if (this.calendar.style.display == 'block') {
        this.calendar.style.display = 'none';
    } else {
        this.calendar.style.display = 'block';
        var pos = getPositionPicker(this.image);
        if (!e) { e = window.event; };
        
        this.calendar.style.left = pos.x - this.Width + 16 + 'px';
        
        var textBoxPosition = this.textbox.getBoundingClientRect();
        this.calendar.style.top = textBoxPosition.top;
        this.calendar.style.zIndex = 1000;

        this.Show();
    };
};
Date.prototype.formatDP = function(f, id) {
    if (!this.valueOf()) { return '&nbsp;'; }; var d = this;
    return f.replace(/(yyyy|MMMM|MMM|MM|M|dddd|ddd|dd|d|hh|h|HH|mm|m|ss|s|tt|t)/gi,
        function($1) {
            switch ($1) {
                case 'yyyy': return d.getFullYear();

                case 'MMMM': return window[id].MonthNames[d.getMonth()];
                case 'MMM': return window[id].ShortMonthNames[d.getMonth()];
                case 'MM': return (d.getMonth() + 1).zf(2);
                case 'M': return (d.getMonth() + 1);
                case 'dddd': return window[id].DayNames[d.getDay()];
                case 'ddd': return window[id].DayNames[d.getDay()].substr(0, 3);
                case 'dd': return d.getDate().zf(2);
                case 'd': return d.getDate();
                case 'hh': return ((h = d.getHours() % 12) ? h : 12).zf(2);
                case 'h': return ((h = d.getHours() % 12) ? h : 12);
                case 'HH': return d.getHours().zf(2);
                case 'mm': return d.getMinutes().zf(2);
                case 'm': return d.getMinutes();
                case 'ss': return d.getSeconds().zf(2);
                case 's': return d.getSeconds();
                case 'tt': return d.getHours() < 12 ? 'AM' : 'PM';
                case 't': return d.getHours() < 12 ? 'a' : 'p';
            }
        }
    )
};
String.prototype.zf = function(l) { return '0'.string(l - this.length) + this; };
String.prototype.string = function(l) { var s = '', i = 0; while (i++ < l) { s += this; }; return s; };
Number.prototype.zf = function(l) { return this.toString().zf(l); }
function getPositionPicker(e) {
    var left = 0, top = 0;
    while (e != document.getElementsByTagName('body')[0]) {
        if (e.style.position != 'relative') {
            if (e.style.position == 'absolute') {
                return { x: left, y: top };
            } else {
                left += e.offsetLeft;
                top += e.offsetTop;
            };
            e = e.offsetParent
        } else {
            e = e.parentNode;
        };
    };
    if (e.offsetParent) {
        left += e.offsetLeft;
        top += e.offsetTop;
    };
    return { x: left, y: top };
};
asDatePicker.prototype.buildTime = function() {
    var id = this.id;
    var ddl = document.createElement('select');
    //ddl.className = 'amtextbox';
    ddl.id = 'time_' + this.id;
    ddl.onchange = function() { window[id].SetTime(this); };
    var dx = 0;
    var d = new Date();
    var t = d.getDay() + 1;
    d.setHours(0, 0, 0, 0);
    if (dx == 0 && this.RequireTime == false) {
        ddl.options[0] = new Option('-----', '')
        dx += 1
    };

    while (d.getDay() < t) {
        var time = d.formatDP(this.TimeFormat, id)
        ddl.options[dx] = new Option(time, time)
        if (time == this.DefaultTime && this.SelectedTime == '') { ddl.selectedIndex = dx; };
        if (time == this.SelectedTime) { ddl.selectedIndex = dx; };
        dx += 1
        if (d.getMinutes() == 45) {
            d.setHours(d.getHours() + 1, 0, 0, 0)
        } else {
            d.setHours(d.getHours(), d.getMinutes() + 15, 0, 0)
        };

    };



    return ddl;
}

Date.prototype.addDays = function(days) {
    var d = this;
    return d.setDate(d.getDate() + (days - 1));

};
Date.prototype.addMinutes = function(minutes) {
    var d = this;
    var m = d.getMinutes() + 15;
    if (m == 0) {
        d.setHours(d.getHours() + 1);

    };
    return d.setMinutes(m - 1);

};
String.prototype.pad = function(l, s, t) {
    return s || (s = " "), (l -= this.length) > 0 ? (s = new Array(Math.ceil(l / s.length)
        + 1).join(s)).substr(0, t = !t ? l : t == 1 ? 0 : Math.ceil(l / 2))
        + this + s.substr(0, l - t) : this;
};
asDatePicker.prototype.nextMonth = function() {
    var m = 0;
    var y;
    if (this.currMonth == 11) {
        m = 0;
        y = this.currYear + 1;
    } else {
        m = this.currMonth + 1
        y = this.currYear;
    };
    this.Show(m, y);
    //preRender(m, y);

};

asDatePicker.prototype.prevMonth = function() {
    var m = 0;
    var y;
    if (this.currMonth == 0) {
        m = 11;
        y = this.currYear - 1;
    } else {
        m = this.currMonth - 1;
        y = this.currYear;
    };
    this.Show(m, y)
    //this.preRender(m, y);
};
asDatePicker.prototype.onlyDateChars = function(evt) {
    var charCode = (evt.which != undefined) ? evt.which : evt.keyCode;
    if (charCode == 47 || charCode == 45 || charCode == 46 || charCode == 58)
        return true;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && (charCode != 46) && (charCode != 44))
        return false;
    return true;
};
