using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using Shape = Microsoft.Office.Interop.Word.Shape;

namespace Word.Modules
{
    /// <summary>
    /// Shared utility methods for Word documents.
    /// </summary>
    internal static class Shared
    {
        /// <summary>
        /// Closes the split view in the active Word document and ensures it is in Print View mode.
        /// </summary>
        internal static void EnsurePrintViewAndCloseSplit(Application app)
        {
            var activeWindow = app.ActiveWindow;

            try
            {
                if (activeWindow.Panes.Count == 2)
                {
                    activeWindow.Panes[1].Activate();

                    if (activeWindow.Split)
                    {
                        activeWindow.Split = false;
                    }
                }

                if (activeWindow.ActivePane.View.Type != WdViewType.wdPrintView)
                {
                    activeWindow.ActivePane.View.Type = WdViewType.wdPrintView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during view setup: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Applies the specified action to the entire document, including its content, headers, footers, footnotes, lists, and endnotes.
        /// </summary>
        internal static void ApplyGlobally(Document doc, Action<Range> action)
        {
            action(doc.Content);
            // It handles both inline and floating textboxes but not images and other shapes
            // Word is broken i guess
            TraverseShapes(doc, new HashSet<MsoShapeType> { MsoShapeType.msoTextBox }, inner =>
            {
                if (inner.TextFrame.HasText != 0) action(inner.TextFrame.TextRange);
            });

            // Main body, headers, footers
            foreach (Section section in doc.Sections)
            {
                foreach (var headerIndex in new[]
                {
                    WdHeaderFooterIndex.wdHeaderFooterPrimary,
                    WdHeaderFooterIndex.wdHeaderFooterFirstPage,
                    WdHeaderFooterIndex.wdHeaderFooterEvenPages
                })
                {
                    var header = section.Headers[headerIndex];
                    if (header != null && header.Exists)
                    {
                        action(header.Range);

                        TraverseShapes(header, new HashSet<MsoShapeType> { MsoShapeType.msoTextBox }, inner =>
                        {
                            if (inner.TextFrame.HasText != 0) action(inner.TextFrame.TextRange);
                        });
                    }

                    var footer = section.Footers[headerIndex];
                    if (footer != null && footer.Exists)
                    {
                        action(footer.Range);

                        TraverseShapes(footer, new HashSet<MsoShapeType> { MsoShapeType.msoTextBox }, inner =>
                        {
                            if (inner.TextFrame.HasText != 0) action(inner.TextFrame.TextRange);
                        });
                    }
                }
            }

            // Footnotes
            foreach (Footnote footnote in doc.Footnotes)
            {
                action(footnote.Range);
            }

            // Endnotes
            foreach (Endnote endnote in doc.Endnotes)
            {
                action(endnote.Range);
            }

            // Lists
            // TODO: Figure out why it doesn't work sometimes
            foreach (List list in doc.Lists)
            {
                var range = list.Range;

                action(range);
            }
        }

        /// <summary>
        /// Traverses all shapes in the document and applies the specified action to each shape of the specified types.
        /// </summary>
        internal static void TraverseShapes(
            Document doc,
            HashSet<MsoShapeType> types,
            Action<Shape> action)
        {
            if (doc == null) return;
            TraverseShapes(doc.Content, types, action);
        }

        /// <summary>
        /// Traverses all shapes in the header/footer and applies the specified action to each shape of the specified types.
        /// </summary>
        internal static void TraverseShapes(
            HeaderFooter hf,
            HashSet<MsoShapeType> types,
            Action<Shape> action)
        {
            if (hf == null) return;
            TraverseShapes(hf.Range, types, action);
        }

        /// <summary>
        /// Traverses all shapes in the specified range and applies the specified action to each shape of the specified types.
        /// </summary>
        internal static void TraverseShapes(
            Range range,
            HashSet<MsoShapeType> types,
            Action<Shape> action)
        {
            if (range?.ShapeRange == null || range.ShapeRange.Count == 0)
                return;

            foreach (Shape shape in range.ShapeRange)
                TraverseShapes(shape, types, action);
        }

        /// <summary>
        /// Traverses a single shape and applies the specified action if the shape type is in the specified set of types.
        /// </summary>
        internal static void TraverseShapes(
            Shape shape,
            HashSet<MsoShapeType> types,
            Action<Shape> action)
        {
            if (shape == null) return;

            if (shape.Type == MsoShapeType.msoGroup)
            {
                foreach (Shape inner in shape.GroupItems)
                    TraverseShapes(inner, types, action);

                return;
            }

            if (types != null && types.Contains(shape.Type))
                action?.Invoke(shape);
        }

        /// <summary>
        /// Traverses all inline shapes in the specified range and applies the specified action to each inline shape of the specified types.
        /// </summary>
        internal static void TraverseInlineShapes(
            Range range,
            HashSet<WdInlineShapeType> types,
            Action<InlineShape> action)
        {
            if (range == null || types == null || action == null)
                return;

            foreach (InlineShape inlineShape in range.InlineShapes)
                TraverseInlineShapes(inlineShape, types, action);
        }

        /// <summary>
        /// Traverses a single inline shape and applies the specified action if the inline shape type is in the specified set of types.
        /// </summary>
        internal static void TraverseInlineShapes(
            InlineShape inlineShape,
            HashSet<WdInlineShapeType> types,
            Action<InlineShape> action)
        {
            if (inlineShape == null)
                return;

            if (types.Contains(inlineShape.Type))
                action(inlineShape);
        }

        /// <summary>
        /// Wrapper for running an action under an undo record in Word.
        /// </summary>
        internal static void RunWithUndo(string undoRecordName, string errorTitle, Action action)
        {
            // TODO: improve
            // 1. add error message prexif arg
            // 2. add toggle for screen updating
            // 3. use this wrapper in conversion classes to avoid code duplication
            var app = Globals.ThisAddIn.Application;
            if (app == null || app.ActiveDocument == null) return;

            var undo = app.UndoRecord;

            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord(undoRecordName);

            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    errorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                app.ScreenUpdating = true;
            }
        }

        // TODO: implement some common null-checking methods to avoid repetitive null checks in the code
    }
}
