// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using C2CS.Contexts.ReadCodeC.Data.Model;
using C2CS.Contexts.ReadCodeC.Domain.Explore.Handlers;
using C2CS.Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static bottlenoselabs.clang;

namespace C2CS.Contexts.ReadCodeC.Domain.Explore;

public sealed partial class Explorer
{
    private readonly ILogger<Explorer> _logger;
    private readonly ImmutableDictionary<CKind, ExploreHandler> _handlers;

    private readonly List<CMacroObject> _macroObjects = new();
    private readonly List<CVariable> _variables = new();
    private readonly List<CFunction> _functions = new();
    private readonly List<CRecord> _records = new();
    private readonly List<CEnum> _enums = new();
    private readonly List<CTypeAlias> _typeAliases = new();
    private readonly List<COpaqueType> _opaqueTypes = new();
    private readonly List<CFunctionPointer> _functionPointers = new();
    private readonly List<CPointer> _pointers = new();
    private readonly List<CArray> _arrays = new();
    private readonly List<CPrimitive> _primitives = new();

    private readonly ArrayDeque<ExploreInfoNode> _frontierMacros = new();
    private readonly ArrayDeque<ExploreInfoNode> _frontierVariables = new();
    private readonly ArrayDeque<ExploreInfoNode> _frontierFunctions = new();
    private readonly ArrayDeque<ExploreInfoNode> _frontierTypes = new();

    public Explorer(IServiceProvider services, ILogger<Explorer> logger)
    {
        _logger = logger;
        _handlers = CreateHandlers(services);
    }

    private static ImmutableDictionary<CKind, ExploreHandler> CreateHandlers(IServiceProvider services)
    {
        var result = new Dictionary<CKind, ExploreHandler>
        {
            { CKind.Macro, services.GetService<MacroExploreHandler>()! },
            { CKind.Variable, services.GetService<VariableExploreHandler>()! },
            { CKind.Function, services.GetService<FunctionExploreHandler>()! },
            { CKind.Struct, services.GetService<StructExploreHandler>()! },
            { CKind.Union, services.GetService<UnionExploreHandler>()! },
            { CKind.Enum, services.GetService<EnumExploreHandler>()! },
            { CKind.TypeAlias, services.GetService<TypeAliasExploreHandler>()! },
            { CKind.FunctionPointer, services.GetService<FunctionPointerExploreHandler>()! },
            { CKind.Array, services.GetService<ArrayExploreHandler>()! },
            { CKind.Pointer, services.GetService<PointerExploreHandler>()! },
            { CKind.Primitive, services.GetService<PrimitiveExploreHandler>()! }
        };
        return result.ToImmutableDictionary();
    }

    public CAbstractSyntaxTree AbstractSyntaxTree(
        TargetPlatform targetPlatform,
        ExplorerOptions options,
        ImmutableArray<string> userIncludeDirectories,
        CXTranslationUnit translationUnit,
        ImmutableDictionary<string, string> linkedPaths)
    {
        CAbstractSyntaxTree result;

        var context = new ExploreContext(
            _handlers, targetPlatform, translationUnit, options, TryEnqueueVisitInfoNode, userIncludeDirectories, linkedPaths);

        try
        {
            VisitTranslationUnit(context, translationUnit);
            ExploreMacros(context);
            ExploreVariables(context);
            ExploreFunctions(context);
            ExploreTypes(context);
            result = CollectAbstractSyntaxTree(context);
        }
        catch (Exception e)
        {
            LogFailure(e);
            throw;
        }

        LogSuccess();
        return result;
    }

    private CAbstractSyntaxTree CollectAbstractSyntaxTree(ExploreContext context)
    {
        var macroObjects = _macroObjects.ToImmutableDictionary(x => x.Name);
        var variables = _variables.ToImmutableDictionary(x => x.Name);
        var functions = _functions.ToImmutableDictionary(x => x.Name);
        var records = _records.ToImmutableDictionary(x => x.Name);
        var enums = _enums.ToImmutableDictionary(x => x.Name);
        var typeAliases = _typeAliases.ToImmutableDictionary(x => x.Name);
        var opaqueTypes = _opaqueTypes.ToImmutableDictionary(x => x.Name);
        var functionPointers = _functionPointers.ToImmutableDictionary(x => x.Name);

        var result = new CAbstractSyntaxTree
        {
            FileName = context.FilePath,
            PlatformRequested = context.TargetPlatformRequested,
            PlatformActual = context.TargetPlatformActual,
            MacroObjects = macroObjects,
            Variables = variables,
            Functions = functions,
            Records = records,
            Enums = enums,
            TypeAliases = typeAliases,
            OpaqueTypes = opaqueTypes,
            FunctionPointers = functionPointers
        };

        return result;
    }

    private void ExploreMacros(ExploreContext context)
    {
        var totalCount = _frontierMacros.Count;
        var macroNamesToExplore = string.Join(", ", _frontierMacros.Select(x => x.Name));
        LogExploringMacros(totalCount, macroNamesToExplore);
        var macroNamesFound = _macroObjects.Select(x => x.Name).ToArray();
        var macroNamesFoundString = string.Join(", ", _macroObjects.Select(x => x.Name));
        LogFoundMacros(macroNamesFound.Length, macroNamesFoundString);
    }

    private void ExploreVariables(ExploreContext context)
    {
        var totalCount = _frontierVariables.Count;
        var variableNamesToExplore = string.Join(", ", _frontierVariables.Select(x => x.Name));
        LogExploringVariables(totalCount, variableNamesToExplore);
        var variableNamesFound = _variables.Select(x => x.Name).ToArray();
        var variableNamesFoundString = string.Join(", ", variableNamesFound);
        LogFoundVariables(variableNamesFound.Length, variableNamesFoundString);
    }

    private void ExploreFunctions(ExploreContext context)
    {
        var totalCount = _frontierFunctions.Count;
        var functionNamesToExplore = string.Join(", ", _frontierFunctions.Select(x => x.Name));
        LogExploringFunctions(totalCount, functionNamesToExplore);
        ExploreFrontier(context, _frontierFunctions);
        var functionNamesFound = _functions.Select(x => x.Name).ToArray();
        var functionNamesFoundString = string.Join(", ", functionNamesFound);
        LogFoundFunctions(functionNamesFound.Length, functionNamesFoundString);
    }

    private void ExploreTypes(ExploreContext context)
    {
        var totalCount = _frontierTypes.Count;
        var typeNamesToExplore = string.Join(", ", _frontierTypes.Select(x => x.Name));
        LogExploringTypes(totalCount, typeNamesToExplore);
        ExploreFrontier(context, _frontierTypes);

        var typeNamesFound = new List<string>();
        typeNamesFound.AddRange(_records.Select(x => x.Name));
        typeNamesFound.AddRange(_enums.Select(x => x.Name));
        typeNamesFound.AddRange(_typeAliases.Select(x => x.Name));
        typeNamesFound.AddRange(_opaqueTypes.Select(x => x.Name));
        typeNamesFound.AddRange(_functionPointers.Select(x => x.Name));
        typeNamesFound.AddRange(_pointers.Select(x => x.Name));
        typeNamesFound.AddRange(_arrays.Select(x => x.Name));
        typeNamesFound.AddRange(_primitives.Select(x => x.Name));
        var typeNamesFoundJoined = string.Join(", ", typeNamesFound);

        LogFoundTypes(typeNamesFound.Count, typeNamesFoundJoined);
    }

    private void ExploreFrontier(
        ExploreContext context, ArrayDeque<ExploreInfoNode> frontier)
    {
        while (frontier.Count > 0)
        {
            var node = frontier.PopFront()!;
            ExploreNode(context, node);
        }
    }

    private bool ExploreNode(ExploreContext context, ExploreInfoNode visitInfo)
    {
        var node = context.Explore(visitInfo);
        FoundNode(node);
        return true;
    }

    private void FoundNode(CNode node)
    {
        var location = node is CNodeWithLocation nodeWithLocation ? nodeWithLocation.Location : CLocation.NoLocation;
        LogFoundNode(node.Kind, node.Name, location);

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (node.Kind)
        {
            case CKind.Macro:
                FoundMacro((CMacroObject)node);
                break;
            case CKind.Variable:
                FoundVariable((CVariable)node);
                break;
            case CKind.Function:
                FoundFunction((CFunction)node);
                break;
            case CKind.Struct:
            case CKind.Union:
                FoundRecord((CRecord)node);
                break;
            case CKind.Enum:
                FoundEnum((CEnum)node);
                break;
            case CKind.TypeAlias:
                FoundTypeAlias((CTypeAlias)node);
                break;
            case CKind.OpaqueType:
                FoundOpaqueType((COpaqueType)node);
                break;
            case CKind.FunctionPointer:
                FoundFunctionPointer((CFunctionPointer)node);
                break;
            case CKind.Pointer:
                FoundPointer((CPointer)node);
                break;
            case CKind.Array:
                FoundArray((CArray)node);
                break;
            case CKind.Primitive:
                FoundPrimitive((CPrimitive)node);
                break;
            default:
                var up = new NotImplementedException($"Found a node of kind '{node.Kind}' but do not know how to add it.");
                throw up;
        }
    }

    private void FoundMacro(CMacroObject node)
    {
        _macroObjects.Add(node);
    }

    private void FoundVariable(CVariable node)
    {
        _variables.Add(node);
    }

    private void FoundFunction(CFunction node)
    {
        _functions.Add(node);
    }

    private void FoundRecord(CRecord node)
    {
        _records.Add(node);
    }

    private void FoundEnum(CEnum node)
    {
        _enums.Add(node);
    }

    private void FoundTypeAlias(CTypeAlias node)
    {
        _typeAliases.Add(node);
    }

    private void FoundOpaqueType(COpaqueType node)
    {
        _opaqueTypes.Add(node);
    }

    private void FoundFunctionPointer(CFunctionPointer node)
    {
        _functionPointers.Add(node);
    }

    private void FoundPointer(CPointer node)
    {
        _pointers.Add(node);
    }

    private void FoundArray(CArray node)
    {
        _arrays.Add(node);
    }

    private void FoundPrimitive(CPrimitive node)
    {
        _primitives.Add(node);
    }

    private void VisitTranslationUnit(ExploreContext context, CXTranslationUnit translationUnit)
    {
        var translationUnitCursor = clang_getTranslationUnitCursor(translationUnit);
        var cursors = translationUnitCursor.GetDescendents(
            (child, _) => IsTopLevelCursorOfInterest(child, context.Options));

        foreach (var cursor in cursors)
        {
            VisitTopLevelCursor(context, cursor);
        }
    }

    private static bool IsTopLevelCursorOfInterest(CXCursor cursor, ExplorerOptions options)
    {
        var kind = clang_getCursorKind(cursor);
        if (kind != CXCursorKind.CXCursor_FunctionDecl &&
            kind != CXCursorKind.CXCursor_VarDecl &&
            kind != CXCursorKind.CXCursor_EnumDecl &&
            kind != CXCursorKind.CXCursor_MacroDefinition)
        {
            return false;
        }

        if (kind == CXCursorKind.CXCursor_MacroDefinition)
        {
            var isMacroBuiltIn = clang_Cursor_isMacroBuiltin(cursor) > 0;
            if (isMacroBuiltIn)
            {
                return false;
            }
        }
        else
        {
            var linkage = clang_getCursorLinkage(cursor);
            var isExternallyLinked = linkage == CXLinkageKind.CXLinkage_External;
            if (!isExternallyLinked)
            {
                return false;
            }
        }

        return true;
    }

    private void VisitTopLevelCursor(ExploreContext context, CXCursor cursor)
    {
        var kind = cursor.kind switch
        {
            CXCursorKind.CXCursor_FunctionDecl => CKind.Function,
            CXCursorKind.CXCursor_VarDecl => CKind.Variable,
            CXCursorKind.CXCursor_EnumDecl => CKind.Enum,
            CXCursorKind.CXCursor_MacroDefinition => CKind.Macro,
            _ => CKind.Unknown
        };

        if (kind == CKind.Unknown)
        {
            LogUnexpectedTopLevelCursor(cursor.kind);
            return;
        }

        if (kind == CKind.Macro)
        {
            // Function-like macros currently not implemented
            // https://github.com/lithiumtoast/c2cs/issues/35
            if (clang_Cursor_isMacroFunctionLike(cursor) > 0)
            {
                return;
            }
        }

        var type = clang_getCursorType(cursor);

        if (type.kind == CXTypeKind.CXType_Unexposed)
        {
            // CXType_Unexposed: A type whose specific kind is not exposed via this interface (libclang).
            // When this happens, use the "canonical form" or the standard/normal form of the type
            type = clang_getCanonicalType(type);
        }

        if (type.kind == CXTypeKind.CXType_Attributed)
        {
            // CXTypeKind.CXType_Attributed: The type has a Clang attribute.
            type = clang_Type_getModifiedType(type);
        }

        if (kind == CKind.Enum && !context.Options.IsEnabledEnumsDangling)
        {
            return;
        }

        var spelling = clang_getCursorSpelling(cursor);
        string name = clang_getCString(spelling);

        var visitInfo = context.CreateVisitInfoNode(kind, name, cursor, type, null);
        TryEnqueueVisitInfoNode(context, kind, visitInfo);
    }

    private void TryEnqueueVisitInfoNode(ExploreContext context, CKind kind, ExploreInfoNode info)
    {
        var frontier = kind switch
        {
            CKind.Macro => _frontierMacros,
            CKind.Variable => _frontierVariables,
            CKind.Function => _frontierFunctions,
            _ => _frontierTypes,
        };

        switch (kind)
        {
            case CKind.Macro when !context.Options.IsEnabledMacroObjects:
            case CKind.Variable when !context.Options.IsEnabledVariables:
            case CKind.Function when !context.Options.IsEnabledFunctions:
                return;
        }

        if (!context.CanVisit(kind, info))
        {
            return;
        }

        LogEnqueue(kind, info.Name, info.Location);
        frontier.PushBack(info);
    }

    private string GetFunctionPointerName(CXCursor cursor, CXType type)
    {
        return cursor.kind switch
        {
            CXCursorKind.CXCursor_TypedefDecl => cursor.Name(),
            _ => type.Name()
        };
    }

    [LoggerMessage(0, LogLevel.Error, "- Expected a top level translation unit declaration (function, variable, enum, or macro) but found '{Kind}'")]
    public partial void LogUnexpectedTopLevelCursor(CXCursorKind kind);

    [LoggerMessage(1, LogLevel.Error, "- Failure")]
    public partial void LogFailure(Exception exception);

    [LoggerMessage(2, LogLevel.Debug, "- Success")]
    public partial void LogSuccess();

    [LoggerMessage(3, LogLevel.Information, "- Exploring {Count} macros: {Names}")]
    public partial void LogExploringMacros(int count, string names);

    [LoggerMessage(4, LogLevel.Information, "- Found {FoundCount} macros: {Names}")]
    public partial void LogFoundMacros(int foundCount, string names);

    [LoggerMessage(5, LogLevel.Information, "- Exploring {Count} variables: {Names}")]
    public partial void LogExploringVariables(int count, string names);

    [LoggerMessage(6, LogLevel.Information, "- Found {FoundCount} variables: {Names}")]
    public partial void LogFoundVariables(int foundCount, string names);

    [LoggerMessage(7, LogLevel.Information, "- Exploring {Count} functions: {Names}")]
    public partial void LogExploringFunctions(int count, string names);

    [LoggerMessage(8, LogLevel.Information, "- Found {FoundCount} functions: {Names}")]
    public partial void LogFoundFunctions(int foundCount, string names);

    [LoggerMessage(9, LogLevel.Information, "- Exploring {Count} types: {Names}")]
    public partial void LogExploringTypes(int count, string names);

    [LoggerMessage(10, LogLevel.Information, "- Found {FoundCount} types: {Names}")]
    public partial void LogFoundTypes(int foundCount, string names);

    [LoggerMessage(11, LogLevel.Debug, "- Enqueued {Kind} '{Name}' ({Location})")]
    public partial void LogEnqueue(CKind kind, string name, CLocation location);

    [LoggerMessage(12, LogLevel.Information, "- Found {Kind} '{Name}' ({Location})")]
    public partial void LogFoundNode(CKind kind, string name, CLocation location);
}
