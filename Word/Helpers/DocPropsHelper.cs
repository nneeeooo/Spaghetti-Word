using System;
using Microsoft.Office.Core;

namespace Word.Helpers
{
    /// <summary>
    /// Helper for managing custom document properties in Word documents.
    /// </summary>
    internal static class DocPropsHelper
    {
        internal static void SaveValue(string key, bool value)
        {
            SaveOrUpdateProperty(key, MsoDocProperties.msoPropertyTypeBoolean, value);
        }

        internal static void SaveValue(string key, string value)
        {
            SaveOrUpdateProperty(key, MsoDocProperties.msoPropertyTypeString, value);
        }

        internal static void SaveOrUpdateProperty(string key, MsoDocProperties type, object value)
        {
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            var props = doc.CustomDocumentProperties;

            try
            {
                foreach (DocumentProperty prop in props)
                {
                    if (prop.Name != key) continue;
                    prop.Delete();
                    break;
                }

                props.Add(key, false, type, value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SaveValue error: " + ex.Message);
            }
        }

        internal static string LoadValue(string key, string defaultValue = "")
        {
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            var props = doc.CustomDocumentProperties;

            try
            {
                foreach (DocumentProperty prop in props)
                {
                    if (prop.Name == key && prop.Type == MsoDocProperties.msoPropertyTypeString)
                        return (string)prop.Value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadValue (string) error: " + ex.Message);
            }

            return defaultValue;
        }

        internal static void DeleteValue(string key)
        {
            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            var props = doc.CustomDocumentProperties;

            try
            {
                foreach (DocumentProperty prop in props)
                {
                    if (prop.Name != key) continue;
                    prop.Delete();
                    break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteValue error: " + ex.Message);
            }
        }
    }
}
