import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import common_en from "./translations/en/common.json";
import common_nl from "./translations/nl/common.json";

// Initialize i18n
i18n
  .use(initReactI18next)
  .init({
    resources: {
        en: {
            translations: common_en
        },
        nl: {
            translations: common_nl
        }
    },
    lng: 'en',
    ns: ["translations"],
    defaultNS: "translations",
    keySeparator: ".",
    interpolation: {
      escapeValue: false
    }
  });

export default i18n;
