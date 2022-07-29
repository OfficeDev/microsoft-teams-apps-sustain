// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application.Common.Models
{
    public class UserWithPhotoModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Initial { get; set; }
        public string? EmailAddress { get; set; }
        public bool HasPhoto { get; set; }
        public string? Photo { get; set; }
        public int? Status { get; set; }
    }
}
