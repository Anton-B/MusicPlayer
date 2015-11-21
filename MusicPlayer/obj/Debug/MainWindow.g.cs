﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D7E495EAA7C420AD729A25FF43A8F028"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MusicPlayer;
using MusicPlayerAPI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MusicPlayer {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MusicPlayer.MainWindow mainWindow;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel mainDockPanel;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel playerDockPanel;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel infoStackPanel;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelArtist;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelTitle;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel controlsDockPanel;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btPrev;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btPlayPause;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btNext;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btRand;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btRepeat;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider volumeSlider;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelMaxTimelinePos;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSlash;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelCurrTimelinePos;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider musicTimelineSlider;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MediaElement mediaElement;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl tabControl;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabItem compTabItem;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid songsDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MusicPlayer;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.mainWindow = ((MusicPlayer.MainWindow)(target));
            return;
            case 2:
            this.mainDockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 3:
            this.playerDockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 4:
            this.infoStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            this.labelArtist = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.labelTitle = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.controlsDockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 8:
            this.btPrev = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\MainWindow.xaml"
            this.btPrev.Click += new System.Windows.RoutedEventHandler(this.btPrev_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btPlayPause = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\MainWindow.xaml"
            this.btPlayPause.Click += new System.Windows.RoutedEventHandler(this.btPlayPause_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btNext = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\MainWindow.xaml"
            this.btNext.Click += new System.Windows.RoutedEventHandler(this.btNext_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btRand = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\MainWindow.xaml"
            this.btRand.Click += new System.Windows.RoutedEventHandler(this.btRand_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btRepeat = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\MainWindow.xaml"
            this.btRepeat.Click += new System.Windows.RoutedEventHandler(this.btRepeat_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.volumeSlider = ((System.Windows.Controls.Slider)(target));
            return;
            case 14:
            this.labelMaxTimelinePos = ((System.Windows.Controls.Label)(target));
            return;
            case 15:
            this.labelSlash = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.labelCurrTimelinePos = ((System.Windows.Controls.Label)(target));
            return;
            case 17:
            this.musicTimelineSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 32 "..\..\MainWindow.xaml"
            this.musicTimelineSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.SetMediaPosition);
            
            #line default
            #line hidden
            return;
            case 18:
            this.mediaElement = ((System.Windows.Controls.MediaElement)(target));
            
            #line 36 "..\..\MainWindow.xaml"
            this.mediaElement.MediaOpened += new System.Windows.RoutedEventHandler(this.MediaElement_MediaOpened);
            
            #line default
            #line hidden
            return;
            case 19:
            this.tabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 20:
            this.compTabItem = ((System.Windows.Controls.TabItem)(target));
            return;
            case 21:
            this.songsDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 45 "..\..\MainWindow.xaml"
            this.songsDataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.songsDataGrid_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

