import Document, { Html, Head, Main, NextScript } from "next/document";
import { GA_TRACKING_ID } from "../lib/constants";

export default class MyDocument extends Document {
  render() {
    const currentURL = "https://collaction.org";
    const description =
      "Do you want to make the world a better place, but do you sometimes feel that your actions are only a drop in the ocean? Well, not anymore, because we are introducing crowdacting: taking action knowing that you are one of many. We revamp the neighbourhood with a hundred people or switch to renewable energy with thousands at the same time. Find out more about crowdacting, take action collectively with one of the existing crowdactions, or start your own crowdaction.";
    const previewImage = "/android-chrome-256x256.png";
    const pageTitle = "CollAction";
    return (
      <Html>
        <Head>
          <meta property="og:url" content={currentURL} key="ogurl" />
          <meta property="og:image" content={previewImage} key="ogimage" />
          <meta property="og:title" content={pageTitle} key="ogtitle" />
          <meta property="og:description" content={description} key="ogdesc" />

          <meta name="description" content={description} />

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
