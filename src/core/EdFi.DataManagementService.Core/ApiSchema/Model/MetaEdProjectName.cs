// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.DataManagementService.Core.External.Model;

namespace EdFi.DataManagementService.Core.ApiSchema.Model;

/// <summary>
/// A string type branded as a MetaEdProjectName, which is the MetaEd project name for a collection of
/// API resources, e.g. "EdFi" for an Ed-Fi data standard version.
/// </summary>
internal record struct MetaEdProjectName(string Value) : IMetaEdProjectName;
