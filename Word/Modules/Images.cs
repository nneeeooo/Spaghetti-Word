using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;

namespace Word.Modules
{
    /// <summary>
    /// Image handling utilities for Word documents.
    /// </summary>
    internal static partial class Images
    {
        /// <summary>
        /// Locks the aspect ratio of all images in the specified range.
        /// </summary>
        internal static void LockImageAspectRatio(Range range)
        {
            var lockableShapeTypes = new HashSet<MsoShapeType>
            {
                MsoShapeType.msoPicture,
                MsoShapeType.msoLinkedPicture,
                MsoShapeType.msoLinkedOLEObject,
                MsoShapeType.msoEmbeddedOLEObject,
                MsoShapeType.msoMedia
            };

            var lockableInlineShapeTypes = new HashSet<WdInlineShapeType>()
            {
                WdInlineShapeType.wdInlineShapePicture,
                WdInlineShapeType.wdInlineShapeLinkedPicture,
                WdInlineShapeType.wdInlineShapeEmbeddedOLEObject,
                WdInlineShapeType.wdInlineShapeLinkedOLEObject
            };

            Shared.TraverseShapes(range, lockableShapeTypes, shape =>
            {
                shape.LockAspectRatio = MsoTriState.msoTrue;
            });

            Shared.TraverseInlineShapes(range, lockableInlineShapeTypes, shape =>
            {
                shape.LockAspectRatio = MsoTriState.msoTrue;
            });
        }
    }
}
