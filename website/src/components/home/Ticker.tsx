import React, { useEffect, useState } from "react";
import clsx from "clsx";

export default function Ticker() {
  const activeSentences = [
    useState(true),
    useState(false),
    useState(false),
    useState(false),
    useState(false),
    useState(false),
  ]

  var [activeTickerIdx, setActiveTickerIdx] = useState(0);

  useEffect(() => {
    setTimeout(() => {
      var currentIdx = activeTickerIdx;
      activeSentences[currentIdx][1](false);
      if (currentIdx + 1 >= 6) {
        currentIdx = 0;
      } else {
        currentIdx += 1;
      }
      activeSentences[currentIdx][1](true);
      setActiveTickerIdx(currentIdx);
    }, 4000);
  }, []);

  return (
    <section className="p-5 py-12 md:py-32">
      <p className="text-xl md:text-4xl text-center ticker-wrapper">
        <div className="ticker-size-guide">
          Let&apos;s <span className="text-collaction">switch to a green energy provider.</span>
        </div>

        <div className={clsx("ticker-item", activeSentences[0][0] && "active-ticker-item")}>
          Let&apos;s <span className="text-collaction">be vegan for a month.</span>
        </div>
        <div className={clsx("ticker-item", activeSentences[1][0] && "active-ticker-item")}>
          Let&apos;s <span className="text-collaction">separate our waste.</span>
        </div>
        <div className={clsx("ticker-item", activeSentences[2][0] && "active-ticker-item")}>
          Let&apos;s <span className="text-collaction">reduce food waste.</span>
        </div>
        <div className={clsx("ticker-item", activeSentences[3][0] && "active-ticker-item")}>
          Let&apos;s <span className="text-collaction">switch to a green energy provider.</span>
        </div>
        <div className={clsx("ticker-item", activeSentences[4][0] && "active-ticker-item")}>
          Let&apos;s <span className="text-collaction">meet with an elderly once a week.</span>
        </div>
        <div className={clsx("ticker-item", activeSentences[5][0] && "active-ticker-item")}>
          <span className="text-collaction font-medium">
            Together we make waves!
          </span>
        </div>
      </p>
    </section>
  );
}
