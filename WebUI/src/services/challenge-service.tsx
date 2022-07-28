// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios from "common/AxiosJWTDecorator";
import { Challenge } from "model/Challenge/Challenge";

export const getChallenges = async (pageNumber: number, pageSize: number, status: any): Promise<any> => {
    const params = [
        'pageNumber=' + pageNumber,
        'pageSize=' + pageSize,
    ];

    if (status > -1) {
        params.push('status=' + status);
    }

    const paramsStr = params.join('&');

    const url = process.env.REACT_APP_BASE_URL + "/api/challenge?" + paramsStr;

    return axios.get(url);
};

export const getChallengesManagement = async (pageNumber: number, isArchived: boolean, pageSize: number, keyword: any): Promise<any> => {
    const params = [
        'pageNumber=' + pageNumber,
        'pageSize=' + pageSize,
        'isArchived=' + isArchived
    ];

    if (keyword) {
        params.push('keyword=' + keyword)
    }

    const paramsStr = params.join('&');

    const url = process.env.REACT_APP_BASE_URL + "/api/challenge/management?" + paramsStr;

    return axios.get(url);
};

export const editCallenge = async (challenge: Challenge, file: File): Promise<any> => {
    const formData = new FormData();
    formData.append("Id", challenge.id.toString());
    formData.append("Title", challenge.title);
    formData.append("IsPinned", challenge.isPinned.toString());
    formData.append("Recurrence", challenge.recurrence.toString());
    formData.append("Points", challenge.points.toString());
    formData.append("Description", challenge.description);
    formData.append("ActiveUntil", challenge.activeUntil);
    formData.append("FocusArea", challenge.focusArea);
    formData.append("AdditionalResources", challenge.additionalResources);

    if (file) {
        formData.append("ThumbnailFile", file);
        formData.append("ThumbnailFilename", file.name);
    }

    const url = process.env.REACT_APP_BASE_URL + "/api/challenge";

    return axios.put(url, formData);
}

export const addCallenge = async (challenge: Challenge, file: File): Promise<any> => {
    const formData = new FormData();
    formData.append("Title", challenge.title);
    formData.append("IsPinned", challenge.isPinned.toString());
    formData.append("Recurrence", challenge.recurrence.toString());
    formData.append("Points", challenge.points.toString());
    formData.append("Description", challenge.description);
    formData.append("ActiveUntil", challenge.activeUntil);
    formData.append("FocusArea", challenge.focusArea);
    formData.append("AdditionalResources", challenge.additionalResources);

    if (file) {
        formData.append("ThumbnailFile", file);
        formData.append("ThumbnailFilename", file.name);
    }

    const url = process.env.REACT_APP_BASE_URL + "/api/challenge";

    return axios.post(url, formData);
}