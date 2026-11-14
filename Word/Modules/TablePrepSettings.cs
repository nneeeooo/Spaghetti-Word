using Microsoft.Office.Interop.Word;
using Word.Helpers;

namespace Word.Modules
{
    /// <summary>
    /// Table options of the Word Prep form. Single place that maps them to <see cref="Tables.Prepare"/>
    /// and to the registry (HKCU\Software\Macaroni\TableConverter), so the form and the context menu agree.
    /// </summary>
    internal sealed class TablePrepSettings
    {
        internal const string RegistryPath = @"Software\Macaroni\TableConverter";

        public bool AutofitWindow, AutofitRowHeight, DoNotResizeToFitContents, RemoveSpacingBeforeAfter,
            DoNotBreakAcrossPages, ResetPaginationSettings;
        public bool ZeroMargins, DefaultMargins, KeepMargins, CustomMargins, RemoveBorders;
        public float MarginLeft, MarginRight, MarginTop, MarginBottom;

        internal static TablePrepSettings Load()
        {
            const string p = RegistryPath;
            return new TablePrepSettings
            {
                AutofitRowHeight = RegistryHelper.LoadValue(p, "DoAutofitRowHeight", true),
                AutofitWindow = RegistryHelper.LoadValue(p, "DoAutofitWindow", true),
                DoNotResizeToFitContents = RegistryHelper.LoadValue(p, "DoDoNotResizeToFitContents", true),
                RemoveSpacingBeforeAfter = RegistryHelper.LoadValue(p, "DoRemoveSpacingBeforeAfter", true),
                DoNotBreakAcrossPages = RegistryHelper.LoadValue(p, "DoDoNotBreakAcrossPages", true),
                ZeroMargins = RegistryHelper.LoadValue(p, "DoSetZeroMargins", false),
                DefaultMargins = RegistryHelper.LoadValue(p, "DoSetDefaultMargins", false),
                ResetPaginationSettings = RegistryHelper.LoadValue(p, "DoResetPaginationSettings", true),
                RemoveBorders = RegistryHelper.LoadValue(p, "DoRemoveBorders", false),
                KeepMargins = RegistryHelper.LoadValue(p, "DoKeepMargins", true),
                CustomMargins = RegistryHelper.LoadValue(p, "DoSetCustomMargins", false),
                MarginLeft = RegistryHelper.LoadValue(p, "MarginLeft", 0f),
                MarginRight = RegistryHelper.LoadValue(p, "MarginRight", 0f),
                MarginTop = RegistryHelper.LoadValue(p, "MarginTop", 0f),
                MarginBottom = RegistryHelper.LoadValue(p, "MarginBottom", 0f),
            };
        }

        internal void Save()
        {
            const string p = RegistryPath;
            RegistryHelper.SaveValue(p, "DoAutofitWindow", AutofitWindow);
            RegistryHelper.SaveValue(p, "DoAutofitRowHeight", AutofitRowHeight);
            RegistryHelper.SaveValue(p, "DoDoNotResizeToFitContents", DoNotResizeToFitContents);
            RegistryHelper.SaveValue(p, "DoSetZeroMargins", ZeroMargins);
            RegistryHelper.SaveValue(p, "DoSetDefaultMargins", DefaultMargins);
            RegistryHelper.SaveValue(p, "DoResetPaginationSettings", ResetPaginationSettings);
            RegistryHelper.SaveValue(p, "DoRemoveBorders", RemoveBorders);
            RegistryHelper.SaveValue(p, "DoKeepMargins", KeepMargins);
            RegistryHelper.SaveValue(p, "DoDoNotBreakAcrossPages", DoNotBreakAcrossPages);
            RegistryHelper.SaveValue(p, "DoRemoveSpacingBeforeAfter", RemoveSpacingBeforeAfter);
            RegistryHelper.SaveValue(p, "DoSetCustomMargins", CustomMargins);
            RegistryHelper.SaveValue(p, "MarginLeft", MarginLeft);
            RegistryHelper.SaveValue(p, "MarginRight", MarginRight);
            RegistryHelper.SaveValue(p, "MarginTop", MarginTop);
            RegistryHelper.SaveValue(p, "MarginBottom", MarginBottom);
        }

        /// <summary>Runs Tables.Prepare. <paramref name="onlyTable"/> limits it to that one table.</summary>
        internal void Apply(bool convertSelectionOnly = false, Table onlyTable = null, string undoName = null)
        {
            Tables.Prepare(
                doAutofitWindow: AutofitWindow,
                doAutofitRowHeight: AutofitRowHeight,
                doDoNotResizeToFitContents: DoNotResizeToFitContents,
                doRemoveSpacingBeforeAfter: RemoveSpacingBeforeAfter,
                doDoNotBreakAcrossPages: DoNotBreakAcrossPages,
                doSetZeroMargins: ZeroMargins,
                doSetDefaultMargins: DefaultMargins,
                doSetCustomMargins: CustomMargins,
                doResetPaginationSettings: ResetPaginationSettings,
                doRemoveBorders: RemoveBorders,
                convertSelectionOnly: convertSelectionOnly,
                customMarginTop: MarginTop,
                customMarginBottom: MarginBottom,
                customMarginLeft: MarginLeft,
                customMarginRight: MarginRight,
                onlyTable: onlyTable,
                undoName: undoName);
        }
    }
}
