import { AsyncTypeahead } from 'react-bootstrap-typeahead';
import React, {Fragment ,useState,useCallback} from 'react';
import http from "../../JsFiles/helpers/axiosClient";

const InputTypeahead = (props) => {
    let {id,labelKey,placeholder,name,formik, api,maxLength} = props;
    const [isLoading, setIsLoading] = useState(false);
    const [options, setOptions] = useState([]);
    const [query, setQuery] = useState('');

    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    let defaultValue = meta.value;
    const handleInputChange = (text, e) => {
        setQuery(text);
        const changeEvent = {
            target: {
                name: name,
                value: text
            }
        };
        formik.handleChange(changeEvent);
    }
    const handleSearch = useCallback((query) => {
        setIsLoading(true);
        http.get(`${api}${query}`).then((data) => {
            if (data.code === 200){
                if (data.content.length > 0){
                    const options = data.content.map((i) => ({
                        id: i.id,
                        name: i.name,
                    }));
                    setOptions(options);
                }else{
                    setOptions([]);
                }
            }
            setIsLoading(false);
        })
    }, []);
    const handleChange = (data) => {
        if (data.length > 0){
            const changeEvent = {
                target: {
                    name: name,
                    value: data[0].name
                }
            };
            formik.handleChange(changeEvent);   
        }
    }
    const filterBy = () => true;
    return (
        <>
            <AsyncTypeahead
                id={id}
                filterBy={filterBy}
                minLength={1}
                maxLength={maxLength}
                isLoading={isLoading}
                labelKey={labelKey}
                onSearch={handleSearch}
                onInputChange={handleInputChange}
                onChange={handleChange}
                defaultInputValue={defaultValue}
                options={options}
                placeholder={placeholder}
                renderMenuItemChildren={(option, props) => (
                    <Fragment>
                        <span>{option.name}</span>
                    </Fragment>
                )}
            />
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
};

export default InputTypeahead;