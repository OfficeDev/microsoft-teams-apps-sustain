// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Teams.Apps.Sustainability.Application;

public class ChallengePinningException : Exception
{
    public ChallengePinningException()
        : base()
    {
    }

    public ChallengePinningException(string message)
        : base(message)
    {
    }

    public ChallengePinningException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ChallengePinningException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

