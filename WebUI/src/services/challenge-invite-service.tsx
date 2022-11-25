import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";
import { NotificationRequest } from "model/Notification/NotificationRequest";

export const sendActivityNotification = async (request: NotificationRequest): Promise<any> => {
    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/ChallengeInvite/SendChallengeNotification";
    return await axios.post(url, request);
};

export const getUsers = async (): Promise<any> => {
    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/ChallengeInvite/GetUsers";
    return await axios.get(url);
};