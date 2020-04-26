import React, { useContext, useEffect } from "react";
import { useQuery } from "@apollo/client";
import { gql } from "@apollo/client";
import { ISettings } from "../api/types";

export const GET_SETTINGS = gql`
    query GetSettings {
        miscellaneous {  
            mailChimpNewsletterListId
            disqusSiteId
            stripePublicKey
            externalLoginProviders
            googleAnalyticsID
            facebookPixelID
        }
        categories: __type(name: "Category") {
            enumValues {
                name
            }
        }
        displayPriorities: __type(name: "ProjectDisplayPriority") {
            enumValues {
                name
            }
        }
        projectStatusses: __type(name: "ProjectStatus") {
            enumValues {
                name
            }
        }
    }
`; 

const defaultSettings: ISettings = {
    mailChimpNewsletterListId: "",
    stripePublicKey: "",
    disqusSiteId: "",
    googleAnalyticsID: "",
    facebookPixelID: "",
    externalLoginProviders: [],
    categories: [],
    displayPriorities: [],
    projectStatusses: []
};

export const SettingsContext = React.createContext(defaultSettings);

const mapSettings = (settingsData: any): ISettings => {
    if (!settingsData) {
        return defaultSettings;
    }

    const misc = settingsData.miscellaneous;
    return {
        mailChimpNewsletterListId: misc.mailChimpNewsletterListId,
        stripePublicKey: misc.stripePublicKey,
        disqusSiteId: misc.disqusSiteId,
        externalLoginProviders: misc.externalLoginProviders,
        googleAnalyticsID: misc.googleAnalyticsID,
        facebookPixelID: misc.facebookPixelID,
        categories: settingsData.categories.enumValues.map((v: any) => v.name),
        displayPriorities: settingsData.displayPriorities.enumValues.map((v: any) => v.name),
        projectStatusses: settingsData.projectStatusses.enumValues.map((v: any) => v.name)
    };
};

export default ({ children }: any) => {
    let { data, error } = useQuery(GET_SETTINGS);

    useEffect(() => {
        if (error) {
            console.error(error.message);
        }
    }, [error]);

    return <SettingsContext.Provider value={mapSettings(data)}>
        { children }
    </SettingsContext.Provider>;
};

export const useSettings = () => useContext(SettingsContext);