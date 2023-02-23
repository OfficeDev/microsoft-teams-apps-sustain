// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces
{
    public interface IGraphService
    {
        Task<UserWithPhotoModel> Get();
        Task<List<UserWithPhotoModel>> GetUsersStartWith(string keyword);
        Task<List<UserWithPhotoModel>> GetUserPhotosBulk(List<UserWithPhotoModel> users);
        Task<List<UserWithPhotoModel>> GetRelatedPeople(bool includePhoto = true);
        Task<UserWithPhotoModel> GetUser(string userId);
        Task<List<UserWithPhotoModel>> GetUsers(string userEmail, bool includePhoto = false);
        Task SendActivityFeedNotification(string recipients, string title, string message, string appId, string pageId, int challengeId);
        Task AddMemberToYammerGroup(List<string> Emails, string? groupId);
    }
}
