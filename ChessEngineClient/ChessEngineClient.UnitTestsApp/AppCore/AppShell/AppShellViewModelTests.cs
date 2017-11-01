using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChessEngineClient.UnitTests
{
    public class AppShellViewModelTests
    {
        [Fact]
        public void AppShellViewModel_CurrentPageChanged_CallsOnCurrentPageChanged()
        {
        }

        [Fact]
        public void OnCurrentPageChanged_OnNavItemKey_SetsCurrentNavItem()
        {
        }

        [Fact]
        public void OnCurrentPageChanged_OnNavItemKey_NullsCurrentSecondaryNavItem()
        {
        }

        [Fact]
        public void OnCurrentPageChanged_OnSecondaryNavItemKey_SetsCurrentSecondaryNavItem()
        {
        }

        [Fact]
        public void OnCurrentPageChanged_OnNavItemKey_NullsCurrnetNavItem()
        {
        }

        [Fact]
        public void CurrentNavItem_WhenSet_CallsOnCurrentNavItemChanged()
        {
        }

        [Fact]
        public void CurrentSecondaryNavItem_WhenSet_CallsOnCurrentNavItemChanged()
        {
        }

        [Fact]
        public void OnCurrentNavItemChanged_OnDifferentPageKey_CallsNavigateTo()
        {
        }

        [Fact]
        public void OnCurrentNavItemChanged_OnSamePageKey_DoesntCallNavigateTo()
        {
        }
    }
}
