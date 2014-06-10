﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FluentBootstrap
{
    public class Tag : Component
    {
        private readonly List<Component> _children = new List<Component>();

        internal TagBuilder TagBuilder { get; private set; }
        internal HashSet<string> CssClasses { get; private set; }

        protected internal Tag(BootstrapHelper helper, string tagName, params string[] cssClasses) : base(helper)
        {
            TagBuilder = new TagBuilder(tagName);
            CssClasses = new HashSet<string>();
            foreach (string cssClass in cssClasses)
                CssClasses.Add(cssClass);
        }

        internal void AddChild(Component component)
        {
            _children.Add(component);
        }

        internal void MergeAttributes(object attributes, bool replaceExisting = true)
        {
            if (attributes == null)
                return;
            MergeAttributes(System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(attributes), replaceExisting);
        }

        internal void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting = true)
        {
            if (attributes == null)
                return;
            foreach (KeyValuePair<TKey, TValue> attribute in attributes)
            {
                string key = Convert.ToString(attribute.Key, CultureInfo.InvariantCulture);
                string value = Convert.ToString(attribute.Value, CultureInfo.InvariantCulture);
                MergeAttribute(key, value, replaceExisting);
            }
        }

        // This works a little bit differently then the TagBuilder.MergeAttribute() method
        // This version does not throw on null or whitespace key and removes the attribute if value is null
        internal void MergeAttribute(string key, string value, bool replaceExisting = true)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (value == null && replaceExisting && TagBuilder.Attributes.ContainsKey(key))
            {
                TagBuilder.Attributes.Remove(key);
            }
            else if (value != null && (replaceExisting || !TagBuilder.Attributes.ContainsKey(key)))
            {
                TagBuilder.Attributes[key] = value;
            }
        }

        internal void ToggleCssClass(string cssClass, bool add, params string[] removeIfAdding)
        {
            if (add)
            {
                foreach (string remove in removeIfAdding)
                {
                    CssClasses.Remove(remove);
                }
                CssClasses.Add(cssClass);
            }
            else
            {
                CssClasses.Remove(cssClass);
            }
        }

        protected override void OnStart(TextWriter writer)
        {
            // Set CSS classes
            foreach (string cssClass in CssClasses)
            {
                TagBuilder.AddCssClass(cssClass);
            }

            // Append the start tag
            writer.Write(TagBuilder.ToString(TagRenderMode.StartTag));

            // Append any children
            foreach (Component child in _children)
            {
                child.Start(writer, false);
                child.Finish(writer);
            }
        }

        protected override void OnFinish(TextWriter writer)
        {
            // Append the end tag
            writer.Write(TagBuilder.ToString(TagRenderMode.EndTag));
        }
    }
}