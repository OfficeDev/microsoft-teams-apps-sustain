// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { UserRole } from "./UserRole";

export class User {
    id?: number;
    username?: string;
    email?: string;
    created?: string;
    createdBy?: string;
    lastModified?: string;
    lastModifiedBy?: string;
    userRoles?: UserRole[];
}