﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.R.Package.Commands;
using Microsoft.VisualStudio.R.Package.ProjectSystem;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.R.Package.Sql.Publish;
using Microsoft.VisualStudio.R.Package.ProjectSystem.Configuration;
using System.Threading.Tasks;
using Microsoft.R.Components.Sql.Publish;
#if VS14
using Microsoft.VisualStudio.ProjectSystem.Designers;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
#else
using Microsoft.VisualStudio.ProjectSystem;
#endif

namespace Microsoft.VisualStudio.R.Package.Sql {
    [ExportCommandGroup("AD87578C-B324-44DC-A12A-B01A6ED5C6E3")]
    [AppliesTo(ProjectConstants.RtvsProjectCapability)]
    internal sealed class PublishSProcOptionsCommand : IAsyncCommandGroupHandler {
        private readonly IApplicationShell _appShell;
        private readonly IProjectSystemServices _pss;
        private readonly IProjectConfigurationSettingsProvider _pcsp;
        private readonly IDacPackageServicesProvider _dacServicesProvider;

        [ImportingConstructor]
        public PublishSProcOptionsCommand(IApplicationShell appShell, IProjectSystemServices pss, IProjectConfigurationSettingsProvider pcsp, IDacPackageServicesProvider dacServicesProvider) {
            _appShell = appShell;
            _pss = pss;
            _pcsp = pcsp;
            _dacServicesProvider = dacServicesProvider;
        }

        public Task<CommandStatusResult> GetCommandStatusAsync(IImmutableSet<IProjectTree> nodes, long commandId, bool focused, string commandText, CommandStatus progressiveStatus) {
            if (commandId == RPackageCommandId.icmdPublishSProcOptions) {
                return Task.FromResult(new CommandStatusResult(true, commandText, CommandStatus.Enabled | CommandStatus.Supported));
            }
            return Task.FromResult(CommandStatusResult.Unhandled);
        }

        public async Task<bool> TryHandleCommandAsync(IImmutableSet<IProjectTree> nodes, long commandId, bool focused, long commandExecuteOptions, IntPtr variantArgIn, IntPtr variantArgOut) {
            if (commandId == RPackageCommandId.icmdPublishSProcOptions) {
                if (_dacServicesProvider.GetDacPackageServices() != null) {
                    var dlg = await SqlPublshOptionsDialog.CreateAsync(_appShell, _pss, _pcsp);
                    dlg.ShowModal();
                } else {
#if VS14
                    var message = Resources.SqlPublish_NoSqlToolsVS14;
#else
                    var message = Resources.SqlPublish_NoSqlToolsVS15;
#endif
                    _appShell.ShowErrorMessage(message);
                }
                return true;
            }
            return false;
        }
    }
}
