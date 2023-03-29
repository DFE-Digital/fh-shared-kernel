// js components were originally snaffled from https://github.com/alphagov/govuk-design-system/blob/main/src/javascripts/components/

//todo: swap to a components folder?

declare global {
    interface Window {
        GDS_CONSENT_COOKIE_VERSION: number;
        GA_MEASUREMENT_ID: string;
        GA_CONTAINER_ID: string;
        GA_COOKIE_NAME: string;
        dataLayer: any[];
        FamilyHubsFrontend: any;
    }
}

import CookieBanner from './components/cookie-banner'
import CookiesPage from './components/cookies-page'
import initAnalytics from './components/analytics';
import { nodeListForEach } from './components/helpers';

//todo: consistency in module/proto/class style

window.FamilyHubsFrontend = window.FamilyHubsFrontend || {};
window.FamilyHubsFrontend.initAll = () => {

    // Initialise cookie banner
    const $cookieBanner = document.querySelector('[data-module="govuk-cookie-banner"]') as HTMLElement | null;
    new CookieBanner($cookieBanner).init();

    initAnalytics(window.GA_MEASUREMENT_ID);

    //todo: move this into scripts section on cookie page
    // Initialise cookie page
    var $cookiesPage = document.querySelector('[data-module="app-cookies-page"]')
    new CookiesPage($cookiesPage).init()

    //todo: move into module
    const backLinks = document.querySelectorAll(".app-back-link");
    nodeListForEach(backLinks, (link: HTMLAnchorElement) => {
        link.addEventListener("click", () => {
            window.history.back();
        });
    });
};
