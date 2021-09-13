import React from "react";

export default function CurrentActicity() {
  return (
    <section className="px-5 py-10 md:px-10 md:py-20 text-center">
      <div className="container mx-auto">
        <div className="px-4">
          <h1 className="text-collaction text-2xl font-medium">
            What we are currently doing
          </h1>
          <p>
            Together with a team of developers from all over the world we’re
            building an app for iOS and Android. We aim to test the alpha
            version before October 2021. Would you like to receive updates on
            our progress and be one of the first to use our app? <br />
            <br />
            Join our{" "}
            <a
              className="text-collaction"
              target="_blank"
              rel="noreferrer"
              href="https://forms.gle/A233e3PHzA8VwEBGA"
            >
              mailing list
            </a>{" "}
            here and/or follow us on{" "}
            <a
              className="text-collaction"
              target="_blank"
              rel="noreferrer"
              href="https://www.instagram.com/collaction_org/"
            >
              Instagram
            </a>
            !
          </p>
        </div>
        <div className="px-4 pt-12">
          <h1 className="text-collaction text-2xl font-medium">History</h1>
          <p>
            CollAction was founded in 2015 by Ron van den Akker and Spencer
            Heijnen. After great successes with fashion-related crowdactions,
            their team decided to focus solely on{" "}
            <a className="text-collaction" href="https://slowfashion.global/">
              SlowFashion
            </a>{" "}
            in 2020. As there is still strong confidence that there&apos;s room
            for CollAction in this world, a new team has formed in 2021 and is
            expanding rapidly.
          </p>
        </div>
      </div>
    </section>
  );
}
