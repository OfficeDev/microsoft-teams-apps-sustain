// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios from "common/AxiosJWTDecorator";
import { DashboardDetailsModel } from "model/Dashboard/DashboardDetails";

export const getDashboardDetails = async (): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/Dashboard";
    return await axios.get<DashboardDetailsModel>(url);
};