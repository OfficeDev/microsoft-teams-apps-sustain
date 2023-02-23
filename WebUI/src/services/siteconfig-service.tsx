import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";
import { SiteConfig } from "model/SiteConfig/SiteConfig";
import { SiteConfigResult } from "model/SiteConfig/SiteConfigResult";

export const getSiteConfig = async (serviceType: number): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/SiteConfig?pageNumber=1&pageSize=1&serviceType=" + serviceType;

    return await axios.get<SiteConfigResult>(url);
};

export const saveSiteConfig = async (serviceType: number, uri: string, isEnabled: boolean, newsEnable: boolean, eventsEnabled: boolean, yammerGroupId?: string): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/SiteConfig";

    return await axios.post<SiteConfig>(url,
        {
            'serviceType': serviceType,
            'uri': uri,
            'isEnabled': isEnabled,
            'isNewsEnabled': newsEnable,
            'isEventsEnabled': eventsEnabled,
            'yammerGroupId': yammerGroupId ? yammerGroupId : ''
        });
};