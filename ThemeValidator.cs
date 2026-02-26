using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace FluentDialogs;

/// <summary>
/// DEBUG-only validator that checks whether all required semantic tokens
/// are present in the active resource dictionary tree.
/// In Release builds, all methods are no-ops.
/// </summary>
public static class ThemeValidator
{
    /// <summary>
    /// Validates that all required semantic token keys exist in the given dictionary.
    /// Only runs when compiled with DEBUG; no-op in Release.
    /// </summary>
    [Conditional("DEBUG")]
    public static void ValidateIfDebug(ResourceDictionary resources)
    {
        var missing = new List<string>();

        foreach (var key in ThemeTokenKeys.RequiredSemanticKeys)
        {
            if (!ContainsKey(resources, key))
            {
                missing.Add(key);
            }
        }

        if (missing.Count > 0)
        {
            var message = $"[FluentDialogs] Theme validation: {missing.Count} required semantic token(s) missing:\n" +
                          string.Join("\n  • ", missing);

            Debug.WriteLine(message);

            // Also trace to Output window in VS
            Trace.TraceWarning(message);
        }
        else
        {
            Debug.WriteLine("[FluentDialogs] Theme validation passed: all required semantic tokens present.");
        }
    }

    private static bool ContainsKey(ResourceDictionary dict, string key)
    {
        if (dict.Contains(key))
            return true;

        foreach (var merged in dict.MergedDictionaries)
        {
            if (ContainsKey(merged, key))
                return true;
        }

        return false;
    }
}
