﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace APKInstaller.Controls
{
    /// <summary>
    /// This is the base control to create consistent settings experiences, inline with the Windows 11 design language.
    /// A Setting can also be hosted within a SettingExpander.
    /// </summary>
    public partial class Setting : ButtonBase
    {
        internal const string NormalState = "Normal";
        internal const string PointerOverState = "PointerOver";
        internal const string PressedState = "Pressed";
        internal const string DisabledState = "Disabled";

        internal const string HeaderIconPresenterHolder = "PART_HeaderIconPresenterHolder";

        /// <summary>
        /// Creates a new instance of the <see cref="Setting"/> class.
        /// </summary>
        public Setting()
        {
            DefaultStyleKey = typeof(Setting);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsEnabledChanged -= OnIsEnabledChanged;
            OnButtonIconChanged();
            OnHeaderChanged();
            OnHeaderIconChanged();
            OnDescriptionChanged();
            OnIsClickEnabledChanged();
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
            RegisterAutomation();
            IsEnabledChanged += OnIsEnabledChanged;
        }

        private void RegisterAutomation()
        {
            if (Header is string headerString && headerString != string.Empty)
            {
                AutomationProperties.SetName(this, headerString);
                // We don't want to override an AutomationProperties.Name that is manually set, or if the Content basetype is of type ButtonBase (the ButtonBase.Content will be used then)
                if (Content is UIElement element
                    && string.IsNullOrEmpty(AutomationProperties.GetName(element))
                    && element is not ButtonBase or TextBlock)
                {
                    AutomationProperties.SetName(element, headerString);
                }
            }
        }

        private void EnableButtonInteraction()
        {
            DisableButtonInteraction();

            PointerEntered += Control_PointerEntered;
            PointerExited += Control_PointerExited;
            PreviewKeyDown += Control_PreviewKeyDown;
            PreviewKeyUp += Control_PreviewKeyUp;
        }

        private void DisableButtonInteraction()
        {
            PointerEntered -= Control_PointerEntered;
            PointerExited -= Control_PointerExited;
            PreviewKeyDown -= Control_PreviewKeyDown;
            PreviewKeyUp -= Control_PreviewKeyUp;
        }

        private void Control_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
            {
                VisualStateManager.GoToState(this, NormalState, true);
            }
        }

        private void Control_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
            {
                VisualStateManager.GoToState(this, PressedState, true);
            }
        }

        public void Control_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }

        public void Control_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            VisualStateManager.GoToState(this, PointerOverState, true);
        }
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            //  e.Handled = true;
            if (IsClickEnabled)
            {
                base.OnPointerPressed(e);
                VisualStateManager.GoToState(this, PressedState, true);
            }
        }
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (IsClickEnabled)
            {
                base.OnPointerReleased(e);
                VisualStateManager.GoToState(this, NormalState, true);
            }
        }

        /// <summary>
        /// Creates AutomationPeer
        /// </summary>
        /// <returns>An automation peer for <see cref="Setting"/>.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SettingAutomationPeer(this);
        }

        private void OnIsClickEnabledChanged()
        {
            OnButtonIconChanged();
            if (IsClickEnabled)
            {
                EnableButtonInteraction();
            }
            else
            {
                DisableButtonInteraction();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
        }


        public void OnButtonIconChanged()
        {
            _ = VisualStateManager.GoToState(this, IsClickEnabled && ActionIcon != null ? "ActionIconVisible" : "ActionIconCollapsed", false);
        }

        public void OnHeaderIconChanged()
        {
            if (GetTemplateChild(HeaderIconPresenterHolder) is FrameworkElement headerIconPresenter)
            {
                headerIconPresenter.Visibility = Icon != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public void OnDescriptionChanged()
        {
            _ = VisualStateManager.GoToState(this, Description == null ? "DescriptionCollapsed" : "DescriptionVisible", false);
        }

        public void OnHeaderChanged()
        {
            _ = VisualStateManager.GoToState(this, Header == null ? "HeaderCollapsed" : "HeaderVisible", false);
        }
    }
}
