﻿#region BSD License
/*
 * 
 *  New BSD 3-Clause License (https://github.com/Krypton-Suite/Standard-Toolkit/blob/master/LICENSE)
 *  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV), et al. 2023 - 2023. All rights reserved. 
 *  
 */
#endregion

namespace Krypton.Ribbon
{
    internal class KryptonRibbonGroupThemeComboBoxDesigner : ComponentDesigner, IKryptonDesignObject
    {
        #region Instance Fields
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;
        private KryptonRibbonGroupThemeComboBox _ribbonThemeComboBox;
        private DesignerVerbCollection _verbs;
        private DesignerVerb _toggleHelpersVerb;
        private DesignerVerb _moveFirstVerb;
        private DesignerVerb _movePrevVerb;
        private DesignerVerb _moveNextVerb;
        private DesignerVerb _moveLastVerb;
        private DesignerVerb _deleteComboBoxVerb;
        private ContextMenuStrip? _cms;
        private ToolStripMenuItem _toggleHelpersMenu;
        private ToolStripMenuItem _visibleMenu;
        private ToolStripMenuItem _moveFirstMenu;
        private ToolStripMenuItem _movePreviousMenu;
        private ToolStripMenuItem _moveNextMenu;
        private ToolStripMenuItem _moveLastMenu;
        private ToolStripMenuItem _deleteComboBoxMenu;

        #endregion

        #region Identity
        /// <summary>
        /// Initialize a new instance of the KryptonRibbonGroupThemeComboBoxDesigner class.
        /// </summary>
        public KryptonRibbonGroupThemeComboBoxDesigner()
        {
        }
        #endregion

        #region Public
        /// <summary>
        /// Initializes the designer with the specified component.
        /// </summary>
        /// <param name="component">The IComponent to associate the designer with.</param>
        public override void Initialize(IComponent component)
        {
            // Let base class do standard stuff
            base.Initialize(component);

            Debug.Assert(component != null);

            // Cast to correct type
            _ribbonThemeComboBox = component as KryptonRibbonGroupThemeComboBox;
            if (_ribbonThemeComboBox != null)
            {
                _ribbonThemeComboBox.ComboBoxDesigner = this;

                // Update designer properties with actual starting values
                Visible = _ribbonThemeComboBox.Visible;
                Enabled = _ribbonThemeComboBox.Enabled;

                // Update visible/enabled to always be showing/enabled at design time
                _ribbonThemeComboBox.Visible = true;
                _ribbonThemeComboBox.Enabled = true;

                // Tell the embedded text box it is in design mode
                _ribbonThemeComboBox.ComboBox.InRibbonDesignMode = true;

                // Hook into events
                _ribbonThemeComboBox.DesignTimeContextMenu += OnContextMenu;
            }

            // Get access to the services
            _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // We need to know when we are being removed/changed
            _changeService.ComponentChanged += OnComponentChanged;
        }

        /// <summary>
        /// Gets the design-time verbs supported by the component that is associated with the designer.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                UpdateVerbStatus();
                return _verbs;
            }
        }

        /// <summary>
        /// Gets and sets if the object is enabled.
        /// </summary>
        public bool DesignEnabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        /// <summary>
        /// Gets and sets if the object is visible.
        /// </summary>
        public bool DesignVisible
        {
            get => Visible;
            set => Visible = value;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Releases all resources used by the component. 
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Unhook from events
                    _ribbonThemeComboBox.DesignTimeContextMenu -= OnContextMenu;
                    _changeService.ComponentChanged -= OnComponentChanged;
                }
            }
            finally
            {
                // Must let base class do standard stuff
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Adjusts the set of properties the component exposes through a TypeDescriptor.
        /// </summary>
        /// <param name="properties">An IDictionary containing the properties for the class of the component.</param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // Setup the array of properties we override
            var attributes = Array.Empty<Attribute>();
            string[] strArray = { nameof(Visible), nameof(Enabled) };

            // Adjust our list of properties
            for (var i = 0; i < strArray.Length; i++)
            {
                var descrip = (PropertyDescriptor)properties[strArray[i]];
                if (descrip != null)
                {
                    properties[strArray[i]] = TypeDescriptor.CreateProperty(typeof(KryptonRibbonGroupThemeComboBoxDesigner), descrip, attributes);
                }
            }
        }
        #endregion

        #region Internal
        internal bool Visible { get; set; }

        internal bool Enabled { get; set; }

        #endregion

        #region Implementation
        private void ResetVisible() => Visible = true;

        private bool ShouldSerializeVisible() => !Visible;

        private void ResetEnabled() => Enabled = true;

        private bool ShouldSerializeEnabled() => !Enabled;

        private void UpdateVerbStatus()
        {
            // Create verbs first time around
            if (_verbs == null)
            {
                _verbs = [];
                _toggleHelpersVerb = new DesignerVerb(@"Toggle Helpers", OnToggleHelpers);
                _moveFirstVerb = new DesignerVerb(@"Move ComboBox First", OnMoveFirst);
                _movePrevVerb = new DesignerVerb(@"Move ComboBox Previous", OnMovePrevious);
                _moveNextVerb = new DesignerVerb(@"Move ComboBox Next", OnMoveNext);
                _moveLastVerb = new DesignerVerb(@"Move ComboBox Last", OnMoveLast);
                _deleteComboBoxVerb = new DesignerVerb(@"Delete ThemeComboBox", OnDeleteThemeComboBox);
                _verbs.AddRange(new[] { _toggleHelpersVerb, _moveFirstVerb, _movePrevVerb,
                                                     _moveNextVerb, _moveLastVerb, _deleteComboBoxVerb });
            }

            var moveFirst = false;
            var movePrev = false;
            var moveNext = false;
            var moveLast = false;

            if (_ribbonThemeComboBox.Ribbon != null)
            {
                var items = ParentItems;
                moveFirst = items.IndexOf(_ribbonThemeComboBox) > 0;
                movePrev = items.IndexOf(_ribbonThemeComboBox) > 0;
                moveNext = items.IndexOf(_ribbonThemeComboBox) < (items.Count - 1);
                moveLast = items.IndexOf(_ribbonThemeComboBox) < (items.Count - 1);
            }

            _moveFirstVerb.Enabled = moveFirst;
            _movePrevVerb.Enabled = movePrev;
            _moveNextVerb.Enabled = moveNext;
            _moveLastVerb.Enabled = moveLast;
        }

        private void OnToggleHelpers(object sender, EventArgs e)
        {
            // Invert the current toggle helper mode
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                _ribbonThemeComboBox.Ribbon.InDesignHelperMode = !_ribbonThemeComboBox.Ribbon.InDesignHelperMode;
            }
        }

        private void OnMoveFirst(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Get access to the parent collection of items
                var items = ParentItems;

                // Use a transaction to support undo/redo actions
                DesignerTransaction transaction = _designerHost.CreateTransaction(@"KryptonRibbonGroupThemeComboBoxBox MoveFirst");

                try
                {
                    // Get access to the Items property
                    MemberDescriptor propertyItems = TypeDescriptor.GetProperties(_ribbonThemeComboBox.RibbonContainer)[@"Items"];

                    RaiseComponentChanging(propertyItems);

                    // Move position of the combobox
                    items.Remove(_ribbonThemeComboBox);
                    items.Insert(0, _ribbonThemeComboBox);
                    UpdateVerbStatus();

                    RaiseComponentChanged(propertyItems, null, null);
                }
                finally
                {
                    // If we managed to create the transaction, then do it
                    transaction?.Commit();
                }
            }
        }

        private void OnMovePrevious(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Get access to the parent collection of items
                var items = ParentItems;

                // Use a transaction to support undo/redo actions
                DesignerTransaction transaction = _designerHost.CreateTransaction(@"KryptonRibbonGroupThemeComboBox MovePrevious");

                try
                {
                    // Get access to the Items property
                    MemberDescriptor propertyItems = TypeDescriptor.GetProperties(_ribbonThemeComboBox.RibbonContainer)[@"Items"];

                    RaiseComponentChanging(propertyItems);

                    // Move position of the combotextbox
                    var index = items.IndexOf(_ribbonThemeComboBox) - 1;
                    index = Math.Max(index, 0);
                    items.Remove(_ribbonThemeComboBox);
                    items.Insert(index, _ribbonThemeComboBox);
                    UpdateVerbStatus();

                    RaiseComponentChanged(propertyItems, null, null);
                }
                finally
                {
                    // If we managed to create the transaction, then do it
                    transaction?.Commit();
                }
            }
        }

        private void OnMoveNext(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Get access to the parent collection of items
                var items = ParentItems;

                // Use a transaction to support undo/redo actions
                DesignerTransaction transaction = _designerHost.CreateTransaction(@"KryptonRibbonGroupThemeComboBox MoveNext");

                try
                {
                    // Get access to the Items property
                    MemberDescriptor propertyItems = TypeDescriptor.GetProperties(_ribbonThemeComboBox.RibbonContainer)[@"Items"];

                    RaiseComponentChanging(propertyItems);

                    // Move position of the combobox
                    var index = items.IndexOf(_ribbonThemeComboBox) + 1;
                    index = Math.Min(index, items.Count - 1);
                    items.Remove(_ribbonThemeComboBox);
                    items.Insert(index, _ribbonThemeComboBox);
                    UpdateVerbStatus();

                    RaiseComponentChanged(propertyItems, null, null);
                }
                finally
                {
                    // If we managed to create the transaction, then do it
                    transaction?.Commit();
                }
            }
        }

        private void OnMoveLast(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Get access to the parent collection of items
                var items = ParentItems;

                // Use a transaction to support undo/redo actions
                DesignerTransaction transaction = _designerHost.CreateTransaction(@"KryptonRibbonGroupThemeComboBox MoveLast");

                try
                {
                    // Get access to the Items property
                    MemberDescriptor propertyItems = TypeDescriptor.GetProperties(_ribbonThemeComboBox.RibbonContainer)[@"Items"];

                    RaiseComponentChanging(propertyItems);

                    // Move position of the combobox
                    items.Remove(_ribbonThemeComboBox);
                    items.Insert(items.Count, _ribbonThemeComboBox);
                    UpdateVerbStatus();

                    RaiseComponentChanged(propertyItems, null, null);
                }
                finally
                {
                    // If we managed to create the transaction, then do it
                    transaction?.Commit();
                }
            }
        }

        private void OnDeleteThemeComboBox(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Get access to the parent collection of items
                var items = ParentItems;

                // Use a transaction to support undo/redo actions
                DesignerTransaction transaction = _designerHost.CreateTransaction(@"KryptonRibbonGroupThemeComboBox DeleteThemeComboBox");

                try
                {
                    // Get access to the Items property
                    MemberDescriptor propertyItems = TypeDescriptor.GetProperties(_ribbonThemeComboBox.RibbonContainer)[@"Items"];

                    RaiseComponentChanging(null);
                    RaiseComponentChanging(propertyItems);

                    // Remove the combobox from the group
                    items.Remove(_ribbonThemeComboBox);

                    // Get designer to destroy it
                    _designerHost.DestroyComponent(_ribbonThemeComboBox);

                    RaiseComponentChanged(propertyItems, null, null);
                    RaiseComponentChanged(null, null, null);
                }
                finally
                {
                    // If we managed to create the transaction, then do it
                    transaction?.Commit();
                }
            }
        }

        private void OnEnabled(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                PropertyDescriptor propertyEnabled = TypeDescriptor.GetProperties(_ribbonThemeComboBox)[nameof(Enabled)];
                var oldValue = (bool)propertyEnabled.GetValue(_ribbonThemeComboBox);
                var newValue = !oldValue;
                _changeService.OnComponentChanged(_ribbonThemeComboBox, null, oldValue, newValue);
                propertyEnabled.SetValue(_ribbonThemeComboBox, newValue);
            }
        }

        private void OnVisible(object sender, EventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                PropertyDescriptor propertyVisible = TypeDescriptor.GetProperties(_ribbonThemeComboBox)[nameof(Visible)];
                var oldValue = (bool)propertyVisible.GetValue(_ribbonThemeComboBox);
                var newValue = !oldValue;
                _changeService.OnComponentChanged(_ribbonThemeComboBox, null, oldValue, newValue);
                propertyVisible.SetValue(_ribbonThemeComboBox, newValue);
            }
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e) => UpdateVerbStatus();

        private void OnContextMenu(object sender, MouseEventArgs e)
        {
            if (_ribbonThemeComboBox.Ribbon != null)
            {
                // Create the menu strip the first time around
                if (_cms == null)
                {
                    _cms = new ContextMenuStrip();
                    _toggleHelpersMenu = new ToolStripMenuItem("Design Helpers", null, OnToggleHelpers);
                    _visibleMenu = new ToolStripMenuItem("Visible", null, OnVisible);
                    _moveFirstMenu = new ToolStripMenuItem("Move ThemeComboBox First", GenericImageResources.MoveFirst, OnMoveFirst);
                    _movePreviousMenu = new ToolStripMenuItem("Move ThemeComboBox Previous", GenericImageResources.MovePrevious, OnMovePrevious);
                    _moveNextMenu = new ToolStripMenuItem("Move ThemeComboBox Next", GenericImageResources.MoveNext, OnMoveNext);
                    _moveLastMenu = new ToolStripMenuItem("Move ThemeComboBox Last", GenericImageResources.MoveLast, OnMoveLast);
                    _deleteComboBoxMenu = new ToolStripMenuItem("Delete ThemeComboBox", GenericImageResources.Delete, OnDeleteThemeComboBox);
                    _cms.Items.AddRange(new ToolStripItem[]
                    {
                        _toggleHelpersMenu, new ToolStripSeparator(),
                        _visibleMenu, new ToolStripSeparator(),
                        _moveFirstMenu, _movePreviousMenu, _moveNextMenu, _moveLastMenu, new ToolStripSeparator(),
                        _deleteComboBoxMenu
                    });
                }

                // Update verbs to work out correct enable states
                UpdateVerbStatus();

                // Update menu items state from verbs
                _toggleHelpersMenu.Checked = _ribbonThemeComboBox.Ribbon.InDesignHelperMode;
                _visibleMenu.Checked = Visible;
                _moveFirstMenu.Enabled = _moveFirstVerb.Enabled;
                _movePreviousMenu.Enabled = _movePrevVerb.Enabled;
                _moveNextMenu.Enabled = _moveNextVerb.Enabled;
                _moveLastMenu.Enabled = _moveLastVerb.Enabled;

                // Show the context menu
                if (CommonHelper.ValidContextMenuStrip(_cms))
                {
                    Point screenPt = _ribbonThemeComboBox.Ribbon.ViewRectangleToPoint(_ribbonThemeComboBox.ComboBoxView);
                    VisualPopupManager.Singleton.ShowContextMenuStrip(_cms, screenPt);
                }
            }
        }

        private TypedRestrictCollection<KryptonRibbonGroupItem>? ParentItems
        {
            get
            {
                switch (_ribbonThemeComboBox.RibbonContainer)
                {
                    case KryptonRibbonGroupTriple triple:
                        return triple.Items;
                    case KryptonRibbonGroupLines lines:
                        return lines.Items;
                    default:
                        // Should never happen!
                        Debug.Assert(false);
                        return null;
                }
            }
        }
        #endregion
    }
}