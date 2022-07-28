// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Challenge } from "./Challenge";

export class ChallengesResults {
    items?: Challenge[];
    pageNumber?: number;
    totalPages?: number;
    totalCount?: number;
    hasPreviousPage!: boolean
    hasNextPage!: boolean
}