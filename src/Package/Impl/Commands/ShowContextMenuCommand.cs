﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Common.Core.Services;
using Microsoft.Common.Core.UI.Commands;
using Microsoft.Languages.Editor.Controllers.Commands;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using IMenuCommandService = System.ComponentModel.Design.IMenuCommandService;
using CommandID = System.ComponentModel.Design.CommandID;
using System.Windows.Threading;

namespace Microsoft.VisualStudio.R.Package.Commands {
    internal class ShowContextMenuCommand : ViewCommand {
        private readonly IServiceContainer _services;
        private readonly Guid _cmdSetGuid;
        private readonly int _menuId;
        private Guid _packageGuid;
        private IMenuCommandService _menuService;
        private bool _triedGetMenuService;

        public ShowContextMenuCommand(ITextView textView, Guid packageGuid, Guid cmdSetGuid, int menuId, IServiceContainer services)
            : base(textView, new CommandId(VSConstants.VSStd2K, (int)VSConstants.VSStd2KCmdID.SHOWCONTEXTMENU), false) {
            _services = services;
            _cmdSetGuid = cmdSetGuid;
            _packageGuid = packageGuid;
            _menuId = menuId;
        }

        public override CommandStatus Status(Guid group, int id) {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            return MenuCommandService != null ? CommandStatus.SupportedAndEnabled : CommandStatus.NotSupported;
        }

        public override CommandResult Invoke(Guid group, int id, object inputArg, ref object outputArg) {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            if (MenuCommandService != null) {
                var position = (POINTS[])inputArg;
                var menuCommand = new CommandID(_cmdSetGuid, (int)_menuId);
                MenuCommandService.ShowContextMenu(menuCommand, position[0].x, position[0].y);

                return CommandResult.Executed;
            }
            return CommandResult.NotSupported;
        }

        // IMenuCommandService is null in weird scenarios, such as Open With <non-html editor>, or diff view
        private IMenuCommandService MenuCommandService {
            get {
                Dispatcher.CurrentDispatcher.VerifyAccess();

                if (_menuService == null && !_triedGetMenuService) {
                    _triedGetMenuService = true;

                    var shell = _services.GetService<IVsShell>(typeof(SVsShell));
                    shell.LoadPackage(ref _packageGuid, out var package);
                    if (package != null) {
                        var services = package as IServiceProvider;
                        _menuService = services.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
                    }

                    Debug.Assert(_menuService != null);
                }

                return _menuService;
            }
        }
    }
}
