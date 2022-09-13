"use strict"

import http from "../../../helpers/axiosClient"

export const FetchPoint = (id,page) => {
   let params = {
      pageindex:page
   }
   return  http.get(`/Customer/Customer/${id}/Point`,{params:params})
}