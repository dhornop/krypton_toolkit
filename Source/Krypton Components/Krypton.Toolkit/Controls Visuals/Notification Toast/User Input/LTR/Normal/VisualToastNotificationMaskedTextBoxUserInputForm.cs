﻿#region BSD License
/*
 *
 *  New BSD 3-Clause License (https://github.com/Krypton-Suite/Standard-Toolkit/blob/master/LICENSE)
 *  Modifications by Peter Wagner(aka Wagnerp) & Simon Coghlan(aka Smurf-IV), et al. 2024 - 2025. All rights reserved.
 *
 */
#endregion

using Timer = System.Windows.Forms.Timer;

namespace Krypton.Toolkit
{
    internal partial class VisualToastNotificationMaskedTextBoxUserInputForm : VisualToastNotificationBaseForm
    {
        #region Instance Fields

        private int _time;

        private Timer _timer;

        private readonly KryptonUserInputToastNotificationData _data;

        #endregion

        #region Internal

        internal string UserResponse => kmtxtUserInput.Text ?? string.Empty;

        #endregion

        #region Identity

        public VisualToastNotificationMaskedTextBoxUserInputForm(KryptonUserInputToastNotificationData data)
        {
            InitializeComponent();

            _data = data;

            GotFocus += (sender, args) => kmtxtUserInput.Focus();

            UpdateBorderColors();
        }

        #endregion

        #region Implementation

        private void UpdateBorderColors()
        {
            StateCommon!.Border.Color1 = _data.BorderColor1 ?? GlobalStaticValues.EMPTY_COLOR;

            StateCommon!.Border.Color2 = _data.BorderColor2 ?? GlobalStaticValues.EMPTY_COLOR;
        }

        private void UpdateText()
        {
            kwlNotificationTitle.Text = _data.NotificationTitle;

            kwlNotificationMessage.Text = _data.NotificationContent;
        }

        private void UpdateInitialValues()
        {
            // Set initial date and time values
            kmtxtUserInput.Text = GlobalStaticValues.DEFAULT_EMPTY_STRING;
        }

        private void SetIcon(Bitmap? image) => pbxNotificationIcon.Image = image;

        private void UpdateLocation()
        {
            //Once loaded, position the form, or position it to the bottom left of the screen with added padding
            Location = _data.NotificationLocation ?? new Point(Screen.PrimaryScreen!.WorkingArea.Width - Width - 5,
                Screen.PrimaryScreen.WorkingArea.Height - Height - 5);
        }

        private void UpdateIcon()
        {
            int scaledImageSize = _data.NotificationIconSize == ToastNotificationIconSize.Small
                ? (_data.CustomNotificationIconSize ?? 32)
                : (int)_data.NotificationIconSize!;

            switch (_data.NotificationIcon)
            {
                case KryptonToastNotificationIcon.None:
                    SetIcon(null);
                    break;
                case KryptonToastNotificationIcon.Hand:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Hand_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.SystemHand:
                    SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.Hand.ToBitmap(), scaledImageSize, scaledImageSize));
                    break;
                case KryptonToastNotificationIcon.Question:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Question_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.SystemQuestion:
                    SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.Question.ToBitmap(), scaledImageSize, scaledImageSize));
                    break;
                case KryptonToastNotificationIcon.Exclamation:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Warning_128_x_115, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.SystemExclamation:
                    SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.Exclamation.ToBitmap(), scaledImageSize, scaledImageSize));
                    break;
                case KryptonToastNotificationIcon.Asterisk:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Asterisk_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.SystemAsterisk:
                    SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.Asterisk.ToBitmap(), scaledImageSize, scaledImageSize));
                    break;
                case KryptonToastNotificationIcon.Stop:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Stop_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.Error:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Critical_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.Warning:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Warning_128_x_115, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.Information:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Information_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.Shield:
                    if (OSUtilities.IsAtLeastWindowsEleven)
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_UAC_Shield_Windows_11_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    }
                    else if (OSUtilities.IsWindowsTen)
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_UAC_Shield_Windows_10_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    }
                    else
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_UAC_Shield_Windows_7_and_8_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    }
                    break;
                case KryptonToastNotificationIcon.WindowsLogo:
                    if (OSUtilities.IsAtLeastWindowsEleven)
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(WindowsLogoImageResources.Windows_11_128_128, new Size(scaledImageSize, scaledImageSize)));
                    }
                    else if (OSUtilities.IsWindowsEight || OSUtilities.IsWindowsEightPointOne || OSUtilities.IsWindowsTen)
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(WindowsLogoImageResources.Windows_8_81_10_128_128, new Size(scaledImageSize, scaledImageSize)));
                    }
                    else
                    {
                        SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.WinLogo.ToBitmap(), new Size(scaledImageSize, scaledImageSize)));
                    }
                    break;
                case KryptonToastNotificationIcon.Application:
                    SetIcon(GraphicsExtensions.ScaleImage(_data.ApplicationIcon.ToBitmap(), new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.SystemApplication:
                    SetIcon(GraphicsExtensions.ScaleImage(SystemIcons.Asterisk.ToBitmap(), scaledImageSize, scaledImageSize));
                    break;
                case KryptonToastNotificationIcon.Ok:
                    SetIcon(GraphicsExtensions.ScaleImage(ToastNotificationImageResources.Toast_Notification_Ok_128_x_128, new Size(scaledImageSize, scaledImageSize)));
                    break;
                case KryptonToastNotificationIcon.Custom:
                    SetIcon(_data.CustomImage != null
                        ? GraphicsExtensions.ScaleImage(new Bitmap(_data.CustomImage), new Size(scaledImageSize, scaledImageSize))
                        : null);
                    break;
                case null:
                    SetIcon(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void VisualToastNotificationMaskedTextBoxUserInputForm_Load(object sender, EventArgs e)
        {
            UpdateIcon();

            UpdateLocation();

            ShowCloseButton();

            _timer.Start();
        }

        private void ShowCloseButton()
        {
            CloseBox = _data.ShowCloseBox ?? false;

            FormBorderStyle = CloseBox ? FormBorderStyle.Fixed3D : FormBorderStyle.FixedSingle;

            ControlBox = _data.ShowCloseBox ?? false;
        }

        private void itbDismiss_Click(object sender, EventArgs e) => Close();

        public new DialogResult ShowDialog()
        {
            TopMost = _data.TopMost ?? true;

            UpdateText();

            UpdateIcon();

            UpdateInitialValues();

            UpdateLocation();

            if (_data.CountDownSeconds != 0)
            {
                kbtnDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                itbDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                _timer = new Timer();

                _timer.Interval = _data.CountDownTimerInterval ?? 1000;

                _timer.Tick += (sender, args) =>
                {
                    _time++;

                    kbtnDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                    itbDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                    if (_time == _data.CountDownSeconds)
                    {
                        _timer.Stop();

                        Close();
                    }
                };
            }

            return base.ShowDialog();
        }

        public new DialogResult ShowDialog(IWin32Window owner)
        {
            TopMost = _data.TopMost ?? true;

            UpdateText();

            UpdateIcon();

            UpdateInitialValues();

            if (_data.CountDownSeconds != 0)
            {
                kbtnDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                itbDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                _timer = new Timer();

                _timer.Interval = _data.CountDownTimerInterval ?? 1000;

                _timer.Tick += (sender, args) =>
                {
                    _time++;

                    kbtnDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                    itbDismiss.Text = $@"{KryptonManager.Strings.ToastNotificationStrings.Dismiss} ({_data.CountDownSeconds - _time})";

                    if (_time == _data.CountDownSeconds)
                    {
                        _timer.Stop();

                        Close();
                    }
                };
            }

            return base.ShowDialog(owner);
        }

        internal static string ShowNotification(KryptonUserInputToastNotificationData data)
        {
            var owner = data.ToastHost ?? FromHandle(PI.GetActiveWindow());

            using var toast = new VisualToastNotificationMaskedTextBoxUserInputForm(data);

            if (owner != null)
            {
                toast.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;

                return toast.ShowDialog(owner!) == DialogResult.OK ? toast.UserResponse : GlobalStaticValues.DEFAULT_EMPTY_STRING;
            }
            else
            {
                return toast.ShowDialog() == DialogResult.OK ? toast.UserResponse : GlobalStaticValues.DEFAULT_EMPTY_STRING;
            }
        }

        #endregion
    }
}
