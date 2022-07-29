// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { SiteConfig } from "./SiteConfig";

export class SiteConfigResult {
    items: SiteConfig[];
    pageNumber?: number;
    totalPages?: number;
    totalCount?: number;
}