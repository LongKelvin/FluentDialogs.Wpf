namespace FluentDialogs;

/// <summary>
/// Contains string constants for all v2 theme token resource keys.
/// Use these when accessing/overriding tokens from C# code to avoid magic strings.
/// </summary>
public static class ThemeTokenKeys
{
    // ═══════════════ Surface ═══════════════
    public const string SurfacePrimary = "FDSemSurfacePrimary";
    public const string SurfaceSecondary = "FDSemSurfaceSecondary";
    public const string SurfaceOverlay = "FDSemSurfaceOverlay";

    // ═══════════════ On Surface ═══════════════
    public const string OnSurfacePrimary = "FDSemOnSurfacePrimary";
    public const string OnSurfaceSecondary = "FDSemOnSurfaceSecondary";

    // ═══════════════ Interactive ═══════════════
    public const string InteractiveDefault = "FDSemInteractiveDefault";
    public const string InteractiveHover = "FDSemInteractiveHover";
    public const string InteractivePressed = "FDSemInteractivePressed";
    public const string OnInteractive = "FDSemOnInteractive";

    // ═══════════════ Neutral ═══════════════
    public const string NeutralDefault = "FDSemNeutralDefault";
    public const string NeutralHover = "FDSemNeutralHover";
    public const string NeutralPressed = "FDSemNeutralPressed";

    // ═══════════════ Status: Error ═══════════════
    public const string StatusError = "FDSemStatusError";
    public const string StatusErrorSubtle = "FDSemStatusErrorSubtle";
    public const string OnStatusError = "FDSemOnStatusError";
    public const string StatusErrorHover = "FDSemStatusErrorHover";
    public const string StatusErrorPressed = "FDSemStatusErrorPressed";

    // ═══════════════ Status: Warning ═══════════════
    public const string StatusWarning = "FDSemStatusWarning";
    public const string StatusWarningSubtle = "FDSemStatusWarningSubtle";

    // ═══════════════ Status: Success ═══════════════
    public const string StatusSuccess = "FDSemStatusSuccess";
    public const string StatusSuccessSubtle = "FDSemStatusSuccessSubtle";

    // ═══════════════ Status: Info ═══════════════
    public const string StatusInfo = "FDSemStatusInfo";

    // ═══════════════ Border ═══════════════
    public const string BorderDefault = "FDSemBorderDefault";
    public const string BorderStrong = "FDSemBorderStrong";

    // ═══════════════ Shadow ═══════════════
    public const string Shadow = "FDSemShadow";

    // ═══════════════ Link ═══════════════
    public const string LinkDefault = "FDSemLinkDefault";
    public const string LinkHover = "FDSemLinkHover";

    // ═══════════════ Close Button ═══════════════
    public const string CloseHover = "FDSemCloseHover";
    public const string ClosePressed = "FDSemClosePressed";
    public const string OnClose = "FDSemOnClose";

    // ═══════════════ Brush Keys (public API) ═══════════════
    public static class Brushes
    {
        public const string SurfacePrimary = "FDBrushSurfacePrimary";
        public const string SurfaceSecondary = "FDBrushSurfaceSecondary";
        public const string SurfaceOverlay = "FDBrushSurfaceOverlay";

        public const string OnSurfacePrimary = "FDBrushOnSurfacePrimary";
        public const string OnSurfaceSecondary = "FDBrushOnSurfaceSecondary";

        public const string InteractiveDefault = "FDBrushInteractiveDefault";
        public const string InteractiveHover = "FDBrushInteractiveHover";
        public const string InteractivePressed = "FDBrushInteractivePressed";
        public const string OnInteractive = "FDBrushOnInteractive";

        public const string NeutralDefault = "FDBrushNeutralDefault";
        public const string NeutralHover = "FDBrushNeutralHover";
        public const string NeutralPressed = "FDBrushNeutralPressed";

        public const string StatusError = "FDBrushStatusError";
        public const string StatusErrorSubtle = "FDBrushStatusErrorSubtle";
        public const string OnStatusError = "FDBrushOnStatusError";
        public const string StatusErrorHover = "FDBrushStatusErrorHover";
        public const string StatusErrorPressed = "FDBrushStatusErrorPressed";
        public const string StatusWarning = "FDBrushStatusWarning";
        public const string StatusWarningSubtle = "FDBrushStatusWarningSubtle";
        public const string StatusSuccess = "FDBrushStatusSuccess";
        public const string StatusSuccessSubtle = "FDBrushStatusSuccessSubtle";
        public const string StatusInfo = "FDBrushStatusInfo";

        public const string BorderDefault = "FDBrushBorderDefault";
        public const string BorderStrong = "FDBrushBorderStrong";
        public const string Shadow = "FDBrushShadow";
        public const string LinkDefault = "FDBrushLinkDefault";
        public const string LinkHover = "FDBrushLinkHover";
        public const string CloseHover = "FDBrushCloseHover";
        public const string ClosePressed = "FDBrushClosePressed";
        public const string OnClose = "FDBrushOnClose";
    }

    /// <summary>
    /// All required semantic token keys that a valid preset must define.
    /// Used by <see cref="ThemeValidator"/> in DEBUG builds.
    /// </summary>
    public static readonly string[] RequiredSemanticKeys =
    [
        SurfacePrimary, SurfaceSecondary, SurfaceOverlay,
        OnSurfacePrimary, OnSurfaceSecondary,
        InteractiveDefault, InteractiveHover, InteractivePressed, OnInteractive,
        NeutralDefault, NeutralHover, NeutralPressed,
        StatusError, StatusErrorSubtle, OnStatusError, StatusErrorHover, StatusErrorPressed,
        StatusWarning, StatusWarningSubtle,
        StatusSuccess, StatusSuccessSubtle,
        StatusInfo,
        BorderDefault, BorderStrong,
        Shadow,
        LinkDefault, LinkHover,
        CloseHover, ClosePressed, OnClose
    ];
}
