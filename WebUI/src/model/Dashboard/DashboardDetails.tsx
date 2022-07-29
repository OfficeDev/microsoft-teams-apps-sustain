// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { DashboardRankModel } from "./DashboardRank";

export class DashboardDetailsModel {
    username?: string;
    currentPoints?: number;
    currentRankLabel?: string;
    nextRankLabel?: string;
    remainingPoints?: string;
    minScore?: number;
    maxScore?: number;
    dashboardRanks?: DashboardRankModel[];
}