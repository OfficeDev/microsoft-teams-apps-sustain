// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using AutoMapper;

namespace Microsoft.Teams.Apps.Sustainability.Application;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
