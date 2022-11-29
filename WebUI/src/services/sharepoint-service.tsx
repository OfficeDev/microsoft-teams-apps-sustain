import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";
import { Event } from "model/SharePoint/Event";
import { EventResultSet } from "model/SharePoint/EventResultSet";
import { News } from "model/SharePoint/News";

export const getNews = async (): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/News/recent";

    return await axios.get<News>(url);
};

export const getEvents = async (): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Events/upcoming";

    return await axios.get<EventResultSet>(url);
};