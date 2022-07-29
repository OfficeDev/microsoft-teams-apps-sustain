// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export class ChallengeRecord {
    id?: number;
    userId?: number;
    challengeId?: number;
    challengePoint?: number;
    status?: number;
    created?: string;
    createdBy?: string;
    lastModified?: string;
    lastModifiedBy?: string;
    completeYesterday?: boolean;
    completeToday?: boolean;
}