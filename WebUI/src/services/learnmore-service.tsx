// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios from "common/AxiosJWTDecorator";
import { ChallengeRecord } from "model/Challenge/ChallengeRecord";
import { ChallengesResults } from "model/Challenge/ChallengeResult";

export const getChallengesbyid = async (id: any, forManagement: boolean = false): Promise<any> => {
    let url = process.env.REACT_APP_BASE_URL + "/api/challenge/GetChallengebyId?id=" + id;

    if (forManagement) {
        url = url + "&forManagement=" + forManagement;
    }

    return await axios.get<ChallengesResults>(url);
};

export const getChallengeStatus = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/challenge/GetChallengeStatus?id=" + id;

    return await axios.get<ChallengeRecord>(url);
};

export const getPercentage = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/challenge/GetCompletionPercentage?id=" + id;

    return await axios.get<ChallengeRecord>(url);
};

export const getNumCompleted = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/challenge/GetNumUsersCompleted?id=" + id;

    return await axios.get<ChallengeRecord>(url);
};


export const getTotalAccepted = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/challenge/GetNumUsersAccepted?id=" + id;

    return await axios.get<ChallengeRecord>(url);
};

export const acceptChallenge = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/ChallengeRecord";

    return await axios.post<ChallengeRecord>(url, { 'challengeId': id });
};

export const completeChallenge = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/ChallengeRecord";

    return await axios.put<ChallengeRecord>(url,
        {
            'challengeId': id
        }
    );
};

export const leaveChallenge = async (id: any): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/ChallengeRecord";

    return await axios.delete<ChallengeRecord>(url, {
        headers: {
            'Content-Type': 'application/json'
        },
        data: {
            'challengeId': id
        }
    });
};
