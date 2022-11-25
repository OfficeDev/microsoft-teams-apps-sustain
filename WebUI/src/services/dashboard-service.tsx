import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";

import { DashboardDetailsModel } from "model/Dashboard/DashboardDetails";

export const getDashboardDetails = async (): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Dashboard";

    return await axios.get<DashboardDetailsModel>(url);
};