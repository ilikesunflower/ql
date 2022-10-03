import axios from "axios";

export const state = (dataSetUp)=>{
    return {
        dataSetUp:dataSetUp,
        fileFilter : {
            PageSize: 24,
            txtSearch: "",
            Type: dataSetUp.type || "",
            CreateAt:""
        },
        isLoading : false,
        NextPage:null,
        ListFiles: [],
        ListCreateAt: [],
        ListTypes: [],
        ActiveFile:{},
        FileBeingUploaded:{}
    }
};

export const getters = {
    StateListFiles: state => state.ListFiles,
    StateIsLoading: state => state.isLoading,
    StateActiveFile: state => state.ActiveFile,
    StateListTypes: state => state.ListTypes,
    StateListCreateAt: state => state.ListCreateAt,
    StateListActiveFiles: state => {
        if (state.dataSetUp.multiSelect){
            return state.ListFiles.filter(file => file.isActive)
        }else {
            return state.ListFiles.find(file => file.isActive)
        }
    },
    StateFileBeingUploaded: state => state.FileBeingUploaded
};

export const mutations = {
    
    unShiftListTypes(state , file){
        state.ListFiles.unshift(file);
    },
    setFileBeingUploaded(state,listFile){
        state.FileBeingUploaded = listFile;
    },
    setLoading(state){
        state.isLoading = true;
    },
    setLoaded(state){
        state.isLoading = false;
    },
    PushToFileBeingUploaded(state ,{index, info}){
        state.FileBeingUploaded[index] = info;
    },
    UpdateFileBeingUploaded(state ,{index, field ,data}){
        state.FileBeingUploaded[index][field] = data;
    },
    setKeyword(state , keyword){
        state.fileFilter.txtSearch  = keyword
    },
    setType(state , Type){
        state.fileFilter.Type  = Type
    },
    setCreateAt(state , createAt){
        state.fileFilter.CreateAt  = createAt
    },
    setNextPage(state,nextPage){
        state.NextPage  = nextPage
    },
    setListFiles(state,listFiles) {
        state.ListFiles = listFiles;
    },
    setListTypes(state,listTypes){
        state.ListTypes = listTypes;
    },
    setListCreateAt(state,listCreateAt){
        state.ListCreateAt = listCreateAt;
    },
    setActive(state,_file){
        _file.isActive = !_file.isActive;
        if(!state.dataSetUp.multiSelect && state.ActiveFile.id !== _file.id ){
            state.ActiveFile.isActive = false;
        }
        if(_file.isActive === true){
            state.ActiveFile = _file;
        }else{
            state.ActiveFile = {};
        }
    },
    setUnActive(state){
        for(let i = 0 ; i < state.ListFiles.length ; i++){
            state.ListFiles[i].isActive = false;
        }
        state.ActiveFile = {};
    },
    removeActiveFile(state){
        state.ActiveFile = {};
    },
    setRemoveFile(state,_file){
        if(_file.id === state.ActiveFile.id){
            state.ActiveFile = {};
        }
        let indexOfFile = state.ListFiles.findIndex( file => file.id === _file.id );
        state.ListFiles.splice(indexOfFile, 1);
    }
};

export const actions = {
    setDataUnActive({commit}){
        commit("setUnActive");
    },

    setDataKeyword({commit},keyword){
        commit("setKeyword",keyword);
    },

    setDataType({commit},Type){
        commit("setType",Type);
    },

    setDataCreateAt({commit},createAt){
        commit("setCreateAt",createAt);
    },

    fetchFiles({commit, state}) {
        commit("setLoading")
        axios.get("/Admin/File/GetAllListFile",{params: {...state.fileFilter}})
            .then((response) => {
                let resData = response.data;
                if (resData.succeeded){
                    let listFiles =  resData?.listFiles?.map(function (file) {
                        file.isActive = false;
                        return file;
                    });
                    commit('setNextPage',resData.nextPage)
                    commit('setListFiles',listFiles)
                    commit('removeActiveFile')
                    commit("setLoaded")
                }
            })
    },

    fetchFilters({commit, state}) {
        commit("setLoading")
        axios.get("/Admin/File/GetFilterFile")
            .then((response) => {
                let resData = response.data;
                if (resData.succeeded){
                    commit("setListTypes",resData.content.listTypes);
                    commit("setListCreateAt",resData.content.listCreatedAt);
                }
                commit("setLoaded")
            })
    },

    loadMoreFiles({commit, state}){
        if(state.NextPage){
            commit("setLoading")
            axios.get(state.NextPage,{
                params: {
                    ...state.fileFilter,
                    pageindex:state.pageindex
                }})
                .then((response) => {
                    let resData = response.data;
                    if (resData.succeeded){
                        let listNewFiles =  resData?.listFiles?.map(function (file) {
                            file.isActive = false;
                            return file;
                        });
                        let listFiles = [...state.ListFiles,...listNewFiles];
                        commit('setNextPage',resData.nextPage)
                        commit('setListFiles',listFiles)
                    }
                    commit("setLoaded")
                })
        }
    },

    uploadFile({commit,state}, {file , url}){
        let index;
        let oldIndex = Object.keys(state.FileBeingUploaded);
        do {
            index = Math.random().toString(36).substring(7);
        }
        while (oldIndex.includes(index));
        commit("PushToFileBeingUploaded",{index : index , info :{
                urlPreview:url,
                status:0,
                name: file.name,
                process: 0,
                message: "",
            } });
        
        let formData = new FormData();
        formData.append('file', file);
        commit("setLoading")
        axios.post( "/Admin/File/UploadAFile",formData,
            {
                headers: {
                    "RequestVerificationToken": state.dataSetUp.xsrf,
                    'Content-Type': 'multipart/form-data'
                },
                onUploadProgress: progressEvent => {
                    let percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
                    commit("UpdateFileBeingUploaded",{index : index ,field:'process', data:percentCompleted });
                }
            }
        ).then(function(resqponse){
            let resData = resqponse.data;
            if (resData.msg === "successful"){
                commit('unShiftListTypes',resData.content.file);
                commit("UpdateFileBeingUploaded",{index : index ,field:'status', data:1 });
                commit("UpdateFileBeingUploaded",{index : index ,field:'message', data:resData.msg  });
            }else{
                commit("UpdateFileBeingUploaded",{index : index ,field:'status', data:2 });
                commit("UpdateFileBeingUploaded",{index : index ,field:'message', data:resData.content  });
            }
            commit("setLoaded")
        })
        .catch(function(error){
            commit("UpdateFileBeingUploaded",{index : index ,field:'status', data:2 });
            commit("UpdateFileBeingUploaded",{index : index ,field:'message', data:error });
            commit("setLoaded")
        });
    },

    activeFiles({commit},file){
        commit('setActive',file)
    },

    removeFile({commit,state},file){
        let confirmDelete;
        if (state.dataSetUp.confirmDelete){
            confirmDelete = state.dataSetUp.confirmDelete;
        }else{
            confirmDelete = (DeleteFile)=>{
                if(confirm("Bạn có chắc chắn xóa dữ liệu này?")){
                    DeleteFile();
                }
            };
        }
        confirmDelete(()=>{
            commit("setLoading")
            axios({
                method: 'post',
                headers: {
                    "RequestVerificationToken": state.dataSetUp.xsrf,
                },
                url:  `/Admin/File/Delete/${file.id}`
            }).then((response) => {
                let resData = response.data;
                if (resData.msg === "successful"){
                    commit('setRemoveFile',file)
                }
                commit("setLoaded")
            })
        })
    },
    
    setDefaultsFileBeingUploaded({commit}){
        commit('setFileBeingUploaded',{});
    }
};

