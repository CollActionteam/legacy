import React from "react";
import styles from "./style.module.scss";

export default ({
  progress,
  radius = 30,
  stroke = 4,
  fontSize = "var(--font-size-sm)",
}: any) => {
  const normalizedRadius = radius - stroke * 2;
  const circumference = normalizedRadius * 2 * Math.PI;
  const chompedProgress = progress > 100 ? 100 : progress;
  const strokeDashoffset =
    circumference - (chompedProgress / 100) * circumference;

  return (
    <svg className={styles.ring} height={radius * 2} width={radius * 2}>
      <circle
        className={styles.backgroundCircle}
        strokeWidth={stroke}
        strokeDasharray={circumference + " " + circumference}
        r={normalizedRadius}
        cx={radius}
        cy={radius}
      />
      <circle
        className={styles.progressCircle}
        strokeWidth={stroke}
        strokeDasharray={circumference + " " + circumference}
        style={{ strokeDashoffset }}
        r={normalizedRadius}
        cx={radius}
        cy={radius}
      />
      <text x="50%" y="50%" textAnchor="middle" dy=".2rem" fontSize={fontSize}>
        {progress}%
      </text>
    </svg>
  );
};
