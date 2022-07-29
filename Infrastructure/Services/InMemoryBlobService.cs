// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Teams.Apps.Sustainability.Application;

namespace Microsoft.Teams.Apps.Sustainability.Infrastructure;

public class InMemoryBlobService: IBlobService
{
    public string GetSasLink(string link)
    {
        return link;
    }

    public Task RemoveFile(string absoluteUri)
    {
        return Task.FromResult("");
    }

    public Task<string> UploadFile(Stream stream, string fileName)
    {
        return Task.FromResult("");
    }
}
