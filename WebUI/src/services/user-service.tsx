// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios from "common/AxiosJWTDecorator";

export const getMe = async (): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/user/me";

    return axios.get(url);
};

export const getUser = async (email: string): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/user/single?email=" + email;

    return axios.get(url);
};

export const getAllUsers = async (pageNumber: number, pageSize: number, search: any, role: any): Promise<any> => {
    const params = [
        'pageNumber=' + pageNumber,
        'pageSize=' + pageSize,
        'search=' + search,
        'role=' + role
    ];

    const paramsStr = params.join('&');

    const url = process.env.REACT_APP_BASE_URL + "/api/user?" + paramsStr;

    return axios.get(url);
};

export const searchUsers = async (query: string): Promise<any> => {
    const params = [
        'query=' + query
    ];

    const paramsStr = params.join('&');

    const url = process.env.REACT_APP_BASE_URL + "/api/user/search?" + paramsStr;

    return axios.get(url);
};


export const addUser = async (users: string[], role: string): Promise<any> => {
    const request: any = {
        Emails: users,
        Role: role
    };

    const url = process.env.REACT_APP_BASE_URL + "/api/user";

    return axios.post(url, request);
};

export const editUser = async (email: string, role: string): Promise<any> => {
    const request: any = {
        Email: email,
        Role: role
    };

    const url = process.env.REACT_APP_BASE_URL + "/api/user";

    return axios.put(url, request);
};

export const deleteUser = async (email: string): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/email=" + email;

    return axios.delete(url);
};