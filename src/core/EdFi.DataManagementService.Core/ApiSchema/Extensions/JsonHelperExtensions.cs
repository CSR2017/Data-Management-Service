// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using Json.Path;

namespace EdFi.DataManagementService.Core.ApiSchema.Extensions;

internal static class JsonHelperExtensions
{
    /// <summary>
    /// Helper to go from a scalar JSONPath selection directly to the selected JsonNode,
    /// or null if the node does not exist
    /// </summary>
    public static JsonNode? SelectNodeFromPath(this JsonNode jsonNode, string jsonPathString, ILogger logger)
    {
        try
        {
            JsonPath? jsonPath = JsonPath.Parse(jsonPathString);
            if (jsonPath == null)
            {
                logger.LogError("Malformed JSONPath string '{JsonPathString}'", jsonPathString);
                throw new InvalidOperationException($"Malformed JSONPath string '{jsonPathString}'");
            }

            PathResult? result = jsonPath.Evaluate(jsonNode);

            if (result.Matches == null)
            {
                logger.LogError("Malformed JSONPath string '{JsonPathString}'", jsonPathString);
                throw new InvalidOperationException($"Unexpected Json.Path error for '{jsonPathString}'");
            }

            if (result.Matches.Count == 0)
                return null;

            if (result.Matches.Count != 1)
            {
                logger.LogError(
                    "JSONPath string '{JsonPathString}' selected multiple values",
                    jsonPathString
                );
                throw new InvalidOperationException(
                    $"JSONPath string '{jsonPathString}' selected multiple values"
                );
            }

            return result.Matches[0].Value;
        }
        catch (PathParseException)
        {
            throw new InvalidOperationException($"Unexpected Json.Path error for '{jsonPathString}'");
        }
    }

    /// <summary>
    /// Helper to go from an array JSONPath selection directly to the selected JsonNodes.
    /// Returns an empty array if none are selected.
    /// </summary>
    public static IEnumerable<JsonNode> SelectNodesFromArrayPath(
        this JsonNode jsonNode,
        string jsonPathString,
        ILogger logger
    )
    {
        try
        {
            JsonPath? jsonPath = JsonPath.Parse(jsonPathString);
            if (jsonPath == null)
            {
                logger.LogError("Malformed JSONPath string '{JsonPathString}'", jsonPathString);
                throw new InvalidOperationException($"Malformed JSONPath string '{jsonPathString}'");
            }

            PathResult? result = jsonPath.Evaluate(jsonNode);

            if (result.Matches == null)
            {
                logger.LogError("Malformed JSONPath string '{JsonPathString}'", jsonPathString);
                throw new InvalidOperationException($"Unexpected Json.Path error for '{jsonPathString}'");
            }

            return result.Matches.Select(x =>
                x.Value
                ?? throw new InvalidOperationException(
                    $"Unexpected Json.Path error for '{jsonPathString}': returned null for JsonNode"
                )
            );
        }
        catch (PathParseException)
        {
            throw new InvalidOperationException($"Unexpected Json.Path error for '{jsonPathString}'");
        }
    }

    /// <summary>
    /// Helper to go from an array JSONPath selection directly to a string array regardless of the JSON type
    /// Returns empty array if the values do not exist.
    /// </summary>
    public static IEnumerable<string> SelectNodesFromArrayPathCoerceToStrings(
        this JsonNode jsonNode,
        string jsonPathString,
        ILogger logger
    )
    {
        IEnumerable<JsonNode> jsonNodes = SelectNodesFromArrayPath(jsonNode, jsonPathString, logger);
        return jsonNodes.Select(jsonNode =>
        {
            JsonValue result =
                jsonNode.AsValue() ?? throw new InvalidOperationException("Unexpected JSONPath value error");
            return result.ToString();
        });
    }

    /// <summary>
    /// Helper to go from a scalar JSONPath selection directly to the selected JsonNode.
    /// Throws if the value does not exist
    /// </summary>
    public static JsonNode SelectRequiredNodeFromPath(
        this JsonNode jsonNode,
        string jsonPathString,
        ILogger logger
    )
    {
        JsonNode? result =
            SelectNodeFromPath(jsonNode, jsonPathString, logger)
            ?? throw new InvalidOperationException($"Node at path '{jsonPathString}' not found");
        return result;
    }

    /// <summary>
    /// Helper to go from a scalar JSONPath selection directly to the typed value,
    /// or null if the node does not exist
    /// </summary>
    public static T? SelectNodeFromPathAs<T>(this JsonNode jsonNode, string jsonPathString, ILogger logger)
    {
        JsonNode? selectedNode = SelectNodeFromPath(jsonNode, jsonPathString, logger);

        if (selectedNode == null)
            return default;

        JsonValue? resultNode =
            selectedNode.AsValue() ?? throw new InvalidOperationException("Unexpected JSONPath value error");
        return resultNode.GetValue<T>();
    }

    /// <summary>
    /// Helper to go from a scalar JSONPath selection directly to a string value regardless of the JSON type
    /// Throws if the value does not exist
    /// </summary>
    public static string SelectRequiredNodeFromPathCoerceToString(
        this JsonNode jsonNode,
        string jsonPathString,
        ILogger logger
    )
    {
        JsonNode selectedNode =
            SelectNodeFromPath(jsonNode, jsonPathString, logger)
            ?? throw new InvalidOperationException("Unexpected JSONPath value error");

        JsonValue resultNode =
            selectedNode.AsValue() ?? throw new InvalidOperationException("Unexpected JSONPath value error");
        return resultNode.ToString();
    }

    /// <summary>
    /// Helper to go from a scalar JSONPath selection directly to the typed value.
    /// Throws if the value does not exist
    /// </summary>
    public static T SelectRequiredNodeFromPathAs<T>(
        this JsonNode jsonNode,
        string jsonPathString,
        ILogger logger
    )
    {
        T? result =
            SelectNodeFromPathAs<T>(jsonNode, jsonPathString, logger)
            ?? throw new InvalidOperationException($"Node at path '{jsonPathString}' not found");
        return result;
    }

    /// <summary>
    /// Helper to get value from json node. Throws if the node does not exist.
    /// </summary>
    public static T SelectNodeValue<T>(this JsonNode jsonNode, string jsonPathString)
    {
        var resourceName = jsonNode["resourceName"];
        var errorMessage =
            $"Expected {jsonPathString} to be in ResourceSchema for {resourceName}, invalid ApiSchema";

        JsonValue? resultNode =
            jsonNode[jsonPathString]?.AsValue() ?? throw new InvalidOperationException(errorMessage);
        return resultNode.GetValue<T>();
    }
}
