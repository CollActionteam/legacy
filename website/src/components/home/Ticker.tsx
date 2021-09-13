import React from "react";

export default function Ticker() {
  const sentences = [
    <>
      Let’s <span className="text-collaction">be vegan for a month.</span>
    </>,
    <>
      Let’s <span className="text-collaction">separate our waste.</span>
    </>,
    <>
      Let’s <span className="text-collaction">reduce food waste.</span>
    </>,
    <>
      Let’s{" "}
      <span className="text-collaction">
        switch to a green energy provider.
      </span>
    </>,
    <>
      Let’s{" "}
      <span className="text-collaction">meet with an elderly once a week.</span>
    </>,
    <>
      <span className="text-collaction font-medium">
        Together we make waves!
      </span>
    </>,
  ];

  return (
    <section className="p-5 py-20">
        <p className="text-lg md:text-4xl text-center rotatingText">
            {sentences[0]}
        </p>
    </section>
  );
}
