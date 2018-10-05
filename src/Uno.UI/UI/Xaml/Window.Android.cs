using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Util;
using Android.Views;
using Uno.UI;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Windows.UI.Xaml
{
	public sealed partial class Window
	{
		private static Window _current;
		private UIElement _content;

		public Window()
		{
			Dispatcher = CoreDispatcher.Main;
			CoreWindow = new CoreWindow();
		}

		private void InternalActivate()
		{

		}

		private void InternalSetContent(UIElement value)
		{
			_content = value;
			ApplicationActivity.Instance?.SetContentView(value);
		}

		private UIElement InternalGetContent()
		{
			return _content;
		}

		private static Window InternalGetCurrentWindow()
		{
			if (_current == null)
			{
				_current = new Window();
			}

			return _current;
		}

		internal void RaiseNativeSizeChanged(int screenWidth, int screenHeight)
		{
			var newBounds = ViewHelper.PhysicalToLogicalPixels(new Rect(0, 0, screenWidth, screenHeight));
			var statusBarHeight = GetLogicalStatusBarHeight();
			var navigationBarHeight = GetLogicalNavigationBarHeight();

			var newVisibleBounds = new Rect(
				x: newBounds.X,
				y: newBounds.Y + statusBarHeight,
				width: newBounds.Width,
				height: newBounds.Height - statusBarHeight - navigationBarHeight
			);

			global::System.Diagnostics.Debug.WriteLine($"\n\n[YGUE] RaiseNativeSizeChanged :\n\tnewBounds={newBounds}\n\tstatusBarHeight={statusBarHeight}\n\tnavigationBarHeight={navigationBarHeight}\n\tnewVisibleBounds={newVisibleBounds}\n\n");

			var applicationView = ApplicationView.GetForCurrentView();
			if (applicationView != null && applicationView.VisibleBounds != newVisibleBounds)
			{
				applicationView.SetCoreBounds(newBounds);
			}

			if (Bounds != newBounds)
			{
				Bounds = newBounds;

				RaiseSizeChanged(
					new WindowSizeChangedEventArgs(
						new Windows.Foundation.Size(Bounds.Width, Bounds.Height)
					)
				);
			}
		}

		private int GetLogicalStatusBarHeight()
		{
			int logicalStatusBarHeight = 0;

			var activity = ContextHelper.Current as Activity;
			var decorView = activity.Window.DecorView;
			var isStatusBarVisible = ((int)decorView.SystemUiVisibility & (int)SystemUiFlags.Fullscreen) == 0;

			if (isStatusBarVisible)
			{
				int resourceId = Android.Content.Res.Resources.System.GetIdentifier("status_bar_height", "dimen", "android");
				if (resourceId > 0)
				{
					logicalStatusBarHeight = (int)(Android.Content.Res.Resources.System.GetDimensionPixelSize(resourceId) / Android.App.Application.Context.Resources.DisplayMetrics.Density);
				}
			}

			return logicalStatusBarHeight;
		}

		private double GetLogicalNavigationBarHeight()
		{
			var softKeysBarHeight = 0d;

			var metrics = new DisplayMetrics();

			var activity = ContextHelper.Current as Activity;
			var defaultDisplay = activity.WindowManager.DefaultDisplay;

			defaultDisplay.GetMetrics(metrics);
			int usableHeight = metrics.HeightPixels;

			defaultDisplay.GetRealMetrics(metrics);
			int realHeight = metrics.HeightPixels;

			if (realHeight > usableHeight)
			{
				softKeysBarHeight = realHeight - usableHeight;
			}

			return ViewHelper.PhysicalToLogicalPixels(softKeysBarHeight);
		}
	}
}
