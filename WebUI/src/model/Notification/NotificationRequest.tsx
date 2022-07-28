// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export class NotificationRequest {
    Recipients!: string;
    Title!: string;
    Message!: string;
    ChallengeId!: number;
    AppId!: string;
    PageId!: string;
}