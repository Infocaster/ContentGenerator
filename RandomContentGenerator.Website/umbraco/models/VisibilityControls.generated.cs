//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v13.0.3+a0f3c15
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	// Mixin Content Type with alias "visibilityControls"
	/// <summary>Visibility Controls</summary>
	public partial interface IVisibilityControls : IPublishedContent
	{
		/// <summary>Hide From Top Navigation</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		bool HideFromTopNavigation { get; }

		/// <summary>Hide From XML Sitemap</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		bool HideFromXmlsitemap { get; }

		/// <summary>Hide From Search</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		bool UmbracoNaviHide { get; }
	}

	/// <summary>Visibility Controls</summary>
	[PublishedModel("visibilityControls")]
	public partial class VisibilityControls : PublishedContentModel, IVisibilityControls
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		public new const string ModelTypeAlias = "visibilityControls";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<VisibilityControls, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public VisibilityControls(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Hide From Top Navigation
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		[ImplementPropertyType("hideFromTopNavigation")]
		public virtual bool HideFromTopNavigation => GetHideFromTopNavigation(this, _publishedValueFallback);

		/// <summary>Static getter for Hide From Top Navigation</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		public static bool GetHideFromTopNavigation(IVisibilityControls that, IPublishedValueFallback publishedValueFallback) => that.Value<bool>(publishedValueFallback, "hideFromTopNavigation");

		///<summary>
		/// Hide From XML Sitemap: Tick this if you want to hide this page from the XML sitemap
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		[ImplementPropertyType("hideFromXMLSitemap")]
		public virtual bool HideFromXmlsitemap => GetHideFromXmlsitemap(this, _publishedValueFallback);

		/// <summary>Static getter for Hide From XML Sitemap</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		public static bool GetHideFromXmlsitemap(IVisibilityControls that, IPublishedValueFallback publishedValueFallback) => that.Value<bool>(publishedValueFallback, "hideFromXMLSitemap");

		///<summary>
		/// Hide From Search: Tick this box if you want to hide this page from the navigation and from search results
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		[ImplementPropertyType("umbracoNaviHide")]
		public virtual bool UmbracoNaviHide => GetUmbracoNaviHide(this, _publishedValueFallback);

		/// <summary>Static getter for Hide From Search</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "13.0.3+a0f3c15")]
		public static bool GetUmbracoNaviHide(IVisibilityControls that, IPublishedValueFallback publishedValueFallback) => that.Value<bool>(publishedValueFallback, "umbracoNaviHide");
	}
}
