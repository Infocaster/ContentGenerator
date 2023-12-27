import { Axios } from "axios";

// We have to strip the first 6 characters before we parse the data as JSON. This is an angularJS security feature.
export const axiosInstance = new Axios({
    transformResponse: [
        (data) => {
            return JSON.parse(data.substring(6));
        }
    ]
});