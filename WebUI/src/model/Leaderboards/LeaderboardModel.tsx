// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { LeaderboardItem } from "./LeaderboardItem";

export class LeaderboardModel {
    items?: LeaderboardItem[];
    pageNumber?: number;
    totalPages?: number;
    totalCount?: number;
}