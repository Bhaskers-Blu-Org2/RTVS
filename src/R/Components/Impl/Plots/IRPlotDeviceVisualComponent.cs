﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.R.Components.View;
using Microsoft.R.Host.Client;

namespace Microsoft.R.Components.Plots {
    public interface IRPlotDeviceVisualComponent : IVisualComponent {
        PlotDeviceProperties GetDeviceProperties();
        Task AssignAsync(IRPlotDevice device);
        Task UnassignAsync();
        int InstanceId { get; }
        bool HasPlot { get; }
        bool LocatorMode { get; }
        int ActivePlotIndex { get; }
        int PlotCount { get; }
        string DeviceName { get; }
        bool IsDeviceActive { get; }
        IRPlotDevice Device { get; }
        IRPlot ActivePlot { get; }
        Task<LocatorResult> StartLocatorModeAsync(CancellationToken ct);
        void EndLocatorMode();
        void ClickPlot(int x, int y);
    }
}
