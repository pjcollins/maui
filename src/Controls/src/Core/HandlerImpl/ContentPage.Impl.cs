﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
	// TODO: We don't currently have any concept of a page in Maui
	// so this just treats it as a layout for now
	public partial class ContentPage : Microsoft.Maui.ILayout
	{
		IReadOnlyList<Microsoft.Maui.IView> Microsoft.Maui.ILayout.Children =>
			new List<IView>() { Content };

		ILayoutHandler Maui.ILayout.LayoutHandler => Handler as ILayoutHandler;

		Thickness Maui.IView.Margin => new Thickness();

		public Primitives.LayoutAlignment HorizontalLayoutAlignment => Primitives.LayoutAlignment.Fill;

		void Maui.ILayout.Add(IView child)
		{
			Content = (View)child;
		}

		void Maui.ILayout.Remove(IView child)
		{
			Content = null;
		}

		internal override void InvalidateMeasureInternal(InvalidationTrigger trigger)
		{
			IsArrangeValid = false;
			base.InvalidateMeasureInternal(trigger);
		}

		protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			var width = widthConstraint;
			var height = heightConstraint;

#if WINDOWS
			if (double.IsInfinity(width))
			{
				width = 800;
			}

			if (double.IsInfinity(height))
			{
				height = 800;
			}
#endif

			IsMeasureValid = true;
			return new Size(width, height);
		}

		protected override void ArrangeOverride(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			IsArrangeValid = true;
			IsMeasureValid = true;
			Arrange(bounds);
			Handler?.SetFrame(Frame);

			if (Content is IFrameworkElement fe)
			{
				fe.InvalidateArrange();
				fe.Measure(Frame.Width, Frame.Height);
				fe.Arrange(Frame);
			}

			if (Content is Layout layout)
				layout.ResolveLayoutChanges();

		}


	}
}
