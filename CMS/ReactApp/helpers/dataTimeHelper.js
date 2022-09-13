"use strict"

export const dataFormat = (dateTime) => {
    if (dateTime) {
        let options = {
            year: 'numeric', month: '2-digit', day: '2-digit'
        };
        const date = new Date(dateTime);
        return new Intl.DateTimeFormat('vi-VN', options).format(date)
    }
    return ""
}