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
    internal static class Images
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

        /// <summary>
        /// Extract image from a specific ImagePart and save it to the output folder.
        /// </summary>
        private static void ExtractImagesByPart(ImagePart imagePart, string outputFolder, int index)
        {
            var extension = GetImageExtension(imagePart.ContentType);
            var outputPath = Path.Combine(outputFolder, $"image_{index:D3}{extension}");

            using (var partStream = imagePart.GetStream())
            using (var fileStream = File.Create(outputPath))
            {
                partStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Extracts all images from a DOCX file and saves them to the specified output folder.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public static void ExtractImages(string docxPath, string outputFolder)
        {
            if (!File.Exists(docxPath))
                throw new FileNotFoundException("DOCX file not found.", docxPath);

            var wordDoc = WordprocessingDocument.Open(docxPath, false);
            var mainPart = wordDoc.MainDocumentPart
                ?? throw new InvalidDataException("Main document part is missing.");

            var imageParts = mainPart.ImageParts
                .Concat(mainPart.HeaderParts.SelectMany(h => h.ImageParts))
                .Concat(mainPart.FooterParts.SelectMany(f => f.ImageParts))
                .ToList();

            if (!imageParts.Any())
                throw new InvalidDataException("No images found in the document.");

            Directory.CreateDirectory(outputFolder);

            imageParts
                .Select((part, index) => (part, index: index + 1))
                .ToList()
                .ForEach(x => ExtractImagesByPart(x.part, outputFolder, x.index));
        }

        /// <summary>
        /// Gets the file extension for an image based on its content type.
        /// </summary>
        private static string GetImageExtension(string contentType)
        {
            switch (contentType)
            {
                case "image/png": return ".png";
                case "image/jpeg": return ".jpg";
                case "image/gif": return ".gif";
                case "image/tiff": return ".tif";
                case "image/bmp": return ".bmp";
                case "image/x-emf": return ".emf";
                case "image/x-wmf": return ".wmf";
                default: return ".bin";
            }
        }
    }
}
