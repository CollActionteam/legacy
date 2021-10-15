import React, { useState } from "react";
import clsx from "clsx";

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

  const activeSentences = [
    useState(true),
    useState(false),
    useState(false),
    useState(false),
    useState(false),
    useState(false),
  ]

  var [activeTickerIdx, setActiveTickerIdx] = useState(0);
  setTimeout(() => {
    var currentIdx = activeTickerIdx;
    activeSentences[currentIdx][1](false);
    if (currentIdx + 1 >= sentences.length) {
      currentIdx = 0;
    } else {
      currentIdx += 1;
    }
    activeSentences[currentIdx][1](true);
    setActiveTickerIdx(currentIdx);
  }, 4000);

  const longestLine = sentences[4];

  return (
    <section className="p-5 py-12 md:py-32">
      <p className="text-xl md:text-4xl text-center ticker-wrapper">
        <div className="ticker-size-guide">
          {longestLine}
        </div>
        {sentences.map((sentence, idx) => (
          <div
            key={idx}
            className={clsx(
              "ticker-item",
              activeSentences[idx][0] && "active-ticker-item",
            )}
          >
            {sentence}
          </div>
        ))}
      </p>
    </section>
  );
}
