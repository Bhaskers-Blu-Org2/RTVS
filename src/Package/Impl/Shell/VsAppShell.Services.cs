﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using Microsoft.Common.Core.Imaging;
using Microsoft.Common.Core.IO;
using Microsoft.Common.Core.Logging;
using Microsoft.Common.Core.Security;
using Microsoft.Common.Core.Services;
using Microsoft.Common.Core.Shell;
using Microsoft.Common.Core.Tasks;
using Microsoft.Common.Core.Threading;
using Microsoft.Languages.Editor.Text;
using Microsoft.Markdown.Editor.Settings;
using Microsoft.R.Common.Core.Output;
using Microsoft.R.Components;
using Microsoft.R.Components.Security;
using Microsoft.R.Components.Settings;
using Microsoft.R.Components.StatusBar;
using Microsoft.R.Containers;
using Microsoft.R.Editor;
using Microsoft.R.Editor.Settings;
using Microsoft.R.Host.Client;
using Microsoft.R.Platform;
using Microsoft.R.Platform.IO;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.R.Package.Imaging;
using Microsoft.VisualStudio.R.Package.Options.R;
using Microsoft.VisualStudio.R.Package.StatusBar;
using Microsoft.VisualStudio.R.Packages.R;
using VsPackage = Microsoft.VisualStudio.Shell.Package;

namespace Microsoft.VisualStudio.R.Package.Shell {
    public partial class VsAppShell {
        private readonly VsServiceManager _services;
        private VsApplication _application;

        public IServiceContainer Services => _services;

        private void ConfigureServices() {
            _services
                .AddService<IActionLog>(s => new Logger(VsApplication.Name, Path.GetTempPath(), s))
                .AddService<IMainThread, VsMainThread>()
                .AddService<IOutputService, VsOutputService>()
                .AddService<ISettingsStorage, VsSettingsStorage>()
                .AddService<IRSettings, RSettingsImplementation>()
                .AddService<ITaskService, VsTaskService>()
                .AddService<ISecurityService, WindowsSecurityService>()
                .AddService<IFileSystem, WindowsFileSystem>();
        }

        private void ConfigurePackageServices() {
            var platformServices = new VsPlatformServices();

            var componentModel = (IComponentModel)VsPackage.GetGlobalService(typeof(SComponentModel));
            var compositionCatalog = new CompositionCatalog(componentModel.DefaultCompositionService, componentModel.DefaultExportProvider);
            var exportProvider = componentModel.DefaultExportProvider;

            _services
                .AddService(componentModel)
                .AddService(componentModel.DefaultCompositionService)
                .AddService(exportProvider)
                .AddService(compositionCatalog)
                .AddService(new VsUIServices(this))
                .AddService(new VsPlotExportDialog(this))
                .AddService(platformServices)
                .AddService<IEditorSupport, VsEditorSupport>()
                .AddService<IImageService, ImageService>()
                .AddService(new REditorSettings(this))
                .AddService(new RMarkdownEditorSettings(this))
                .AddService<IStatusBar, VsStatusBar>()
                .AddService<RPackageToolWindowProvider>()
                .AddRComponentsServices()
                .AddWindowsPlatformServices()
                .AddWindowsHostClientServices()
                .AddWindowsRComponentsServices()
                .AddEditorServices()
                .AddWindowsContainerServices();
            // TODO: add more

            _application = new VsApplication(_services);
            _services.AddService(_application);
            _services.GetService<IRSettings>().LoadSettings();
        }
    }
}
