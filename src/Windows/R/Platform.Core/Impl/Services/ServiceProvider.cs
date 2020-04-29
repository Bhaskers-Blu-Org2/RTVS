﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Common.Core.Services;
using Microsoft.R.Platform.Interpreters;
using Microsoft.R.Platform.IO;
using Microsoft.R.Platform.OS;
using Microsoft.R.Platform.Shell;

namespace Microsoft.R.Platform {
    public static class ServicesExtensions {
        public static IServiceManager AddWindowsPlatformServices(this IServiceManager serviceManager)
            => serviceManager
                .AddService(new WindowsFileSystem())
                .AddService(new WindowsProcessServices())
                .AddService<IRegistry, RegistryImpl>()
                .AddService<IRInstallationService, RInstallation>();
     }

    /// <summary>
    /// Invoked via reflection to populate service container
    /// with platform-specific services such as R discovery,
    /// file system, process management.
    /// </summary>
    public static class ServiceProvider {
        public static void ProvideServices(IServiceManager services) {
            services
                .AddWindowsPlatformServices()
                .AddService(new WindowsPlatformServices());
        }
    }
}
