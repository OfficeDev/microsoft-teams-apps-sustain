import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";

import { LeaderboardModel } from "model/Leaderboards/LeaderboardModel";

export const getLeaderboard = async (count: number, fileteredByOrg: boolean): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Leaderboard?count=" + count + "&filteredByOrg=" + fileteredByOrg;

    return await axios.get<LeaderboardModel>(url);
};