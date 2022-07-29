// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application;

internal class UserDuplicateException: Exception
{
    public UserDuplicateException()
       : base()
    {
    }

    public UserDuplicateException(string message)
        : base(message)
    {
    }

    public UserDuplicateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UserDuplicateException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
