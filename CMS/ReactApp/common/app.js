import Moment from "moment";
export const isAllowedNumberThan0 = (values) => {
    if (values.value.length == 0) {
        return true;
    }
    if (values.floatValue >= 0) {
        return true;
    }
    return false;

};

export const isAllowedNumberIntThan0AndMax = (values,max) => {
    if (values.value.length == 0) {
        return true;
    }
    if (values.floatValue >= 0 && Number.isInteger(values.floatValue) && values.floatValue <= max) {
        return true;
    }
    return false;
};

export const isAllowedNumberIntThan0 = (values) => {
    if (values.value.length == 0) {
        return true;
    }
    if (values.floatValue >= 0 && Number.isInteger(values.floatValue)) {
        return true;
    }
    return false;
};

export const formatNumber = (value, type = 0) => {
    // return !Number.isNaN(Number.parseFloat(value))
    //     ? (Number.parseFloat(value).toLocaleString("en-US", {maximumFractionDigits: maximumFractionDigits}))
    //     : null;
    if(!Number.isNaN(Number.parseInt(value))){
        if(value ==="N/A"){
            return ""
        }
        else {
            //return Number.parseFloat(value).toLocaleString("en-US", {maximumFractionDigits: maximumFractionDigits})
            return Number.parseInt(value).toLocaleString("de-DE", {
                minimumFractionDigits: type, 
                maximumFractionDigits: type})
        }
    }
    else {
        return null;
    }
};
export const formatNumberDivision = (value1, value2, type = 1) => {
    if (!value2 || value2 == 0) {
        return "N/A";
    } else {
        let value = value1 / value2;
        return !Number.isNaN(Number.parseFloat(value))
            ? Number.parseFloat(value).toLocaleString("en-US", {
                minimumFractionDigits: type,
                maximumFractionDigits: type})
            : null;
    }
};

export const formatNumberDivisionPercent = (value1, value2, type = 1) => {
    if (!value2 || value2 == 0) {
        // return "N/A";
         return "";
    } else {
        let value = value1 / value2 * 100;
        return !Number.isNaN(Number.parseFloat(value))
            ? Number.parseFloat(value).toLocaleString("en-US", {
                minimumFractionDigits: type,
                maximumFractionDigits: type}) + "%"
            : null;
    }
};

export const formatNumberRate = (value1, value2, type = 1) => {
    if (!value2 || value2 == 0) {
        return "N/A";
    } else {
        let value = (value1 / value2) * 100;
        var p = Math.pow(10, type);
        var n = (value * p) * (1 + Number.EPSILON);
        var rs = Math.round(n) / p;
        return !Number.isNaN(Number.parseFloat(rs))
            ? Number.parseFloat(rs).toLocaleString("en-US", {
                minimumFractionDigits: type,
                maximumFractionDigits: type})
            : null;
    }
};
export const formatDateTime = (dateTime) => {
    if (dateTime) {
        let options = {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit',
            hour12: false
        };
        const date = new Date(dateTime);
        return new Intl.DateTimeFormat('vi-VN', options).format(date)
    }
    return ""
}
export const formatDateTimeNoHour = (dateTime) => {
    if (dateTime) {
        let options = {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour12: false
        };
        const date = new Date(dateTime);
        return new Intl.DateTimeFormat('vi-VN', options).format(date)
    }
    return ""
}
export const formatDateEndCoupon = (dateTime)=> {
    if (dateTime) {
        let dateNew = Moment(dateTime).set({'hour': 23, 'minute': 59})
        let options = {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit',
            hour12: false
        };
        const date = new Date(dateNew);
        return new Intl.DateTimeFormat('vi-VN', options).format(date)
    }
    return ""
}
export const formattedDate = (d = new Date) => {
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()]
        .map(n => n < 10 ? `0${n}` : `${n}`).join('/');
}

export const formattedDateM = (d = new Date) => {
    return [1, d.getMonth() + 1, d.getFullYear()]
        .map(n => n < 10 ? `0${n}` : `${n}`).join('/');
}

export const formattedDateMonth = (d = new Date) => {
    return [d.getMonth() + 1, d.getFullYear()]
        .map(n => n < 10 ? `0${n}` : `${n}`).join('/');
}

export const formattedDateFirstMonth = (d = new Date(new Date().getFullYear(), 0, 1)) => {
    return [d.getMonth() + 1, d.getFullYear()]
        .map(n => n < 10 ? `0${n}` : `${n}`).join('/');
}

export const formattedDateFirstYear = (d = new Date(new Date().getFullYear(), 0, 1)) => {
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()]
        .map(n => n < 10 ? `0${n}` : `${n}`).join('/');
}

export const isHtml = (text) => {
    if (text != null && text.length > 0) {
        let html = text.match(/<.+>/g);
        return html != null;
    }
    return false;
}
export const parStr2Float = str => parseFloat(str.toString().replace(",", "."));
export const parFloat2Str = value => {
    if (!value || Number.isNaN(Number.parseFloat(value))) {
        return 0;
    }
    return Number.parseFloat(value).toLocaleString("vi", { maximumFractionDigits: 10 });
};
export const parInt2Str = value => {
    if (!value || Number.isNaN(Number.parseInt(value))) {
        return 0;
    }
    return Number.parseInt(value).toLocaleString("vi", { maximumFractionDigits: 10 });
};
export const isNumber = (num) => {
    return !isNaN(num);
}
export const isPoi = 1000;