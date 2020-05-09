import React from "react";
import { I18nextProvider } from "react-i18next";
import i18n from "../i18n";
import UserProvider from "./UserProvider";
import ConsentProvider from "./ConsentProvider";
import SettingsProvider from "./SettingsProvider";
import GraphQLProvider from "./GraphQLProvider";
import AnalyticsProvider from "./AnalyticsProvider";

export default ({ children }: any) => {
    return <I18nextProvider i18n={i18n}>
        <GraphQLProvider>
            <SettingsProvider>
                <ConsentProvider>
                    <UserProvider>
                        <AnalyticsProvider>
                            { children }
                        </AnalyticsProvider>
                    </UserProvider>
                </ConsentProvider>
            </SettingsProvider>
        </GraphQLProvider>
    </I18nextProvider>;
};