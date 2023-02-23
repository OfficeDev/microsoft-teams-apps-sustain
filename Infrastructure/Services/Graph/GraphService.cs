// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Graph;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Interfaces;
using Microsoft.Teams.Apps.Sustainability.Application.Common.Models;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure.Services.Graph
{
    public class GraphService : IGraphService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GraphService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<List<UserWithPhotoModel>> GetUsersStartWith(string keyword)
        {
            List<UserWithPhotoModel> users = new List<UserWithPhotoModel>();

            if (!string.IsNullOrEmpty(keyword))
            {
                var queryOptions = new List<Option>()
                {
                    new QueryOption("$search", $"\"displayName:{keyword}\"")
                };

                var usersQuery = await _graphServiceClient.Users.Request(queryOptions).Header("ConsistencyLevel", "eventual").GetAsync();
                foreach (var person in usersQuery)
                {
                    if (!string.IsNullOrEmpty(person.UserPrincipalName))
                    {
                        users.Add(new UserWithPhotoModel
                        {
                            EmailAddress = person.UserPrincipalName,
                            FirstName = person.DisplayName
                        });
                    }
                }
            }

            return users;
        }

        public async Task<UserWithPhotoModel> Get()
        {
            var person = new UserWithPhotoModel();

            var user = await _graphServiceClient.Me.Request().GetAsync();

            if (user != null)
            {
                var photoBase64 = string.Empty;

                try
                {
                    var photoStream = await _graphServiceClient.Me.Photo.Content.Request().GetAsync();
                    var photoByte = ((MemoryStream)photoStream).ToArray();
                    photoBase64 = Convert.ToBase64String(photoByte);
                }
                catch (Exception ex)
                {
                    //catch error - implement error logging
                }

                person.Id = user.Id;
                person.Name = user.DisplayName;
                person.FirstName = user.GivenName;
                person.LastName = user.Surname;
                person.Initial = $"{user.GivenName.Substring(0, 1)}{user.Surname.Substring(0, 1)}";
                person.EmailAddress = user.UserPrincipalName;
                person.Photo = photoBase64;
                person.HasPhoto = string.IsNullOrEmpty(photoBase64) ? false : true;
            }

            return person;
        }

        public async Task<UserWithPhotoModel> GetUser(string userId)
        {
            var person = new UserWithPhotoModel();

            try
            {
                var user = await _graphServiceClient.Users[userId].Request().GetAsync();

                if (user != null)
                {
                    var photoBase64 = string.Empty;

                    photoBase64 = await GetProfilePhoto(user.Id);

                    person.Id = user.Id;
                    person.Name = user.DisplayName;
                    person.FirstName = user.GivenName;
                    person.LastName = user.Surname;
                    person.Initial = $"{user.GivenName.Substring(0, 1)}{user.Surname.Substring(0, 1)}";
                    person.EmailAddress = user.UserPrincipalName;
                    person.Photo = photoBase64;
                    person.HasPhoto = string.IsNullOrEmpty(photoBase64) ? false : true;
                }
            }
            catch (Exception ex)
            {
            }

            return person;
        }

        public async Task<List<UserWithPhotoModel>> GetUserPhotosBulk(List<UserWithPhotoModel> users)
        {
            foreach (var user in users)
            {
                var photoBase64 = string.Empty;
                try
                {
                    photoBase64 = await GetProfilePhoto(user.Id);
                }
                catch (Exception ex)
                {
                    //catch error - implement error logging
                }

                user.Photo = photoBase64;
                user.HasPhoto = string.IsNullOrEmpty(photoBase64) ? false : true;
            }

            return users;
        }

        public async Task<List<UserWithPhotoModel>> GetRelatedPeople(bool includePhoto = true)
        {
            var relatedPeople = new List<UserWithPhotoModel>();

            var user = await _graphServiceClient.Me.People.Request().GetAsync();

            if (user != null && user.Count > 0)
            {
                foreach (var item in user)
                {
                    var person = FromApiResult(item);

                    if (includePhoto)
                    {
                        var photoBase64 = string.Empty;
                        try
                        {
                            photoBase64 = await GetProfilePhoto(item.Id);
                        }
                        catch (Exception ex)
                        {
                            //catch error - implement error logging
                        }

                        person.Photo = photoBase64;
                        person.HasPhoto = string.IsNullOrEmpty(photoBase64) ? false : true;
                    }

                    relatedPeople.Add(person);
                }
            }

            return relatedPeople;
        }

        private async Task<string> GetProfilePhoto(string user)
        {
            var photoBase64 = string.Empty;
            try
            {
                var photoStream = await _graphServiceClient.Users[user].Photo.Content.Request().GetAsync();
                var photoByte = ((MemoryStream)photoStream).ToArray();
                photoBase64 = Convert.ToBase64String(photoByte);
            }
            catch
            {
                //error logging
                //catch error - no photo
            }
            return photoBase64;
        }

        private UserWithPhotoModel FromApiResult(Person item)
        {
            var person = new UserWithPhotoModel()
            {
                Id = item.Id,
                Name = item.DisplayName,
                FirstName = item.GivenName,
                LastName = item.Surname,
                EmailAddress = item.UserPrincipalName,
            };

            if (!string.IsNullOrEmpty(item.GivenName) && !string.IsNullOrEmpty(item.Surname))
            {
                person.Initial = $"{item.GivenName.Substring(0, 1)}{item.Surname.Substring(0, 1)}";
            }

            return person;
        }

        public async Task<List<UserWithPhotoModel>> GetUsers(string userEmail, bool includePhoto = false)
        {
            var users = new List<UserWithPhotoModel>();

            var usersData = await _graphServiceClient.Users.Request().GetAsync();

            if (usersData != null && usersData.Count > 0)
            {
                foreach (var item in usersData)
                {
                    var user = new UserWithPhotoModel
                    {
                        Id = item.Id,
                        Name = item.DisplayName,
                        FirstName = item.GivenName,
                        LastName = item.Surname,
                        EmailAddress = item.UserPrincipalName,
                    };

                    if (!string.IsNullOrEmpty(item.GivenName) && !string.IsNullOrEmpty(item.Surname))
                    {
                        user.Initial = $"{item.GivenName.Substring(0, 1)}{item.Surname.Substring(0, 1)}";
                    }

                    if (includePhoto)
                    {
                        var photoBase64 = string.Empty;
                        try
                        {
                            photoBase64 = await GetProfilePhoto(item.Id);
                        }
                        catch (Exception ex)
                        {
                            //catch error - implement error logging
                        }

                        user.Photo = photoBase64;
                        user.HasPhoto = string.IsNullOrEmpty(photoBase64) ? false : true;
                    }
                    if (user.EmailAddress != userEmail)
                    {
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Sends a notification to a group of users.
        /// </summary>
        /// <param name="recipients">Can be comma separated.</param>
        /// <param name="title">Title of notification (max 50 characters).</param>
        /// <param name="message">Message of notification (max 150 characters).</param>
        /// <param name="appId">App ID of teams app.</param>
        /// <param name="pageId">Page ID of the tab.</param>
        /// <param name="challengeId">Id of selected challenge to share.</param>
        /// <returns></returns>
        public async Task SendActivityFeedNotification(string recipients, string title, string message, string appId, string pageId, int challengeId)
        {

            string context = "{\"challengeId\":\"" + challengeId + "\"}";

            var topic = new TeamworkActivityTopic
            {
                Source = TeamworkActivityTopicSource.Text,
                Value = title,
                WebUrl = $"https://teams.microsoft.com/l/entity/{appId}/{pageId}?label={challengeId}&context=" + context
            };

            var activityType = "userMention";

            var previewText = new ItemBody
            {
                Content = message
            };

            List<string> recipientsArray = recipients.Split(',').ToList();

            foreach (var email in recipientsArray)
            {
                await _graphServiceClient.Users[email].Teamwork
                    .SendActivityNotification(
                        topic, activityType, null, previewText, null
                    )
                    .Request()
                    .PostAsync();
            }
        }

        public async Task AddMemberToYammerGroup(List<string> userIDColl, string? groupId)
        {
            try
            {
                if (userIDColl.Count > 0 && groupId != null && groupId != "")
                {

                    var group = new Group
                    {
                        AdditionalData = new Dictionary<string, object>()
                    {
                        {"members@odata.bind", userIDColl}
                    }
                    };

                    await _graphServiceClient.Groups[groupId].Request().UpdateAsync(group);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
