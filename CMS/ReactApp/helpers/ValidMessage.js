"use strict"

const validMessage = {
    required:"Trường này là bắt buộc",
    email:"Sai định dạng email",
    min:function (length) {
        return `Độ dài tối thiếu ${length} ký tự`
    },
    max:function (length) {
        return `Độ dài tối đa ${length} ký tự`
    },
}

export default validMessage