// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import axios from "common/AxiosJWTDecorator";
import { LeaderboardModel } from "model/Leaderboards/LeaderboardModel";

export const getLeaderboard = async (count: number, fileteredByOrg: boolean): Promise<any> => {
    const url = process.env.REACT_APP_BASE_URL + "/api/Leaderboard?count=" + count + "&filteredByOrg=" + fileteredByOrg;
    return await axios.get<LeaderboardModel>(url);
};