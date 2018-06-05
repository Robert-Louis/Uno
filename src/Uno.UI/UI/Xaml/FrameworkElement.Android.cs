﻿using Android.Views;
using Uno.Extensions;
using Uno.Logging;
using Uno.UI;
using Uno.UI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Windows.Foundation;
using Uno.UI.Services;
using Uno.Diagnostics.Eventing;

namespace Windows.UI.Xaml
{
	public partial class FrameworkElement
	{
		private Size? _lastLayoutSize;
		private Size _actualSize;

		internal Size ActualSize => _actualSize;

		public FrameworkElement()
		{
			Initialize();
		}

		partial void Initialize();

		partial void OnLoadedPartial()
		{
			// see StretchAffectsMeasure for details.
			this.SetValue(
				StretchAffectsMeasureProperty,
				!(Parent is DependencyObject),
				DependencyPropertyValuePrecedences.DefaultValue
			);
		}

		#region StretchAffectsMeasure DependencyProperty

		/// <summary>
		/// Indicates whether stretching (HorizontalAlignment.Stretch and VerticalAlignment.Stretch) should affect the measured size of the FrameworkElement.
		/// Only set on a FrameworkElement if the parent is a native view whose layouting relies on the values of MeasuredWidth and MeasuredHeight to account for stretching.
		/// Note that this doesn't take Margins into account.
		/// </summary>
		/// <remarks>
		/// The <see cref="DependencyPropertyValuePrecedences.DefaultValue"/> is updated at each <see cref="OnLoadedPartial"/> call, but may
		/// be overriden by an external called as <see cref="DependencyPropertyValuePrecedences.Local"/>.
		/// </remarks>
		public bool StretchAffectsMeasure
		{
			get { return (bool)GetValue(StretchAffectsMeasureProperty); }
			set { SetValue(StretchAffectsMeasureProperty, value); }
		}

		// Using a DependencyProperty as the backing store for StretchAffectsMeasure.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StretchAffectsMeasureProperty =
			DependencyProperty.Register("StretchAffectsMeasure", typeof(bool), typeof(FrameworkElement), new PropertyMetadata(false));

		#endregion

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			var availableSize = ViewHelper.LogicalSizeFromSpec(widthMeasureSpec, heightMeasureSpec);

			if (!double.IsNaN(Width) || !double.IsNaN(Height))
			{
				availableSize = new Size(
					double.IsNaN(Width) ? availableSize.Width : Width,
					double.IsNaN(Height) ? availableSize.Height : Height
				);
			}

			var measuredSize = _layouter.Measure(availableSize);

			if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
			{
				this.Log().DebugFormat(
					"[{0}/{1}] OnMeasure1({2}, {3}) (parent: {4}/{5})",
					GetType(),
					Name,
					measuredSize.Width,
					measuredSize.Height,
					ViewHelper.MeasureSpecGetSize(widthMeasureSpec),
					ViewHelper.MeasureSpecGetSize(heightMeasureSpec)
				);
			}

			measuredSize = measuredSize.LogicalToPhysicalPixels();

			if (StretchAffectsMeasure)
			{
				if (HorizontalAlignment == HorizontalAlignment.Stretch)
				{
					measuredSize.Width = ViewHelper.MeasureSpecGetSize(widthMeasureSpec);
				}

				if (VerticalAlignment == VerticalAlignment.Stretch)
				{
					measuredSize.Height = ViewHelper.MeasureSpecGetSize(heightMeasureSpec);
				}
			}

			// Report our final dimensions.
			SetMeasuredDimension(
				(int)measuredSize.Width,
				(int)measuredSize.Height
			);


			IFrameworkElementHelper.OnMeasureOverride(this);
		}

		protected override void OnLayoutCore(bool changed, int left, int top, int right, int bottom)
		{
			var newSize = new Size(right - left, bottom - top).PhysicalToLogicalPixels();

			base.OnLayoutCore(changed, left, top, right, bottom);

			if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
			{
				this.Log().DebugFormat(
					"[{0}/{1}] OnLayoutCore({2}, {3}, {4}, {5}) (parent: {5},{6})",
					GetType(),
					Name,
					left, top, right, bottom,
					MeasuredWidth,
					MeasuredHeight
				);
			}

			if (
				// If the layout has changed, but the final size has not, this is just a translation.
				// So unless there was a layout requested, we can skip arranging the children.
				(changed && _lastLayoutSize != newSize)

				// Even if nothing changed, but a layout was requested, arrang the children.
				|| IsLayoutRequested
			)
			{
				_lastLayoutSize = newSize;

				OnBeforeArrange();

				_layouter.Arrange(new Rect(0, 0, newSize.Width, newSize.Height));

				OnAfterArrange();
			}

			var previousSize = _actualSize;
			_actualSize = newSize;
			RenderSize = _actualSize;

			if (previousSize != newSize)
			{
				SizeChanged?.Invoke(this, new SizeChangedEventArgs(previousSize, newSize));
				RenderTransform?.OnViewSizeChanged(previousSize, newSize);
			}
		}

		/// <summary>
		/// Provides an implementation <see cref="ViewGroup.Layout(int, int, int, int)"/> in order 
		/// to avoid the back and forth between Java and C#.
		/// </summary>
		internal void FastLayout(bool changed, int left, int top, int right, int bottom)
		{
			try
			{
				// Flag the native UnoViewGroup so it does not call OnLayoutCore because we're
				// calling it from the Uno side.
				NativeStartLayoutOverride(left, top, right, bottom);

				// Invoke our own layouting without going back and fort with Java.
				OnLayoutCore(changed, left, top, right, bottom);
			}
			finally
			{
				// Invoke the real layout method.
				NativeFinishLayoutOverride();
			}
		}

		protected override bool NativeRequestLayout()
		{
			if (!base.NativeRequestLayout())
			{
				return false;
			}

			if (_trace.IsEnabled && !IsLayoutRequested)
			{
				_trace.WriteEvent(
					FrameworkElement.TraceProvider.FrameworkElement_InvalidateMeasure,
					EventOpcode.Send,
					new[] {
						GetType().ToString(),
						this.GetDependencyObjectId().ToString()
					}
				);
			}

			if (!ShouldPropagateLayoutRequest())
			{
				//This view and the visual tree above it won't change size. Send the view to the LayoutManager to be remeasured and rearranged.
				if (!IsLayoutRequested)
				{
					LayoutManager.InvalidateArrange(this);
					// Call ForceLayout, otherwise View.measure() & View.layout() won't do anything
					this.ForceLayout();
				}
				return false;
			}

			_constraintsChanged = false;
			return true;
		}

		private bool IsTopLevelXamlView()
		{
			IViewParent parent = this;
			while (parent != null)
			{
				parent = parent.Parent;
				if (parent is IFrameworkElement)
				{
					return false;
				}
			}
			// If Parent == null, this is probably a not-yet-attached/already-detached view rather than the top-level view
			return (this as IViewParent).Parent != null;
		}

		/// <summary>
		/// Called before Arrange is called, this method will be deprecated
		/// once OnMeasure/OnArrange will be implemented completely
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnBeforeArrange()
		{

		}

		/// <summary>
		/// Called after Arrange is called, this method will be deprecated
		/// once OnMeasure/OnArrange will be implemented completely
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnAfterArrange()
		{

		}
	}
}