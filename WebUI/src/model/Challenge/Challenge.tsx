// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ChallengeRecord } from "./ChallengeRecord";

export class Challenge {
    id?: number;
    title?: string;
    points?: number;
    thumbnail?: string;
    description?: string;
    activeUntil?: string;
    recurrence?: number;
    focusArea?: string;
    additionalResources?: string;
    isActive?: boolean;
    isPinned?: boolean;
    challengeRecords?: ChallengeRecord[];
    relatedUsers?: any;
    finalStatus?: number;
}