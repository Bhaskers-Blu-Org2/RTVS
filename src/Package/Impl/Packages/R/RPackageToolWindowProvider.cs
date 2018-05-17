﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Common.Core.Services;
using Microsoft.R.Components.Help;
using Microsoft.R.Components.History;
using Microsoft.R.Components.InteractiveWorkflow;
using Microsoft.R.Components.PackageManager;
using Microsoft.R.Components.Plots;
using Microsoft.R.Components.View;
using Microsoft.VisualStudio.R.Package.Help;
using Microsoft.VisualStudio.R.Package.History;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.R.Package.ToolWindows;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.R.Packages.R {
    internal sealed class RPackageToolWindowProvider {
        private readonly IServiceContainer _services;
        private readonly IRInteractiveWorkflowVisualProvider _workflowProvider;
        private VsRInteractiveWorkflowToolWindowService WorkflowToolWindows 
            => (VsRInteractiveWorkflowToolWindowService)_workflowProvider.GetOrCreate().ToolWindows;

        public RPackageToolWindowProvider(IServiceContainer services) {
            _services = services;
            _workflowProvider = _services.GetService<IRInteractiveWorkflowVisualProvider>();
        }

        public ToolWindowPane CreateToolWindow(Guid toolWindowGuid, int id) 
            => WorkflowToolWindows.GetOrCreate(toolWindowGuid, id) ?? GetToolWindowFromVisualComponent(toolWindowGuid, id);

        private ToolWindowPane GetToolWindowFromVisualComponent(Guid toolWindowGuid, int id) {
            var container = CreateVisualComponent(toolWindowGuid, id)?.Container;
            if (container == null) {
                return null;
            }

            if (container is IVisualComponentToolWindowAdapter<IVisualComponent> toolWindowAdapter) {
                return toolWindowAdapter.ToolWindow;
            }

            return (ToolWindowPane)container;
        }

        private IVisualComponent CreateVisualComponent(Guid toolWindowGuid, int id) {
            if (toolWindowGuid == RGuidList.ReplInteractiveWindowProviderGuid) {
                return CreateInteractiveWindow(id);
            }

            if (toolWindowGuid == HistoryWindowPane.WindowGuid) {
                return CreateHistoryToolWindow(id);
            }

            if (toolWindowGuid == PlotDeviceWindowPane.WindowGuid) {
                return CreatePlotDeviceToolWindow(id);
            }

            if (toolWindowGuid == PlotHistoryWindowPane.WindowGuid) {
                return CreatePlotHistoryToolWindow(id);
            }

            return toolWindowGuid == HelpWindowPane.WindowGuid ? CreateHelpToolWindow(id) : null;
        }

        private IInteractiveWindowVisualComponent CreateInteractiveWindow(int id) {
            var workflow = _workflowProvider.GetOrCreate();
            _services.Tasks().Wait(() => workflow.GetOrCreateVisualComponentAsync(id), out var result);
            return result;
        }

        private IRHistoryWindowVisualComponent CreateHistoryToolWindow(int id) {
            var factory = _services.GetService<IRHistoryVisualComponentContainerFactory>();
            var workflow = _workflowProvider.GetOrCreate();
            return workflow.History.GetOrCreateVisualComponent(factory, id);
        }
        
        private IRPlotDeviceVisualComponent CreatePlotDeviceToolWindow(int id) {
            var factory = _services.GetService<IRPlotDeviceVisualComponentContainerFactory>();
            var workflow = _workflowProvider.GetOrCreate();
            return workflow.Plots.GetOrCreateVisualComponent(factory, id);
        }

        private IRPlotHistoryVisualComponent CreatePlotHistoryToolWindow(int id) {
            var factory = _services.GetService<IRPlotHistoryVisualComponentContainerFactory>();
            var workflow = _workflowProvider.GetOrCreate();
            return workflow.Plots.GetOrCreateVisualComponent(factory, id);
        }

        private IHelpVisualComponent CreateHelpToolWindow(int id) {
            var factory = _services.GetService<IHelpVisualComponentContainerFactory>();
            return factory.GetOrCreate(id).Component;
        }
    }
}