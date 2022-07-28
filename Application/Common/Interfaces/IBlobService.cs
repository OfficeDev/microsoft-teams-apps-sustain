// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application;

public interface IBlobService
{
    string GetSasLink(string link);
    Task<string> UploadFile(Stream stream, string fileName);
    Task RemoveFile(string absoluteUri);
}
