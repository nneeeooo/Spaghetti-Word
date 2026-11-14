using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Word.Modules
{
    // Kept free of Word interop types so the unit tests can link just this file.
    internal static partial class Images
    {
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

            // Dispose the package so the file is released before Word reopens it.
            using (var wordDoc = WordprocessingDocument.Open(docxPath, false))
            {
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
        }

        /// <summary>
        /// Gets the file extension for an image based on its content type.
        /// </summary>
        internal static string GetImageExtension(string contentType)
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
