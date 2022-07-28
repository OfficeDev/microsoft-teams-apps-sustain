// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { CarouselModel } from "./Carousel";

export class CarouselResult {
    items: CarouselModel[];
    pageNumber: number;
    totalPages: number;
    totalCount: number;
    hasPreviousPage!: boolean;
    hasNextPage!: boolean;
}