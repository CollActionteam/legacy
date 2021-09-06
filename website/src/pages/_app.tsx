import type { AppProps } from "next/app";
import Head from "next/head";
import "tailwindcss/tailwind.css";

function MyApp({ Component, pageProps }: AppProps) {
  return (
    <>
      <Head>
        <title>CollAction</title>
      </Head>
      <Component {...pageProps} />
    </>
  );
}
export default MyApp;
