// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { LeaderboardUser } from "./LeaderboardUser";

export class LeaderboardItem {
    id?: number;
    user?: LeaderboardUser[];
    currentPoints?: number;
    dateCreated?: string;
    dateModified?: string;
}
