#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY Daniel Vaughan ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL Daniel Vaughan BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-11-20 13:28:27Z</CreationDate>
	<Origin>http://www.calciumsdk.com</Origin>
</File>
*/
#endregion

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DanielVaughan.Windows
{
	public class ProgressIndicatorProxy : FrameworkElement
	{
		bool loaded;

		public ProgressIndicatorProxy()
		{
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (loaded)
			{
				return;
			}

			Attach();

			loaded = true;
		}

		public void Attach()
		{
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}

			var page = this.GetVisualAncestors<PhoneApplicationPage>().First();

			var progressIndicator = SystemTray.ProgressIndicator;
			if (progressIndicator != null)
			{
				return;
			}

			progressIndicator = new ProgressIndicator();

			SystemTray.SetProgressIndicator(page, progressIndicator);

			Binding binding = new Binding("IsIndeterminate") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

			binding = new Binding("IsVisible") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

			binding = new Binding("Text") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.TextProperty, binding);

			binding = new Binding("Value") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.ValueProperty, binding);
		}

		#region IsIndeterminate

		public static readonly DependencyProperty IsIndeterminateProperty
			= DependencyProperty.RegisterAttached(
				"IsIndeterminate",
				typeof(bool),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(false));

		public bool IsIndeterminate
		{
			get
			{
				return (bool)GetValue(IsIndeterminateProperty);
			}
			set
			{
				SetValue(IsIndeterminateProperty, value);
			}
		}

		#endregion

		#region IsVisible

		public static readonly DependencyProperty IsVisibleProperty
			= DependencyProperty.RegisterAttached(
				"IsVisible",
				typeof(bool),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(true));

		public bool IsVisible
		{
			get
			{
				return (bool)GetValue(IsVisibleProperty);
			}
			set
			{
				SetValue(IsVisibleProperty, value);
			}
		}

		#endregion

		#region Text

		public static readonly DependencyProperty TextProperty
			= DependencyProperty.RegisterAttached(
				"Text",
				typeof(string),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(string.Empty));

		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		#endregion

		#region Value

		public static readonly DependencyProperty ValueProperty
			= DependencyProperty.RegisterAttached(
				"Value",
				typeof(double),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(0.0));

		public double Value
		{
			get
			{
				return (double)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		#endregion
	}
}
