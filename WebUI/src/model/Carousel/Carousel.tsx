// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export class CarouselModel {
    id!: number;
    link?: string;
    title?: string;
    thumbnail?: string;
    description?: string;
    isActive!: boolean;
    file: File;
    filename: string;
    linkValid: boolean = true;
    fileValid: boolean = true;
}