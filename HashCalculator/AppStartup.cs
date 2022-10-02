﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace HashCalculator
{
    internal class AppStartup : Application
    {
        [STAThread()]
        public static void Main()
        {
            AppDomain dm = AppDomain.CurrentDomain;
            dm.AssemblyResolve += AppAssemblyResolve;
            AppStartup app = new AppStartup();
            app.ApplicationRunAfterInitialized();
        }

        private static Assembly AppAssemblyResolve(
            object sender, ResolveEventArgs arg)
        {
            string asmbName = new AssemblyName(arg.Name).Name;
            if (!(asmbName == "BouncyCastle.Crypto"))
                return default;
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resName = "HashCalculator.Asmbs." + asmbName + ".dll";
            if (!(asmb.GetManifestResourceStream(resName) is Stream stream))
                return default;
            byte[] assemblyData = new byte[stream.Length];
            stream.Read(assemblyData, 0, assemblyData.Length);
            stream.Close();
            return Assembly.Load(assemblyData);
        }

        private void ApplicationRunAfterInitialized()
        {
            this.DispatcherUnhandledException += this.UnhandledException;
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            this.Resources = new ResourceDictionary();
            this.Resources.Source = new Uri("ResDict.xaml", UriKind.Relative);
            this.Run();
        }

        private void UnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string exceptionTitle;
            // 剪贴板无法打开(CLIPBRD_E_CANT_OPEN)错误代码：0x800401D0
            if ((uint)e.Exception.HResult == 0x800401D0)
            {
                e.Handled = true;
                exceptionTitle = "复制哈希值失败";
            }
            else
                exceptionTitle = "未捕获的未知异常";
            MessageBox.Show($"{exceptionTitle}：\n{e.Exception.Message}", "错误");
        }
    }
}