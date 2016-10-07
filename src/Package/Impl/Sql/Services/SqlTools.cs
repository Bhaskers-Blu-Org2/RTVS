﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Common.Core.Shell;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.R.Package.Sql {
    internal static class SqlTools {
        public static bool CheckInstalled(ICoreShell coreShell, bool showMessage) {
            var fers = coreShell.ExportProvider.GetExportedValue< IFileExtensionRegistryService>();
            var ctrs = coreShell.ExportProvider.GetExportedValue<IContentTypeRegistryService>();
            bool installed = false;

            // Try modern registration first
            var ct = fers.GetContentTypeForExtension(".sql");
            if (ct != null && ct != ctrs.UnknownContentType) {
                installed = true;
            } else {
                // Fall back to IVs* type of registration
                ct = ctrs.GetContentType("SQL Server Tools");
                installed = ct != null && ct != ctrs.UnknownContentType;
            }

            if(!installed && showMessage) {
#if VS14
                var message = Resources.SqlPublish_NoSqlToolsVS14;
#else
                var message = Resources.SqlPublish_NoSqlToolsVS15;
#endif
                coreShell.ShowErrorMessage(message);
            }
            return installed;
        }
    }
}
