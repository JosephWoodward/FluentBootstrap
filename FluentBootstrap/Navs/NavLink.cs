﻿using FluentBootstrap.Badges;
using FluentBootstrap.Html;
using FluentBootstrap.Links;
using FluentBootstrap.Navbars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBootstrap.Navs
{
    public interface INavLinkCreator<TModel> : IComponentCreator<TModel>
    {
    }

    public class NavLinkWrapper<TModel> : TagWrapper<TModel>,
        IBadgeCreator<TModel>
    {
    }

    internal interface INavLink : ITag
    {
    }

    public abstract class NavLink<TModel, TThis, TWrapper> : Tag<TModel, TThis, TWrapper>, IHasLinkExtensions, IHasTextContent
        where TThis : NavLink<TModel, TThis, TWrapper>
        where TWrapper : NavLinkWrapper<TModel>, new()
    {
        internal bool Active { get; set; }
        internal bool Disabled { get; set; }
        private Element<TModel> _listItem = null;

        protected NavLink(IComponentCreator<TModel> creator)
            : base(creator, "a")
        {
        }

        protected override void OnStart(TextWriter writer)
        {
            // Check if we're in a navbar, and if so, make sure we're in a navbar nav
            if (GetComponent<INavbar>() != null && GetComponent<INavbarNav>() == null)
            {
                new NavbarNav<TModel>(Helper).Start(writer);
            }

            // Create the list item wrapper
            _listItem = new Element<TModel>(Helper, "li");
            if (Active)
            {
                _listItem.AddCss(Css.Active);
            }
            if (Disabled)
            {
                _listItem.AddCss(Css.Disabled);
            }
            _listItem.Start(writer);

            base.OnStart(writer);
        }

        protected override void OnFinish(TextWriter writer)
        {
            base.OnFinish(writer);

            _listItem.Finish(writer);
        }
    }
}