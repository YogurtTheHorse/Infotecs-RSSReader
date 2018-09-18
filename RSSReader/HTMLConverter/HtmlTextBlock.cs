using HTMLConverter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace RSSReader
{
	/// <summary>
	/// Simple text block that can get HTML as input
	/// </summary>
	public class HtmlTextBlock : TextBlock
	{
		/// <summary>
		/// HTML string to render
		/// </summary>
		public string HtmlText
		{
			get => GetValue(HtmlTextProperty).ToString();
			set => SetValue(HtmlTextProperty, value);
		}

		/// <summary>
		/// Sets should HtmlTextBlock parse html or act as usual TextBlock
		/// </summary>
		public bool ParseHtml
		{
			get => Convert.ToBoolean(GetValue(ParseHtmlProperty));
			set => SetValue(ParseHtmlProperty, value);
		}

		public static readonly DependencyProperty HtmlTextProperty =
			DependencyProperty.Register("HtmlText", typeof(string), typeof(HtmlTextBlock), new UIPropertyMetadata(null, OnHtmlPropertyChanged));
		public static readonly DependencyProperty ParseHtmlProperty =
			DependencyProperty.Register("ParseHtml", typeof(bool), typeof(HtmlTextBlock), new UIPropertyMetadata(true, OnHtmlPropertyChanged));

		private static void OnHtmlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			HtmlTextBlock textBlock = sender as HtmlTextBlock;
			string text = textBlock.HtmlText ?? string.Empty;

			if (!textBlock.ParseHtml)
			{
				textBlock.Text = text;
				return;
			}

			string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text, true);
			FlowDocument flowDocument = null;

			try
			{
				flowDocument = XamlReader.Parse(xaml) as FlowDocument;
			}
			catch
			{
				textBlock.Text = text;
				return;
			}

			HyperlinksSubscriptions(flowDocument);

			textBlock.Inlines.Clear();
			List<Inline> inlines = new List<Inline>();

			foreach (Block block in flowDocument.Blocks)
			{
				if (block is Paragraph paragraph)
				{
					inlines.AddRange(paragraph.Inlines);
				}
			}

			textBlock.Inlines.AddRange(inlines);
		}

		private static void HyperlinksSubscriptions(FlowDocument flowDocument)
		{
			if (flowDocument == null) return;
			GetVisualChildren(flowDocument).OfType<Hyperlink>().ToList()
					 .ForEach(i => i.RequestNavigate += HyperlinkNavigate);
		}

		private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject root)
		{
			foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
			{
				yield return child;
				foreach (var descendants in GetVisualChildren(child)) yield return descendants;
			}
		}

		private static void HyperlinkNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.ToString());
			e.Handled = true;
		}
	}
}
