import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import common_en from "./translations/en/common.json";
import common_nl from "./translations/nl/common.json";
import matter from 'gray-matter';

type MarkdownJSON = {
  content: string, 
  data: {
    type? : string,
    title? : string
  }
}

// START EXAMPLE
//This is an example of how we may combine Markdown files with i18n translation JSON
//Unfortunately, this does not work since async/sync mismatch between this file loading and i18n initialization
let en: any = Object.assign({}, common_en);
let nl: any = Object.assign({}, common_nl);

// Eventually we would need to require multiple files instead of this single one
const testfile = require('./translations/en/markdown-test.md');

fetch(testfile)
  .then(r => r.text())
  .then(r => {
    // Use library to transform the Markdown file to a JSON object
    const json : MarkdownJSON = matter(r);
    // Add transformed content to translations JSON
    if(json.data.type) {
      en[json.data.type] = json
    }
  })
  .catch(e => console.error(e));
// END EXAMPLE

// Initialize i18n
i18n
  .use(initReactI18next)
  .init({
    resources: {
        en: {
            translations: en
        },
        nl: {
            translations: nl
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
