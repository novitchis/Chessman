using Chessman;
using Framework.MVVM;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class AppShellViewModelTests
{
    private static string TestPageName = "TestPageName";
    private static string TestSecondaryPageName = "TestSecondaryPageName";

    private AppShellViewModel GetViewModel(INavigationService fakeNavigationService)
    {
        AppShellViewModel vm = new AppShellViewModel(fakeNavigationService);
        vm.NavigationItems.Insert(0, new NavMenuItem(null, null, TestPageName));
        vm.SecondaryNavigationItems.Insert(0, new NavMenuItem(null, null, TestSecondaryPageName));

        return vm;
    }

    [Fact]
    public void CurrentPageChanged_OnNavItemKey_SetsCurrentNavItem()
    {
        Mock<INavigationService> mockNavigationService = new Mock<INavigationService>();
        AppShellViewModel vm = GetViewModel(mockNavigationService.Object);

        mockNavigationService.Setup(service => service.CurrentPageKey).Returns(TestPageName);
        mockNavigationService.Raise(service => service.CurrentPageChanged += null, EventArgs.Empty);

        Assert.Equal(vm.CurrentNavItem.PageNavigationName, TestPageName);
        Assert.Null(vm.CurrentSecondaryNavItem);
    }

    [Fact]
    public void CurrentPageChanged_WhenRaised_DoesntCallNavigateTo()
    {
        Mock<INavigationService> fakeNavigationService = new Mock<INavigationService>();
        AppShellViewModel vm = GetViewModel(fakeNavigationService.Object);

        fakeNavigationService.Setup(service => service.CurrentPageKey).Returns(TestPageName);
        fakeNavigationService.Raise(service => service.CurrentPageChanged += null, EventArgs.Empty);

        fakeNavigationService.Verify(service => service.NavigateTo(TestPageName), Times.Never());
    }

    [Fact]
    public void CurrentPageChanged_OnSecondaryNavItemKey_SetsCurrentSecondaryNavItem()
    {
        Mock<INavigationService> mockNavigationService = new Mock<INavigationService>();
        AppShellViewModel vm = GetViewModel(mockNavigationService.Object);

        mockNavigationService.Setup(service => service.CurrentPageKey).Returns(TestSecondaryPageName);
        mockNavigationService.Raise(service => service.CurrentPageChanged += null, EventArgs.Empty);

        Assert.Equal(vm.CurrentSecondaryNavItem.PageNavigationName, TestSecondaryPageName);
        Assert.Null(vm.CurrentNavItem);
    }

    [Fact]
    public void CurrentNavItem_WhenSet_CallsNavigateTo()
    {
        Mock<INavigationService> fakeNavigationService = new Mock<INavigationService>();
        AppShellViewModel vm = GetViewModel(fakeNavigationService.Object);

        vm.CurrentNavItem = vm.NavigationItems[0];
        
        fakeNavigationService.Verify(service => service.NavigateTo(vm.NavigationItems[0].PageNavigationName));
    }

    [Fact]
    public void CurrentSecondaryNavItem_WhenSet_CallsNavigateTo()
    {
        Mock<INavigationService> fakeNavigationService = new Mock<INavigationService>();
        AppShellViewModel vm = GetViewModel(fakeNavigationService.Object);

        vm.CurrentSecondaryNavItem = vm.SecondaryNavigationItems[0];

        fakeNavigationService.Verify(service => service.NavigateTo(vm.SecondaryNavigationItems[0].PageNavigationName));
    }
}
