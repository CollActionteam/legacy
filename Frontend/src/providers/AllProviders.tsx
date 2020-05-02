import React from "react";
import { I18nextProvider } from "react-i18next";
import i18n from "../i18n";
import UserProvider from "./UserProvider";
import CookieConsentProvider from "./CookieConsentProvider";
import SettingsProvider from "./SettingsProvider";
import GraphQLProvider from "./GraphQLProvider";
import AnalyticsProvider from "./AnalyticsProvider";

export default ({ children }: any) => {
    return <React.StrictMode>
        <I18nextProvider i18n={i18n}>
            <GraphQLProvider>
                <SettingsProvider>
                    <CookieConsentProvider>
                        <UserProvider>
                            <AnalyticsProvider>
                                { children }
                            </AnalyticsProvider>
                        </UserProvider>
                    </CookieConsentProvider>
                </SettingsProvider>
            </GraphQLProvider>
        </I18nextProvider>
    </React.StrictMode>;
};