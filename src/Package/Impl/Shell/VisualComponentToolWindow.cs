﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Design;
using System.Windows;
using Microsoft.R.Components.View;
using Microsoft.VisualStudio.R.Package.Windows;

namespace Microsoft.VisualStudio.R.Package.Shell {
    internal abstract class VisualComponentToolWindow<T> : RToolWindowPane, IVisualComponentContainer<T> where T : IVisualComponent {
        private readonly VisualComponentToolWindowAdapter<T> _adapter;

        public T Component {
            get { return _adapter.Component; }
            protected set {
                _adapter.Component = value;
                Content = value?.Control;
            }
        }

        public string CaptionText
        {
            get { return _adapter.CaptionText; }
            set { _adapter.CaptionText = value; }
        }

        public string StatusText
        {
            get { return _adapter.StatusText; }
            set { _adapter.StatusText = value; }
        }

        public bool IsOnScreen => _adapter?.IsOnScreen ?? false;

        protected VisualComponentToolWindow() {
            _adapter = new VisualComponentToolWindowAdapter<T>(this);
        }

        public void Hide() {
            _adapter?.Hide();
        }

        public void Show(bool focus, bool immediate) {
            _adapter?.Show(focus, immediate);
        }

        public void ShowContextMenu(CommandID commandId, Point position) {
            _adapter?.ShowContextMenu(commandId, position);
        }

        public void UpdateCommandStatus(bool immediate) {
            _adapter?.UpdateCommandStatus(immediate);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && Component != null) {
                Component.Dispose();
                Component = default(T);
            }
            base.Dispose(disposing);
        }
    }
}