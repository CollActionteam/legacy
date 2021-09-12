import Document, { Html, Head, Main, NextScript } from "next/document";
import { GA_TRACKING_ID } from "../lib/constants";

export default class MyDocument extends Document {
  render() {
    return (
      <Html>
        <Head>
          <meta property="og:title" content="CollAction" />
          <meta property="og:type" content="website" />
          <meta
            property="og:description"
            content="Do you want to make the world a better place, but do you sometimes feel that your actions are only a drop in the ocean? Well, not anymore, because we are introducing crowdacting: taking action knowing that you are one of many. We revamp the neighbourhood with a hundred people or switch to renewable energy with thousands at the same time. Find out more about crowdacting, take action collectively with one of the existing crowdactions, or start your own crowdaction."
          />
          <meta
            property="og:image"
            content="/public/android-chrome-256x256.png"
          />

          <meta
            name="description"
            content="Do you want to make the world a better place, but do you sometimes feel that your actions are only a drop in the ocean? Well, not anymore, because we are introducing crowdacting: taking action knowing that you are one of many. We revamp the neighbourhood with a hundred people or switch to renewable energy with thousands at the same time. Find out more about crowdacting, take action collectively with one of the existing crowdactions, or start your own crowdaction."
          />

          {/* Global Site Tag (gtag.js) - Google Analytics */}
          <script
            async
            src={`https://www.googletagmanager.com/gtag/js?id=${GA_TRACKING_ID}`}
          />
          <script
            dangerouslySetInnerHTML={{
              __html: `
                window.dataLayer = window.dataLayer || [];
                function gtag(){dataLayer.push(arguments);}
                gtag('js', new Date());
                gtag('config', '${GA_TRACKING_ID}', {
                  page_path: window.location.pathname,
                });
              `,
            }}
          />
        </Head>

        <body>
          <Main />
          <NextScript />
        </body>
      </Html>
    );
  }
}
