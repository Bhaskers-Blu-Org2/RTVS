﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using Microsoft.R.Components.Plots;
using Microsoft.R.Components.View;
using Microsoft.R.Support.Settings;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.R.Package.Windows;

namespace Microsoft.VisualStudio.R.Package.ToolWindows {
    [Export(typeof(IRPlotHistoryVisualComponentContainerFactory))]
    internal class VsRPlotHistoryVisualComponentContainerFactory : ToolWindowPaneFactory<PlotHistoryWindowPane>, IRPlotHistoryVisualComponentContainerFactory {
        public IVisualComponentContainer<IRPlotHistoryVisualComponent> GetOrCreate(IRPlotManager plotManager, int instanceId = 0) {
            return GetOrCreate(instanceId, i => new PlotHistoryWindowPane(plotManager, i, RToolsSettings.Current, VsAppShell.Current));
        }
    }
}
