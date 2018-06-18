//-----------------------------------------------------------------------
// <copyright file="ULSLogging.cs" company="Robert Bosch Engineering and Business Solutions Vietnam">
//     Copyright (c) Robert Bosch Engineering and Business Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>This is the ULSLogging class.</summary>
//-----------------------------------------------------------------------
namespace RBVH.Core.SharePoint
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.SharePoint.Administration;

    /// <summary>
    /// ULS Logging for Sharepoint
    /// </summary>
    public class ULSLogging : SPDiagnosticsServiceBase
    {
        /// <summary>
        /// Area Name
        /// </summary>
        private const string AreaNameDisagnostic = "STADA";

        /// <summary>
        /// Current logging
        /// </summary>
        private static ULSLogging _current;

        #region <Constructor>
        /// <summary>
        /// Prevents a default instance of the ULSLogging class from being created.
        /// </summary>
        private ULSLogging()
            : base("Logging Service", SPFarm.Local)
        {
        }

        /// <summary>
        /// Diagnostics Category
        /// </summary>
        public enum DiagnosticsCategory
        {
            Warning,
            Debug,
            Error
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ULSLogging"/> class.
        /// </summary>
        public static ULSLogging Current => _current ?? (_current = new ULSLogging());
        #endregion <Constructor>

        /// <summary>
        ///     Write log to ULS
        /// </summary>
        /// <param name="category">Name of Category</param>
        /// <param name="traceseverity">Trace severity</param>
        /// <param name="message">Message to log</param>
        public static void Log(SPDiagnosticsCategory category, TraceSeverity traceseverity, string message)
        {
            try
            {
                Current.WriteTrace(0, category, traceseverity, message);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Log message to file
        /// </summary>
        /// <param name="category">Message category</param>
        /// <param name="traceseverity">trace severity</param>
        /// <param name="message">Message to log</param>
        public static void LogMessageToFile(SPDiagnosticsCategory category, TraceSeverity traceseverity, string message)
        {
            var fileName = $"log_pages_{DateTime.Now.ToString("dd_MM_yyyy")}.txt";
            var filePath = Path.Combine("c:\\logs", fileName);

            FileInfo fi = new FileInfo(filePath);
            if (!fi.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fi.DirectoryName);
            }

            var sw = new StreamWriter(filePath, true);
            try
            {
                var logLine = string.Format("{0:G}: {1} {2} {3}.", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.ff"), category.Name, traceseverity, message);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Log message to file
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogMessageToFile(string message)
        {
            try
            {
                LogMessageToFile(Current.Areas[AreaNameDisagnostic].Categories[DiagnosticsCategory.Debug.ToString()], TraceSeverity.Verbose, message);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        /// <summary>
        /// Write log to ULS
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public static void LogError(Exception ex)
        {
            try
            {
                var category = Current.Areas[AreaNameDisagnostic].Categories[DiagnosticsCategory.Error.ToString()];
                Log(category, TraceSeverity.Unexpected, ex.Message + " => " + ex.StackTrace);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogWarning(string message)
        {
            try
            {
                var category = Current.Areas[AreaNameDisagnostic].Categories[DiagnosticsCategory.Warning.ToString()];
                Log(category, TraceSeverity.Medium, message);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogMessage(string message)
        {
            try
            {
                var category = Current.Areas[AreaNameDisagnostic].Categories[DiagnosticsCategory.Debug.ToString()];
                Log(category, TraceSeverity.Verbose, message);
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        ///     Provides the areas.
        /// </summary>
        /// <returns>SharePoint Diagnostic Area</returns>
        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            var areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(AreaNameDisagnostic, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory(DiagnosticsCategory.Warning.ToString(), TraceSeverity.Medium, EventSeverity.Warning),
                    new SPDiagnosticsCategory(DiagnosticsCategory.Debug.ToString(), TraceSeverity.Medium, EventSeverity.Information),
                    new SPDiagnosticsCategory(DiagnosticsCategory.Error.ToString(), TraceSeverity.Unexpected, EventSeverity.Error)
                })
            };

            return areas;
        }
    }
}