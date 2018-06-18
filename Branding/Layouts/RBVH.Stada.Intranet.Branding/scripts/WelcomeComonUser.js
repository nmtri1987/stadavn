var WelcomeCommonUser = {
    IsCommonUser: false,
    SharePointWelcomeMenuBoxId: "welcomeMenuBox",
    CommonWelcomeMenuBoxId: "DivCommonUserWelcome",
    displayWelcome: function () {
        if (WelcomeCommonUser.IsCommonUser === true) {
            $("#" + WelcomeCommonUser.SharePointWelcomeMenuBoxId).hide();
            $("#" + WelcomeCommonUser.DivCommonUserWelcome).show();
        } else {
            $("#" + WelcomeCommonUser.SharePointWelcomeMenuBoxId).show();
            $("#" + WelcomeCommonUser.DivCommonUserWelcome).hide();
        }
    }
};