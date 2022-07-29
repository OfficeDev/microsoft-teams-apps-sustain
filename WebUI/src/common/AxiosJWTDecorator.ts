// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios, { AxiosResponse, AxiosRequestConfig } from "axios";
import { authentication } from "@microsoft/teams-js";

interface AxiosJWTDecoratorRequestConfig<D = any> extends AxiosRequestConfig<D> {
    redirectOnError?: boolean;
}

class AxiosJWTDecorator {
    public async get<T = any, R = AxiosResponse<T>, D = any>(url: string, config?: AxiosJWTDecoratorRequestConfig<D>): Promise<R> {
        try {
            config = await this._ensureAuthHeader(config);
            return axios.get(url, config);
        } catch (e) {
            this._processError(e, config);
        }
    }

    public async getAnonymous<T = any, R = AxiosResponse<T>, D = any>(url: string, config?: AxiosJWTDecoratorRequestConfig<D>): Promise<R> {
        try {
            return axios.get(url, config);
        } catch (e) {
            this._processError(e, config);
        }
    }

    public async delete<T = any, R = AxiosResponse<T>, D = any>(url: string, config?: AxiosJWTDecoratorRequestConfig<D>): Promise<R> {
        try {
            config = await this._ensureAuthHeader(config);
            return axios.delete(url, config);
        } catch (e) {
            this._processError(e, config);
        }
    }

    public async post<T = any, R = AxiosResponse<T>, D = any>(url: string, data?: D, config?: AxiosJWTDecoratorRequestConfig<D>): Promise<R> {
        try {
            config = await this._ensureAuthHeader(config);
            return axios.post(url, data, config);
        } catch (e) {
            this._processError(e, config);
        }
    }

    public async put<T = any, R = AxiosResponse<T>, D = any>(url: string, data?: D, config?: AxiosJWTDecoratorRequestConfig<D>): Promise<R> {
        try {
            config = await this._ensureAuthHeader(config);
            return axios.put(url, data, config);
        } catch (e) {
            this._processError(e, config);
        }
    }

    private _processError<D>(error: any, config: AxiosJWTDecoratorRequestConfig<D> | undefined): never {
        if (config?.redirectOnError !== false)
            this._redirectOnError(error);
        else
            throw error;
    }

    private _redirectOnError(error: any): never {
        const status: number = error?.response?.status;

        switch (status) {
            case 401:
            case 403:
                window.location.assign(`/error?code=${status}`);
                break;
            default:
                window.location.assign("/error");
        }

        // eslint-disable-next-line
        throw undefined;
    }

    private async _ensureAuthHeader<D>(config?: AxiosJWTDecoratorRequestConfig<D>): Promise<AxiosJWTDecoratorRequestConfig<D>> {
        config ||= {};
        config.headers ||= {};

        try {
            const token = await authentication.getAuthToken();
            config.headers["Authorization"] = `Bearer ${token}`;
        } catch (e) {
            console.error("Teams authentication.getAuthToken() failed", e);
        }

        return config;
    }
}

const axiosJWTDecoratorInstance = new AxiosJWTDecorator();
export default axiosJWTDecoratorInstance;