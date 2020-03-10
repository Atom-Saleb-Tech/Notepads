﻿namespace Notepads.Services
{
    using System.Threading.Tasks;
    using Microsoft.Gaming.XboxGameBar;
    using Notepads.Controls.Settings;
    using Notepads.Utilities;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public static class ActivationService
    {
        public static async Task ActivateAsync(Frame rootFrame, IActivatedEventArgs e)
        {
            if (e is ProtocolActivatedEventArgs protocolActivatedEventArgs)
            {
                ProtocolActivated(rootFrame, protocolActivatedEventArgs);
            }
            else if (e is FileActivatedEventArgs fileActivatedEventArgs)
            {
                await FileActivated(rootFrame, fileActivatedEventArgs);
            }
            else if (e is CommandLineActivatedEventArgs commandLineActivatedEventArgs)
            {
                await CommandActivated(rootFrame, commandLineActivatedEventArgs);
            }
            else if (e is LaunchActivatedEventArgs launchActivatedEventArgs)
            {
                LaunchActivated(rootFrame, launchActivatedEventArgs);
            }
            else if (e is XboxGameBarWidgetActivatedEventArgs xboxGameBarWidgetActivatedEventArgs)
            {
                GameBarActivated(rootFrame, xboxGameBarWidgetActivatedEventArgs);
            }
            else // For other types of activated events
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(NotepadsMainPage));
                }
            }
        }

        public static void GameBarActivated(Frame rootFrame, XboxGameBarWidgetActivatedEventArgs xboxGameBarWidgetActivatedEventArgs)
        {
            LoggingService.LogInfo($"[XboxGameBarWidgetActivated] AppExtensionId: {xboxGameBarWidgetActivatedEventArgs.AppExtensionId}");

            if (xboxGameBarWidgetActivatedEventArgs != null)
            {
                if (xboxGameBarWidgetActivatedEventArgs.IsLaunchActivation)
                {
                    App.IsFirstInstance = true;

                    var xboxGameBarWidget = new XboxGameBarWidget(
                        xboxGameBarWidgetActivatedEventArgs,
                        Window.Current.CoreWindow,
                        rootFrame);

                    if (xboxGameBarWidgetActivatedEventArgs.AppExtensionId == "Notepads")
                    {
                        rootFrame.Navigate(typeof(NotepadsMainPage), xboxGameBarWidget);
                    }
                    else if (xboxGameBarWidgetActivatedEventArgs.AppExtensionId == "NotepadsSettings")
                    {
                        rootFrame.Navigate(typeof(SettingsPage), xboxGameBarWidget);
                    }
                }
            }
        }

        private static void ProtocolActivated(Frame rootFrame, ProtocolActivatedEventArgs protocolActivatedEventArgs)
        {
            LoggingService.LogInfo($"[ProtocolActivated] Protocol: {protocolActivatedEventArgs.Uri}");

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(NotepadsMainPage), protocolActivatedEventArgs);
            }
            else if (rootFrame.Content is NotepadsMainPage mainPage)
            {
                mainPage.ExecuteProtocol(protocolActivatedEventArgs.Uri);
            }
        }

        private static void LaunchActivated(Frame rootFrame, LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            if (launchActivatedEventArgs.PrelaunchActivated == false)
            {
                // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                Windows.ApplicationModel.Core.CoreApplication.EnablePrelaunch(true);
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(NotepadsMainPage), launchActivatedEventArgs.Arguments);
            }
        }

        private static async Task FileActivated(Frame rootFrame, FileActivatedEventArgs fileActivatedEventArgs)
        {
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(NotepadsMainPage), fileActivatedEventArgs);
            }
            else if (rootFrame.Content is NotepadsMainPage mainPage)
            {
                await mainPage.OpenFiles(fileActivatedEventArgs.Files);
            }
        }

        private static async Task CommandActivated(Frame rootFrame, CommandLineActivatedEventArgs commandLineActivatedEventArgs)
        {
            LoggingService.LogInfo($"[CommandActivated] CurrentDirectoryPath: {commandLineActivatedEventArgs.Operation.CurrentDirectoryPath} " +
                                   $"Arguments: {commandLineActivatedEventArgs.Operation.Arguments}");

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(NotepadsMainPage), commandLineActivatedEventArgs);
            }
            else if (rootFrame.Content is NotepadsMainPage mainPage)
            {
                var file = await FileSystemUtility.OpenFileFromCommandLine(
                    commandLineActivatedEventArgs.Operation.CurrentDirectoryPath,
                    commandLineActivatedEventArgs.Operation.Arguments);

                if (file != null)
                {
                    await mainPage.OpenFile(file);
                }
            }
        }
    }
}